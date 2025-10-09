using UnityEngine;
using UnityEngine.Assertions;
using System.Collections.Generic;
using UnityEngine.AI;

namespace StateMachine
{   

public class NavMeshPathGenerator : MonoBehaviour
{
    public int pathLength = 5;
    public float stepRadius = 5f;
    public int maxAttemptsPerStep = 20;

    private List<Vector3> _path = new List<Vector3>();

    void Start()
    {
        GeneratePath();
    }

    void GeneratePath()
    {
        _path.Clear();
        Vector3 currentPosition = transform.position;
        _path.Add(currentPosition);


        // Optional: visualize the path
        for (int i = 0; i < _path.Count - 1; i++)
        {
            Debug.DrawLine(_path[i], _path[i + 1], Color.green, 10f);
        }
    }
}

public class PanicState : IState
{
    public GameObject Threat = null;
    public SpeechBubble SpeechBubble = null;

    private Nav.Path _path = null;

    public void Enter(Humon npc)
    {
        Assert.IsNotNull(Threat, "Threat must be set (source of panic)");

        // generate a random path away from threat

        var points = 10;
        var threatPos = Threat.transform.position;
        var origin = npc.transform.position;

        var positions = new Queue<Vector3>();
        float stepRadius = 6f;

        var current = npc.transform.position;

        for (int i = 1; i < points; i++)
        {
            for (int attempt = 0; attempt < 10; attempt++)
            {
                Vector3 off = Random.insideUnitSphere * stepRadius;
                off.y = 0;
                Vector3 pos = current + off;

                if (NavMesh.SamplePosition(pos, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
                {
                    positions.Enqueue(hit.position);
                    current = hit.position;
                    break;
                }
            }
        }

        //DebugUtils.DrawPath(new List<Vector3>(positions.ToArray()));

        _path = new Nav.Path(npc.GetComponent<Navigation>(),
                positions);

        npc.Navigation.TogglePanicSpeed();

        // TODO: draw bubble with ?!
        SpeechBubble = SpeechBubbleManager.Instance.CreateSpeechBubble(npc.transform, "?!");
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
        npc.Navigation.StopMoving();

        Threat = null;
        SpeechBubble = null;
        _path = null;
    }

    public State GetState()
    {
        return State.Panic;
    }
}

} // namespace StateMachine

