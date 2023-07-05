using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEditor;

public class Pedestrian : MonoBehaviour
{
    public int maxAttempts = 30; // Maximum number of attempts to find a valid random point
    public float maxDistance = 100f; // Maximum distance from the agent to sample a random point
    public float idleTime = 5f; // Idle time when it reaches destination
    public NavMeshAgent agent;
    public Animator animator;

    [Space(20)]
    public bool isWaiting = false;

    void Start()
    {
        SetNewDestination();
    }

    void Update()
    {
        // Check if the agent has reached its current destination
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance && !isWaiting)
        {
            StartCoroutine(WaitBeforeNewDestination());
        }

        animator.SetBool("Walking", !isWaiting);
    }

    void SetNewDestination()
    {
        Vector3 randomDestination = GetRandomPointOnNavMesh();
        agent.SetDestination(randomDestination);
    }

    IEnumerator WaitBeforeNewDestination()
    {
        isWaiting = true;
        yield return new WaitForSeconds(idleTime);
        SetNewDestination();
        isWaiting = false;
    }

    public Vector3 GetRandomPointOnNavMesh()
    {
        NavMeshTriangulation navMeshData = NavMesh.CalculateTriangulation();

        // Calculate the bounds of the NavMesh
        float minX = float.MaxValue;
        float maxX = float.MinValue;
        float minY = float.MaxValue;
        float maxY = float.MinValue;
        float minZ = float.MaxValue;
        float maxZ = float.MinValue;

        for (int i = 0; i < navMeshData.vertices.Length; i++)
        {
            Vector3 vertex = navMeshData.vertices[i];
            minX = Mathf.Min(minX, vertex.x);
            maxX = Mathf.Max(maxX, vertex.x);
            minY = Mathf.Min(minY, vertex.y);
            maxY = Mathf.Max(maxY, vertex.y);
            minZ = Mathf.Min(minZ, vertex.z);
            maxZ = Mathf.Max(maxZ, vertex.z);
        }

        int attempt = 0;
        while (attempt < maxAttempts)
        {
            // Generate a random point within the NavMesh bounds
            Vector3 randomPoint = new Vector3(
                Random.Range(minX, maxX),
                Random.Range(minY, maxY),
                Random.Range(minZ, maxZ)
            );

            // Sample the nearest valid position on the NavMesh
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, maxDistance, NavMesh.AllAreas))
            {
                if(hit.mask == 8) continue;
                return hit.position;
            }

            attempt++;
        }

        // If no valid position found within the maximum number of attempts, return Vector3.zero
        Debug.LogWarning("Could not find a valid random point on the NavMesh.");
        return Vector3.zero;
    }
}
