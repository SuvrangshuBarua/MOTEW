using System.Collections;
using God;
using StateMachine;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;

public class Humon : MonoBehaviour
{
    [SerializeField] private Stats stats;

    [Header("Core Components: ")]
    private Perception _perception;
    private Navigation _navigation;
    private Rigidbody _rigidbody;
    private Collider _collider;
    public bool IsBeingDragged => _draggable.IsDragged;
    public Perception Perception => _perception;
    public Navigation Navigation => _navigation;
    public Rigidbody Rigidbody => _rigidbody;
    public StateMachine.StateMachine StateMachine => _stateMachine;
    
    private BehaviorGraphAgent _agent;
    private StateMachine.StateMachine _stateMachine;
    private Draggable _draggable;
    private Coroutine _dropCoroutine;
    private FallDamage _fallDamage;
    private IHealth _health;

    private void Awake()
    {
        _perception = gameObject.AddComponent<Perception>();
        _navigation = gameObject.GetComponent<Navigation>();

        _rigidbody = gameObject.GetComponent<Rigidbody>();
        _collider = gameObject.GetComponent<Collider>();

        _agent = gameObject.GetComponent<BehaviorGraphAgent>();
        _draggable = gameObject.GetComponent<Draggable>();

        _stateMachine = new StateMachine.StateMachine(this);

        _health = GetComponent<IHealth>();
        _fallDamage = gameObject.AddComponent<FallDamage>();

        // Fall damage
        // TODO: check logic here
        _fallDamage.StatsAsset = stats;
        _fallDamage.OnLand += () =>
        {
            if (_stateMachine.CurrentState.GetState() != State.Construction)
                _stateMachine.ChangeState<RoamState>();
        };

        if (_health != null) _health.OnDied += HandleDeath; // subscribe to death event

        InitializeStateMachine();
        InitializePerception();
        _stateMachine.ChangeState<RoamState>();
        _draggable.OnStartDrag += OnStartDrag;
        _draggable.OnDragEnd += OnDragEnd;
    }

    void Start()
    {
    }

    private void InitializeStateMachine()
    {
        _stateMachine.AddState(new RoamState());
        _stateMachine.AddState(new InAirState());
        _stateMachine.AddState(new ConstructionState());
        _stateMachine.AddState(new PanicState());
        _stateMachine.AddState(new SocializeState());
        _stateMachine.AddState(new DeadState());
    }

    void InitializePerception()
    {
        _perception.Subscribe(5, 1, Perception.Type.Single,
                LayerMask.GetMask("Building"), OnPerceiveBuilding);

        _perception.Subscribe(6, 0.1f, Perception.Type.Multiple,
                LayerMask.GetMask("NPC"), OnPerceiveHumonPanic);

        _perception.Subscribe(5, 10, Perception.Type.Single,
                LayerMask.GetMask("NPC"), OnPerceiveHumonSocialize);
    }

    private void Update()
    {
        _stateMachine.Update();
    }

    private void OnStartDrag()
    {
        _stateMachine.ChangeState<InAirState>();
        _fallDamage.BeginAirborne();
    }

    private void OnDragEnd()
    {
    }

    public void DropToSurface()
    {
        if (_dropCoroutine != null) return;
        _dropCoroutine = StartCoroutine(DropToSurfaceRoutine());
    }

    private IEnumerator DropToSurfaceRoutine()
    {
        _rigidbody.isKinematic = false;
        _rigidbody.useGravity = true;
        _rigidbody.linearDamping = 0.0f;

        if (Navigation.Agent.enabled)
        {
            Navigation.Agent.enabled = false;
        }

        bool isOnGround = false;

        while (!isOnGround && _rigidbody.linearVelocity.y <= 0.0f)
        {
            float checkDistance = 0.2f;
            RaycastHit hit;
            Vector3 rayOrigin = transform.position - new Vector3(0, _collider.bounds.extents.y, 0);
            if (Physics.Raycast(rayOrigin, Vector3.down, out hit, checkDistance, LayerMask.GetMask("Terrain"), QueryTriggerInteraction.Ignore))
            {
                isOnGround = true;
            }

            yield return new WaitForFixedUpdate();
        }

        NavMeshHit navHit;
        if (NavMesh.SamplePosition(transform.position, out navHit, 10f, 1 << NavMesh.GetAreaFromName("Walkable")))
        {
            transform.position = navHit.position;

        }

        _rigidbody.isKinematic = true;
        _rigidbody.useGravity = false;
        _rigidbody.linearVelocity = Vector3.zero;

        if (!Navigation.Agent.enabled)
        {
            Navigation.Agent.enabled = true;
        }

        yield return null;

        _stateMachine.ChangeState<RoamState>();
        _dropCoroutine = null;
    }

    void OnPerceiveBuilding(Collider collider)
    {
        var building = collider.GetComponentInParent<Building.BaseBuilding>();

        if (_stateMachine.CurrentState.GetState() == State.Panic)
        {
            // try to hide in the building
            if (building.Visit(gameObject, 5f, OnExitBuilding))
            {
                _stateMachine.GetState<PanicState>().SpeechBubble.Hide();
            }
            return;
        }

        if (building.State.IsInConstruction || building.State.IsDestructed)
        {
            var state = _stateMachine.CurrentState.GetState();
            if (_stateMachine.CurrentState.GetState() == State.Roam)
            {
                if (building.State.IsDestructed)
                {
                    building.Construct();
                }

                _stateMachine.GetState<ConstructionState>().Building = building;
                _stateMachine.ChangeState<ConstructionState>();
                return;
            }
        }
    }

    void OnExitBuilding()
    {
        _stateMachine.ChangeState<RoamState>();
    }

    void OnPerceiveHumonPanic(Collider other)
    {
        var humon = other.GetComponent<Humon>();
        
        if (humon.StateMachine.CurrentState.GetState() == State.InAir)
        {
            // spotted a floating humon -> panic
            if (_stateMachine.CurrentState.GetState() != State.Panic)
            {
                _stateMachine.GetState<PanicState>().Threat = humon.gameObject;
                _stateMachine.ChangeState<PanicState>();
                return;
            }
        }
    }

    void OnPerceiveHumonSocialize(Collider other)
    {
        var humon = other.GetComponent<Humon>();

        // both humons must be roaming
        if (StateMachine.CurrentState.GetState() != State.Roam
            || humon.StateMachine.CurrentState.GetState() != State.Roam)
        {
            return;
        }

        StateMachine.GetState<SocializeState>().Other = humon;
        StateMachine.GetState<SocializeState>().Speak = true;
        humon.StateMachine.GetState<SocializeState>().Other = this;

        StateMachine.ChangeState<SocializeState>();
        humon.StateMachine.ChangeState<SocializeState>();
    }

    private void HandleDeath(DeathArgs args)
    {
        StateMachine.ChangeState<DeadState>();

        IndicatorManager.Instance.New(transform, "$10");
        GameManager.Instance.AddCash(10);

        //Debug.Log($"[Humon] {name} destroyed (source: {args.Source})"); // DEBUG
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (_health != null) _health.OnDied -= HandleDeath;
        _draggable.OnStartDrag -= OnStartDrag;
        _draggable.OnDragEnd -= OnDragEnd;
    }
}


