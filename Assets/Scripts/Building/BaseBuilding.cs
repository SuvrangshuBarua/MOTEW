using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;
using System.Collections.Generic;

namespace Building
{

public class StateImpl
{
    public bool IsInConstruction => (_state & Flag.InConstruction) != 0;
    public bool IsConstructed => (_state & Flag.Constructed) != 0;
    public bool IsInDestruction => (_state & Flag.InDestruction) != 0;
    public bool IsDestructed => (_state & Flag.Destructed) != 0;
    public bool IsOnFire => (_state & Flag.OnFire) != 0;


    private Flag _state = Flag.Destructed;

    internal void Set(Flag fl)
    {
        if (fl <= Flag.Destructed)
        {
            // first 4 flags are exclusive
            //_state &= ~0xf;
            Clear(Flag.InConstruction);
            Clear(Flag.Constructed);
            Clear(Flag.InDestruction);
            Clear(Flag.Destructed);
        }

        _state |= fl;
    }

    internal void Clear(Flag fl)
    {
        _state &= ~fl;
    }

    internal enum Flag
    {
        InConstruction  = (1 << 0),
        Constructed     = (1 << 1),
        InDestruction   = (1 << 2),
        Destructed      = (1 << 3),

        OnFire          = (1 << 4),
    }
};

public class BaseBuilding : MonoBehaviour
{
    /// State of the building
    public StateImpl State = new StateImpl();

    /// Callbacks for when construction finishes
    public System.Action OnConstructed;

    /// Radius workers must be in to help construction
    public float ConstructionSiteRadius => _constructionSiteRadius;

    /// Box collider of the building
    public BoxCollider Collider => _collider.GetComponent<BoxCollider>();

    /// Get current visitors
    public List<GameObject> Visitors()
    {
        return _visitors;
    }

    /// Visit the building.  Visiting temporarily disables the
    /// visitor and re-enables after duration seconds.
    public bool Visit(GameObject visitor, float duration, System.Action onExit)
    {
        if (!State.IsConstructed || _visitors.Count == _visitorCapacity)
            return false;

        visitor.SetActive(false);
        _visitors.Add(visitor);
        StartCoroutine(Leave(visitor, duration, onExit));

        return true;
    }

    /// Start constructing the building.  In order to construct
    /// the building, it has to be in the destructed state.
    public void Construct()
    {
        Assert.IsTrue(State.IsDestructed);

        _task = StartTask(DoConstruct());
    }

    /// Destruct the building.  In order to destroy the building,
    /// it must be in the constructed state.
    public void Destruct()
    {
        Assert.IsTrue(State.IsConstructed);

        _task = StartTask(DoDestruct());
    }

    public void AddConstructionWorker(GameObject worker)
    {
        _workers.Add(worker);
    }

    public void RemoveConstructionWorker(GameObject worker)
    {
        if (_workers.Contains(worker))
        {
            _workers.Remove(worker);
        }
    }

    public void SetOnFire(Fire source)
    {
        if (State.IsOnFire)
        {
            return;
        }

        State.Set(StateImpl.Flag.OnFire);
        var fire = gameObject.AddComponent<FireDamage>();
        fire.Damage = source.Damage;

        _fire_vfxs.Add(UnityEngine.Object.Instantiate(
                _firePrefab, transform));
        //.transform.localScale *= 0.5f;
    }




    // private impl


    private Health _health;

    [SerializeField]
    private uint _visitorCapacity;
    private List<GameObject> _visitors = new ();

    private Coroutine _task = null;

    [SerializeField]
    private GameObject _ruinPrefab = null;
    [SerializeField]
    private GameObject _buildingPrefab = null;
    private GameObject _building = null;
    [SerializeField]
    private GameObject _colliderPrefab = null;
    private GameObject _collider = null;

    private Material[] _mats;
    [SerializeField]
    private Material _hiddenMat;

    [SerializeField]
    private GameObject _firePrefab;
    private List<GameObject> _fire_vfxs = new ();

    [SerializeField]
    private GameObject _particle; 

    [SerializeField]
    private float _constructionTimeSeconds = 60f;
    private List<GameObject> _workers = new ();
    // TODO: make this dependent on the building size
    // if we're gonna have buildings of differnet sizes
    private float _constructionSiteRadius = 10;

    void CheckConstructionWorkers()
    {
        // dead
        _workers.RemoveAll((worker) => worker == null);

        // remove any workers that are not in range of the
        // construction site, aka buiding.
        _workers.RemoveAll((worker) => Vector3.Distance(
                transform.position, worker.transform.position) > _constructionSiteRadius);
    }

    public void InstantiateCollider()
    {
        Assert.IsNull(_collider);
        _collider = Instantiate(_colliderPrefab, transform);
        _collider.transform.localPosition= Vector3.zero;
    }

    void Awake()
    {
        _health = GetComponent<Health>();
        _health.OnDied += HandleBuildingDeath;
        _health.OnDamaged += OnDamaged;
    }

    void HandleBuildingDeath(DeathArgs args)
    {
        if (TryGetComponent(out FireDamage fire))
        {
            Destroy(fire);
            State.Clear(StateImpl.Flag.OnFire);
        }

        Destruct();
    }

    void OnDamaged(HealthChangedArgs args)
    {
        var cash = -args.Delta;
        if (cash <= 0)
        {
            return;
        }

        IndicatorManager.Instance.New(transform,
                $"${cash}");
        GameManager.Instance.AddCash(cash);
    }

    void OnDrawGizmos()
    {
        if (State.IsInConstruction)
        {
            DebugUtils.DrawCircle(transform.position, _constructionSiteRadius);
        }
    }

    private IEnumerator DoConstruct()
    {
        State.Set(StateImpl.Flag.InConstruction);

        _health.Heal(_health.BuildingMaxHealth);

        if (_building == null)
        {
            _building = Instantiate(_buildingPrefab, transform);
            _building.transform.localPosition = Vector3.zero;
            _building.layer = LayerMask.NameToLayer("NoNavMesh");

            _mats = _building.GetComponent<MeshRenderer>()
                    .sharedMaterials;

            Material[] hidden = new Material[_mats.Length];
            for (int i = 0; i < _mats.Length; i++)
            {
                hidden[i] = _hiddenMat;
            }

            _building.GetComponent<MeshRenderer>()
                    .sharedMaterials = hidden;
        }

        Assert.IsNotNull(_collider);
        FindFirstObjectByType<
                Unity.AI.Navigation.NavMeshSurface>()
                    .BuildNavMesh();

        var mesh = _building.GetComponent<MeshRenderer>();
        var materials = mesh.sharedMaterials;
        var time = _constructionTimeSeconds / materials.Length;

        for (var i = 0; i < materials.Length; )
        {
            if (_workers.Count == 0)
            {
                yield return new WaitForSeconds(1f);
                continue;
            }

            // we got workers, continue construction
            var wait = time / _workers.Count;
            yield return new WaitForSeconds(wait);

            materials[i] = _mats[i];
            mesh.sharedMaterials = materials;
            ++i;
        }

        // remove all workers
        _workers.Clear();
        // callbacks
        OnConstructed?.Invoke();
        OnConstructed = null;

        State.Set(StateImpl.Flag.Constructed);
    }

    private IEnumerator DoDestruct()
    {
        State.Set(StateImpl.Flag.InDestruction);

        // TDOO: draw cloud

        if (_building)
        {
            Destroy(_building);
            _mats = null;
        }

        _building = Instantiate(_ruinPrefab, transform);
        _building.transform.localPosition = Vector3.zero;
        //_building.layer = LayerMask.NameToLayer("NoNavMesh");

        // decay
        var mesh = _building.GetComponent<MeshRenderer>();
        var materials = mesh.sharedMaterials;

        // TODO: tweak this?
        yield return new WaitForSeconds(10f);
        var time = 10f / materials.Length;

        // detsroy fire vfx
        foreach (var fire in _fire_vfxs)
        {
            var ps = fire.GetComponent<ParticleSystem>();
            ps.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            Destroy(fire, 2f);
        }
        _fire_vfxs.Clear();

        for (int i = materials.Length - 1; i >= 0; --i)
        {
            yield return new WaitForSeconds(time);

            materials[i] = _hiddenMat;
            mesh.sharedMaterials = materials;
        }

        Destroy(_building);
        State.Set(StateImpl.Flag.Destructed);
    }

    private Coroutine StartTask(IEnumerator task)
    {
        // stop the current task, then start the new one
        if (_task != null)
        {
            StopCoroutine(_task);
        }

        return StartCoroutine(task);
    }

    private IEnumerator Leave(GameObject visitor, float duration, System.Action OnExit)
    {
        yield return new WaitForSeconds(duration);

        visitor.SetActive(true);
        _visitors.Remove(visitor);

        OnExit?.Invoke();
    }
}

} // namespace Building

