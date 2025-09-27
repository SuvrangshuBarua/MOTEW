using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class Navigation : MonoBehaviour
{
    public NavMeshAgent Agent { get { return agent; } }
    public bool ReachedDestination => _reachedDestination;

    private NavMeshAgent agent;
    [SerializeField] public NavMeshSurface Surface;
    private float nextGoalTime;

    private bool _reachedDestination = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        nextGoalTime = Time.time + Random.Range(3f, 8f);
    }

    void Update()
    {
        if (agent.remainingDistance <= 1.0f || !agent.hasPath || Time.time >= nextGoalTime)
        {
            _reachedDestination = true;
        }
    }

    public Vector3 GetRandomDestination()
    {
        Vector3 randomPoint = TakeRandomPoint();

        if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, 10f, 1 << NavMesh.GetAreaFromName("Walkable")))
        {
            return hit.position;
        }
        return transform.position;
    }

    Vector3 TakeRandomPoint()
    {
        Bounds bounds = Surface.navMeshData.sourceBounds;

        return new Vector3(
            Random.Range(bounds.min.x, bounds.max.x),
            transform.position.y,
            Random.Range(bounds.min.z, bounds.max.z)
        );
    }

    public void SetDestination(Vector3 position)
    {
        _reachedDestination = false;
        agent.SetDestination(position);
        nextGoalTime = Time.time + Random.Range(3f, 8f);
    }
}

