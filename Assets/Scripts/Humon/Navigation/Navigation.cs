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
        SetRandomDestination();
        nextGoalTime = Time.time + Random.Range(3f, 8f);
    }

    void Update()
    {
        if (agent.remainingDistance <= 1.0f || !agent.hasPath)
        {
            SetRandomDestination();
            nextGoalTime = Time.time + Random.Range(3f, 8f);
        }

        if (Time.time >= nextGoalTime)
        {
            SetRandomDestination();
            nextGoalTime = Time.time + Random.Range(3f, 8f);
        }
    }

    void SetRandomDestination()
    {
        Bounds bounds = surface.navMeshData.sourceBounds;

        Vector3 randomPoint = new Vector3(
            Random.Range(bounds.min.x, bounds.max.x),
            transform.position.y,
            Random.Range(bounds.min.z, bounds.max.z)
        );

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, 10f, 1 << NavMesh.GetAreaFromName("Walkable")))
        {
            agent.SetDestination(hit.position);
        }
    }
}