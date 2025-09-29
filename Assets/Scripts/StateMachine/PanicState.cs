using UnityEngine;
using UnityEngine.Assertions;
using System.Collections.Generic;
using UnityEngine.AI;

namespace StateMachine
{   

public class PanicState : IState
{
    public GameObject Threat = null;
    private Nav.Path _path = null;

    public void Enter(Humon npc)
    {
        Assert.IsNotNull(Threat, "Threat must be set (source of panic)");

        // generate a zig-zagging path away from threat

        var totalDistance = 20f;
        var zigzagWidth = 1f;
        var points = 10;
        var threatPos = Threat.transform.position;
        var origin = npc.transform.position;

        var positions = new Queue<Vector3>();
        Vector3 awayDir = (origin - threatPos).normalized;
        float stepDist = totalDistance / points;

        Vector3 perpendicular = Vector3.Cross(awayDir, Vector3.up).normalized;

        for (int i = 1; i <= points; i++)
        {
            Vector3 basePoint = origin + awayDir * (stepDist * i);
            float offsetDir = (i % 2 == 0) ? 1f : -1f;
            Vector3 zigzagOffset = perpendicular * zigzagWidth * offsetDir;
            Vector3 zigzagPoint = basePoint + zigzagOffset;

            if (NavMesh.SamplePosition(zigzagPoint, out NavMeshHit hit, 2f, NavMesh.AllAreas))
            {
                positions.Enqueue(hit.position);
            }
            else
            {
                // fallback
                if (NavMesh.SamplePosition(basePoint, out NavMeshHit baseHit, 2f, NavMesh.AllAreas))
                {
                    positions.Enqueue(baseHit.position);
                }
            }
        }

        //DebugUtils.DrawPath(new List<Vector3>(positions.ToArray()));

        _path = new Nav.Path(npc.GetComponent<Navigation>(),
                positions);

        npc.Navigation.TogglePanicSpeed();

        // TODO: draw bubble with ?!
    }

    public void Update(Humon npc)
    {
        if (_path.ReachedDestination)
        {
            npc.StateMachine.ChangeState<RoamState>();
            return;
        }

        _path.Update();
    }

    public void Exit(Humon npc)
    {
        npc.Navigation.TogglePanicSpeed();

        Threat = null;
        _path = null;
    }

    public State GetState()
    {
        return State.Panic;
    }
}

} // namespace StateMachine

