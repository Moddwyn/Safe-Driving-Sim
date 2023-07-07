using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PedestrianSpawner : MonoBehaviour
{
    public Pedestrian pedestrianPrefab;
    public int maxPedestrian = 20;

    void Start()
    {
        Spawn();
    }

    void Spawn()
    {
        for (int i = 0; i < maxPedestrian; i++)
        {
            Instantiate(pedestrianPrefab, GetRandomPointOnNavMesh(30, 100), Quaternion.identity);
        }
    }

    public static Vector3 GetRandomPointOnNavMesh(int maxAttempts, float maxDistance)
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
