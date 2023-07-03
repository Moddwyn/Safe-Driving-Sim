using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Pedestrian : MonoBehaviour
{
    public float wanderRadius = 10f;                 // Maximum distance for the wander movement
    public NavMeshAgent agent;

    private Vector3 targetPosition;

    void Start()
    {
        InvokeRepeating("SetNewDestination", 0, 5);
    }

    private void OnDrawGizmosSelected()
    {
        // Draw detection radius
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, wanderRadius);
    }
    
    private void SetNewDestination()
    {
        Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
        randomDirection += transform.position;

        NavMeshHit navMeshHit;
        if (NavMesh.SamplePosition(randomDirection, out navMeshHit, wanderRadius, NavMesh.AllAreas))
        {
            targetPosition = navMeshHit.position;
            agent.SetDestination(targetPosition);
        }
    }
}
