using GrimTools.Runtime.Core;
using UnityEngine;
using UnityEngine.Assertions;
using System;
using System.Collections;
using System.Collections.Generic;

using Random = UnityEngine.Random;

public class GameManager : MonoSingleton<GameManager>
{
    public bool canAfford;
    public DialogueData IntroDialogue;
    public DialogueData OutroDialogue;
    public DialogueSystem DialogueSystem;
    public event Action<int> OnCashChanged;
    public uint Population => (uint) _humons.Count;

    public int HumonDeathCount => _humonDeathCount;
    public int Souls = 0;
    public event Action<int> OnHumonDeathCountChanged;


    public uint PopulationCapacity()
    {
        uint capacity = 0;

        foreach (var residence in _residences)
        {
            capacity += residence.ResidentCapacity;
        }

        return capacity;
    }

    public void IncreaseDeathCount()
    {
        _humonDeathCount++;
        Souls++;
        OnHumonDeathCountChanged?.Invoke(_humonDeathCount);
    }

    public void UpdateUI()
    {
        OnCashChanged?.Invoke(_cash);
    }



    private int _humonDeathCount = 0;
    private int _maxPopulation = 120;

    private int _cash = 0;
    private Bounds _bounds;

    private bool _won = false;

    [SerializeField] private Humon _humonPrefab;
    [SerializeField] private Building.Residence[] _residencePrefabs;

    private List<Humon> _humons = new ();
    private List<Building.Residence> _residences = new ();

    private void FixedUpdate()
    {
        _humons.RemoveAll((humon) => humon == null);

        var capacity = PopulationCapacity();
        if (Population >= capacity
                && capacity < _maxPopulation)
        {
            SpawnResidence();
        }

        if (Win())
        {
            //Time.timeScale = 0f;
            var now = System.TimeSpan.FromSeconds(Time.time);
            OutroDialogue.lines[OutroDialogue.lines.Count - 1]
                    .text = "Time: " + string.Format(
                        "{0:D2}:{1:D2}:{2:D2}",
                        now.Hours, now.Minutes, now.Seconds);
            DialogueSystem.StartDialogue(OutroDialogue);
        }

        // FIXME: out of map -> die
        List<Humon> remove = new ();
        foreach (var humon in _humons)
        {
            if (humon.transform.position.y < -10)
            {
                remove.Add(humon);
            }
        }

        foreach (var humon in remove)
        {
            humon.GetComponent<Health>().TakeDamage(100000);
        }
    }

    protected virtual void Awake()
    {
        base.Awake();
        
        var nav = FindFirstObjectByType<
                Unity.AI.Navigation.NavMeshSurface>();
        _bounds = nav.navMeshData.sourceBounds;


        // spawn a house so that the game can't
        // be won instantly
        SpawnResidenceAt(new Vector3(-4, 0, 0));
        _residences[0].GetComponent<Building.BaseBuilding>()
                .ConstructionTimeSeconds = 10f;
    }

    private void Start()
    {
        _humons.AddRange(FindObjectsByType<Humon>(default));
        DialogueSystem.StartDialogue(IntroDialogue);

        UpdateUI();
    }

    private bool Win()
    {
        if (_won)
        {
            return false;
        }

        if (Population > 0)
        {
            return false;
        }

        foreach (var residence in _residences)
        {
            if (residence.State.IsConstructed)
            {
                return false;
            }
        }

        _won = true;
        return true;
    }


    public Humon SpawnHumon(Vector3 pos)
    {
        var humon = Instantiate(_humonPrefab, pos,
                Quaternion.identity);
        _humons.Add(humon);
        return humon;
    }

    public void DestroyHumon(Humon humon)
    {
        _humons.Remove(humon);
        //Destroy(humon);
    }

    public void AddCash(int amount)
    {
        _cash += amount;
        OnCashChanged?.Invoke(_cash);
    }

    public int GetCash()
    {
        return _cash;
    }

    public void SetCash(int amount)
    {
        _cash = amount;
        OnCashChanged?.Invoke(_cash);
    }

    public bool TryDeductCash(int amount)
    {
        if (CanDeductCash(amount))
        {
            _cash -= amount;
            OnCashChanged?.Invoke(_cash);
            return true;
        }
        return false;
    }

    public bool CanDeductCash(int amount)
    {
        if (_cash >= amount)
            return true;
        return false;
    }

    private void SpawnResidence()
    {
        var prefab = _residencePrefabs[
                Random.Range(0, _residencePrefabs.Length)];
        var residence = Instantiate(prefab,
                new Vector3(0, 10000, 0),
                new Quaternion(0, Random.value, 0, 1));

        residence.InstantiateCollider();

        var pos = TryPlaceObject(residence.Collider);

        if (pos == null)
        {
            Debug.LogWarning(
                    "Found no place to spawn residence");
            Destroy(residence.gameObject);
            return;
        }

        residence.gameObject.transform.position = pos.Value;
        // NOTE: add whatever offset needed so that
        // the assets are level with the ground...
        residence.gameObject.transform.position +=
                prefab.transform.position;
        _residences.Add(residence);

        // rebake mesh
        StartCoroutine(DeferredRebake());
    }

    private void SpawnResidenceAt(Vector3 pos)
    {
        var prefab = _residencePrefabs[
                Random.Range(0, _residencePrefabs.Length)];
        var residence = Instantiate(prefab,
                new Vector3(0, 10000, 0),
                new Quaternion(0, Random.value, 0, 1));

        residence.InstantiateCollider();

        residence.gameObject.transform.position = pos;
        // NOTE: add whatever offset needed so that
        // the assets are level with the ground...
        residence.gameObject.transform.position +=
                prefab.transform.position;
        _residences.Add(residence);

        // rebake mesh
        StartCoroutine(DeferredRebake());
    }

    private IEnumerator DeferredRebake()
    {
        // defer at least 1 frame
        yield return null;

        var surface = FindFirstObjectByType<
                Unity.AI.Navigation.NavMeshSurface>();
        surface.useGeometry = UnityEngine.AI
                .NavMeshCollectGeometry.PhysicsColliders;
        surface.BuildNavMesh();
    }

    private Vector3? TryPlaceObject(Collider col)
    {
        var box = col as BoxCollider;
        Assert.IsNotNull(box);
        var half = Vector3.Scale(
                box.size * 0.5f, box.transform.lossyScale);

        // make sure the object is not placed too close
        var buffer = 2f;
        half.x += buffer;
        half.z += buffer;

        // HACK: navMeshSurface.sourceBounds uses underlying
        // geometry, not the nav mesh
        const float off = 10;

        var min = new Vector2(
                _bounds.min.x + MathF.Sqrt(2) * half.x + off,
                _bounds.min.z + MathF.Sqrt(2) * half.z + off);
        var max = new Vector2(
                _bounds.max.x - MathF.Sqrt(2) * half.x - off,
                _bounds.max.z - MathF.Sqrt(2) * half.z - off);

        var surface = FindFirstObjectByType<
                Unity.AI.Navigation.NavMeshSurface>();

        for (var tries = 0; tries < 100; ++tries)
        {
            var pos = new Vector3(
                    Random.Range(min.x, max.x),
                    0,
                    Random.Range(min.y, max.y));

            if (!UnityEngine.AI.NavMesh.SamplePosition(pos,
                    out var hit, 1f,
                    UnityEngine.AI.NavMesh.AllAreas))
            {
                continue;
            }

            var cols = Physics.OverlapBox(pos, half,
                    col.transform.rotation,
                    LayerMask.GetMask("Building", "NPC"));

            if (cols.Length == 0)
            {
                return pos;
            }
        }

        return null;
    }
} 

