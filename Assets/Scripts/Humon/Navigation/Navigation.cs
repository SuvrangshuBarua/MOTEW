using System;
using Unity.AI.Navigation;
using UnityEngine;

using UnityEngine.AI;
using Random = UnityEngine.Random;

public class Navigation : MonoBehaviour
{
    
    private NavMeshAgent agent;
    
    [SerializeField] private NavMeshSurface surface;
    private float nextGoalTime;
    
    public NavMeshAgent Agent => agent;
    public bool IsMoving => agent.velocity.sqrMagnitude > 0.01f;
    public bool HasPath => agent.hasPath;
    public float RemainingDistance => agent.remainingDistance;
    public float Speed => agent.speed;
    public float RemainingTime => agent.remainingDistance / agent.speed;
    public bool ReachedDestination => !agent.pathPending && agent.remainingDistance <= 1.0f;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Start()
    {
        
        /*GetRandomDestination();
        nextGoalTime = Time.time + Random.Range(3f, 8f);*/
    }

    void Update()
    {
        /*if (agent.remainingDistance <= 1.0f || !agent.hasPath || Time.time >= nextGoalTime)
        {
            GetRandomDestination();
            nextGoalTime = Time.time + Random.Range(3f, 8f);
        }*/
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

    public void StopMoving()
    {
        if(agent.isOnNavMesh)
             agent.ResetPath();
    }

    public void SetDestination(Vector3 position)
    {
        agent.SetDestination(position);
    }
}