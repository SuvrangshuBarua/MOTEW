using System;
using System.Collections.Generic;
using Unity.Behavior;
using UnityEngine;
using Composite = Unity.Behavior.Composite;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "WeightedRandom", story: "Considers [weights] with Random", category: "Flow", id: "f767313eede30b012c243477250d25e9")]
public partial class WeightedRandom : Composite
{
    [SerializeReference] public BlackboardVariable<List<float>> Weights;
    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

