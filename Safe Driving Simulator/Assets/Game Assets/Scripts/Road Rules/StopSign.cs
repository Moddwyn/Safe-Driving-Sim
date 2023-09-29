using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEditor;
using UnityEngine;

public class StopSign : MonoBehaviour
{
    public bool stop;
    public Node stopNode;
    public Transform stopNodeIndicatorPoint;
    public float autoFindRadius = 3;

    void Update()
    {
        if(stopNode != null) 
        stopNode.nodeType = stop? Node.NodeType.Stop : Node.NodeType.None;
    }

    public void AutoFindStopNode()
    {
        Vector3 checkPosition = (transform.position + GetComponentInChildren<StopSignDetector>().transform.position) / 2;

        Collider[] colliders = Physics.OverlapSphere(checkPosition, autoFindRadius, Physics.AllLayers);
        List<Node> nodes = new List<Node>();

        foreach (var collider in colliders)
        {
            Node node = collider.GetComponent<Node>();
            if (node != null && node.connectedNodes.Count > 0)
            {
                nodes.Add(node);
            }
        }

        Node nearestNode = null;
        float shortestDistance = Mathf.Infinity;

        foreach (Node node in nodes)
        {
            float distance = Vector3.Distance(checkPosition, node.transform.position);
            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                nearestNode = node;
            }
        }

        if(nearestNode != null) stopNode = nearestNode;
    }

    #if UNITY_EDITOR
    void OnDrawGizmos()
    {
        if(Selection.transforms.Any(x=>x.GetComponent<StopSign>() == this ||
        (x.GetComponent<StopSignRoutine>() != null && x.GetComponent<StopSignRoutine>().transform == transform.parent)))
        {
            if(stopNode != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(stopNodeIndicatorPoint.position, stopNode.transform.position);
            }
        }
    }
    #endif
}
