using System;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Deactivate Agent", story: "Deactivate [Agent]", category: "Action", id: "f93510176b2185a62de9bc817874c31d")]
public partial class DeactivateAgentAction : Action
{
    [SerializeReference] public BlackboardVariable<NavMeshAgent> Agent;
    protected override Status OnStart()
    {
        Agent.Value.GetComponent<Rigidbody>().useGravity = true;
        Agent.Value.enabled = false;
        
        return Status.Success;
    }

    protected override Status OnUpdate()
    {

        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

