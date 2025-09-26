using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "GetPositionFromHumon", story: "Get [Position] from [Humon]", category: "Action", id: "5a8c9da3fd9c05659f7457797298524d")]
public partial class GetPositionFromHumonAction : Action
{
    [SerializeReference] public BlackboardVariable<Vector3> Position;
    [SerializeReference] public BlackboardVariable<Humon> Humon;

    protected override Status OnStart()
    {
        if(Humon.Value.Navigation.Agent.GetComponent<Rigidbody>().useGravity)
            Humon.Value.Navigation.Agent.GetComponent<Rigidbody>().useGravity = false;
        Position.Value = Humon.Value.Navigation.GetRandomDestination();
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

