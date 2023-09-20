using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using NaughtyAttributes;
using UnityEditor;

public class RoadManager : MonoBehaviour
{
    public List<Node> allNodes = new List<Node>();

    [HorizontalLine]
    public List<Node> waypointNodes = new List<Node>();
    
    [HorizontalLine]
    [InfoBox("If the next node is new route, and if there's a car within this radius, it will not move till it's clear")]
    public float trafficCheckRad = 5;
    [InfoBox("The radius to check if a car is on a node")]
    public float carCheckRad = 2;

    public static RoadManager Instance;

    void Awake() {
        Instance = this;
    }

    void OnEnable()
    {
        RefreshNodes();
    }

    [Button("Refresh")]
    public void RefreshNodes()
    {
        allNodes = FindObjectsOfType<Node>().ToList();
    }

    public Node GetRandomWaypoint(Node excluding)
    {
        List<Node> availableWaypoints = waypointNodes.Where(node => node != excluding).ToList();

        if (availableWaypoints.Count == 0)
        {
            Debug.LogWarning("No available waypoints to select from.");
            return null;
        }

        int randomIndex = Random.Range(0, availableWaypoints.Count);
        return availableWaypoints[randomIndex];
    }

    #if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (waypointNodes.Count == 0) return;
        foreach (var waypoint in waypointNodes)
        {
            if(waypoint != null)
            Gizmos.DrawIcon(waypoint.transform.position + (Vector3.up * 2), "Waypoint.png", true);
        }
    }
    #endif
}
