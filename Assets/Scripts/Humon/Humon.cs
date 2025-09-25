using System;
using God;
using StateMachine;
using Unity.Behavior;
using UnityEngine;

public class Humon : MonoBehaviour
{
    private Perception _perception;
    public Perception Perception => _perception;
    
    private Navigation _navigation;
    public Navigation Navigation => _navigation;
    
    private BehaviorGraphAgent _agent;
    private StateMachine.StateMachine _stateMachine;
    private Draggable _draggable;

    private void Awake()
    {
        _perception = gameObject.AddComponent<Perception>();
        _navigation = gameObject.GetComponent<Navigation>();  
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
        
        _stateMachine.ChangeState<RoamState>();
    }

    private void Update()
    {
        _stateMachine.Update();
    }

    private void OnDragEnd()
    {
        //_agent.BlackboardReference.SetVariableValue("interrupt", false);
    }

    private void OnStartDrag()
    {
        //_agent.BlackboardReference.SetVariableValue("interrupt", true);
    }

    private void OnDestroy()
    {
        _draggable.OnStartDrag -= OnStartDrag;
        _draggable.OnDragEnd -= OnDragEnd;
    }
    
}

