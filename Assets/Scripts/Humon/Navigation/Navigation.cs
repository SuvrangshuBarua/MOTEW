using Unity.AI.Navigation;
using UnityEngine;

using UnityEngine.AI;

public class Navigation : MonoBehaviour
{
    public NavMeshAgent Agent { get { return agent; } }
    private NavMeshAgent agent;
    [SerializeField] private NavMeshSurface surface;
    private float nextGoalTime;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        GetRandomDestination();
        nextGoalTime = Time.time + Random.Range(3f, 8f);
    }

    void Update()
    {
        if (agent.remainingDistance <= 1.0f || !agent.hasPath || Time.time >= nextGoalTime)
        {
            GetRandomDestination();
            nextGoalTime = Time.time + Random.Range(3f, 8f);
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
        Bounds bounds = surface.navMeshData.sourceBounds;

        return new Vector3(
            Random.Range(bounds.min.x, bounds.max.x),
            transform.position.y,
            Random.Range(bounds.min.z, bounds.max.z)
        );
    }

    public void SetDestination(Vector3 position)
    {
        agent.SetDestination(position);
    }
}