using UnityEngine;

using UnityEngine.AI;
using System.Collections;

public class Navigation : MonoBehaviour
{
    private NavMeshAgent agent;
    private float nextGoalTime;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        SetRandomDestination();
        nextGoalTime = Time.time + Random.Range(2f, 5f);
    }

    void Update()
    {
        if (Time.time >= nextGoalTime)
        {
            SetRandomDestination();
            nextGoalTime = Time.time + Random.Range(2f, 5f);
        }
    }

    void SetRandomDestination()
    {
        Vector3 randomDirection = Random.insideUnitSphere * 10f; // Change 10f to your desired range
        randomDirection += transform.position;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, 10f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }
}
