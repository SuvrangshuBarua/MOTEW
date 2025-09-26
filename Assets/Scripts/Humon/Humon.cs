using System;
using System.Collections;
using System.Collections.Generic;
using God;
using StateMachine;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;

public class Humon : MonoBehaviour

{
    [SerializeField] private Stats stats;

    private Health _health;



    private void OnEnable()
    {
        if (_health != null)
            _health.OnDied += HandleDeath;
    }

    private void OnDisable()
    {
        if (_health != null)
            _health.OnDied -= HandleDeath;
    }
    

    void HandleDeath(DeathArgs args)
    {
        Debug.Log($"[Humon] {name} destroyed (source: {args.Source})"); // DEBUG
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"[Humon] {name} collided with {collision.collider.name}"); // DEBUG
        // Handle fall damage
        if (_health == null || _health.IsDead || stats == null) return;

        float impactSpeed = collision.relativeVelocity.magnitude;

        // Calculates fall damage based on impact speed and stats
        // ? Later change to height-based damage? 
        if (impactSpeed > stats.MinImpactSpeed)
        {
            // calculate: (ImpactSpeed − MinImpactSpeed) x DamagePerSpeed
            float over = impactSpeed - stats.MinImpactSpeed;
            int damage = Mathf.Clamp(Mathf.RoundToInt(over * stats.DamagePerSpeed), 1, stats.MaxFallDamage);

            Debug.Log($"[Humon] Impact {impactSpeed:F2} -> fall damage {damage}"); // DEBUG
            _health.TakeDamage(damage, source: "fall");
        }
    }

    [Header("Core Components: ")]
    private Perception _perception;
    private Navigation _navigation;
    private Rigidbody _rigidbody;
    private Collider _collider;
    public bool IsBeingDragged => _draggable.IsDragged;
    public Perception Perception => _perception;
    public Navigation Navigation => _navigation;
    public Rigidbody Rigidbody => _rigidbody;
    
    private BehaviorGraphAgent _agent;
    private StateMachine.StateMachine _stateMachine;
    private Draggable _draggable;
    private Coroutine _dropCoroutine;

    private void Awake()
    {
        _perception = gameObject.AddComponent<Perception>();
        _navigation = gameObject.GetComponent<Navigation>();  
        _rigidbody = gameObject.GetComponent<Rigidbody>();
        _collider = gameObject.GetComponent<Collider>();
        _health = gameObject.GetComponent<Health>();
        
        _agent = gameObject.GetComponent<BehaviorGraphAgent>();
        _draggable = gameObject.GetComponent<Draggable>();
        
        _stateMachine = new StateMachine.StateMachine(this);
    }

    void Start()
    {
        InitializeStateMachine();
        _draggable.OnStartDrag += OnStartDrag;
        _draggable.OnDragEnd += OnDragEnd;
    }

    private void InitializeStateMachine()
    {
        _stateMachine.AddState(new RoamState());
        _stateMachine.AddState(new InAirState());
        _stateMachine.ChangeState<RoamState>();
    }

    private void Update()
    {
        _stateMachine.Update();
    }
    
    private void OnStartDrag()
    {
        _stateMachine.ChangeState<InAirState>();
    }
    private void OnDragEnd()
    {
        

    }

    public void DropToSurface()
    {
        if(_dropCoroutine != null) return;
        _dropCoroutine = StartCoroutine(DropToSurfaceRoutine());
    }
    private IEnumerator DropToSurfaceRoutine()
    {
        _rigidbody.isKinematic = false;
        _rigidbody.useGravity = true;
        

        if (Navigation.Agent.enabled)
        {
            Navigation.Agent.enabled = false;
        }
        
        bool isOnGround = false;

        while (!isOnGround && _rigidbody.linearVelocity.y <= 0.0f)
        {
            float checkDistance = 0.2f;
            RaycastHit hit;
            Vector3 rayOrigin = transform.position - new Vector3(0, _collider.bounds.extents.y, 0 );
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
            
        if (!Navigation.Agent.enabled)
        {
            Navigation.Agent.enabled = true;
        }
        
        yield return null;

        _stateMachine.ChangeState<RoamState>();
        _dropCoroutine = null;
    }
    private void OnDestroy()
    {
        _draggable.OnStartDrag -= OnStartDrag;
        _draggable.OnDragEnd -= OnDragEnd;

    }
    
}

