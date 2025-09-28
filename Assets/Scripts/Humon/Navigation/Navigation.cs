using System;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class Navigation : MonoBehaviour
{
    private NavMeshAgent _agent;
    
    [SerializeField] private NavMeshSurface surface;

    private float nextGoalTime;
    
    public NavMeshAgent Agent => _agent;
    public bool IsMoving => _agent.velocity.sqrMagnitude > 0.01f;
    public bool HasPath => _agent.hasPath;
    public float RemainingDistance => _agent.remainingDistance;
    public float Speed => _agent.speed;
    public float RemainingTime => _agent.remainingDistance / _agent.speed;
    public bool ReachedDestination => !_agent.pathPending && _agent.remainingDistance <= 1.0f;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
    }

    private bool _reachedDestination = false;

    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        nextGoalTime = Time.time + Random.Range(3f, 8f);
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
        if(_agent.isOnNavMesh)
        {
             _agent.ResetPath();
        }
    }

    public void SetDestination(Vector3 position)
    {
        _reachedDestination = false;
        _agent.SetDestination(position);
        nextGoalTime = Time.time + Random.Range(3f, 8f);
    }
}

