using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.AI;

public class PedestrianManager : MonoBehaviour
{
    public GameObject pedestrianPrefab;
    public int maxPedestrian = 20;

    bool toggleOnAreas;

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

    public Vector3 GetRandomPointOnNavMesh(int maxAttempts, float maxDistance)
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

    [Button("Show/Hide Navigation Areas")]
    public void ShowHideNavArea()
    {
        toggleOnAreas = !toggleOnAreas;
        foreach (var renderer in transform.GetComponentsInChildren<MeshRenderer>())
        {
            renderer.enabled = toggleOnAreas;
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Get all NavMesh areas in the scene.
        NavMeshTriangulation navMeshData = NavMesh.CalculateTriangulation();

        // Create a material for rendering the solid planes.
        Material material = new Material(Shader.Find("Standard"));
        material.color = new Color(0.0f, 1.0f, 0.0f, 0.5f); // Green with transparency.

        // Draw the NavMesh triangles as solid planes using Gizmos.
        for (int i = 0; i < navMeshData.indices.Length; i += 3)
        {
            Vector3 vertex1 = navMeshData.vertices[navMeshData.indices[i]];
            Vector3 vertex2 = navMeshData.vertices[navMeshData.indices[i + 1]];
            Vector3 vertex3 = navMeshData.vertices[navMeshData.indices[i + 2]];

            // Calculate the triangle's normal.
            Vector3 normal = Vector3.Cross(vertex2 - vertex1, vertex3 - vertex1).normalized;

            // Create a mesh for the triangle.
            Mesh triangleMesh = new Mesh();
            triangleMesh.vertices = new Vector3[] { vertex1, vertex2, vertex3 };
            triangleMesh.normals = new Vector3[] { normal, normal, normal };
            triangleMesh.triangles = new int[] { 0, 1, 2 };

            // Draw the triangle as a solid plane using Gizmos.
            Gizmos.color = Color.green;
            Gizmos.DrawMesh(triangleMesh, Vector3.zero, Quaternion.identity, Vector3.one);
        }
    }
}
