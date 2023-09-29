using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEditor;
using UnityEngine;

public class YieldSigns : MonoBehaviour
{
    public Node stopNode;
    public BoxCollider checkCollider;
    public Transform stopNodeIndicatorPoint;
    [ReadOnly] public bool areCarsPassing;

    [HorizontalLine]
    public Transform stopNodeChecker;
    public float stopNodeCheckerRadius = 3;

    void Update() {
        Collider[] checkColliders = Physics.OverlapBox(
            checkCollider.bounds.center,
            checkCollider.bounds.size / 2,
            Quaternion.identity).Where(x=>x.GetComponent<CarAIController>() != null || x.GetComponent<PlayerCar>() != null).ToArray();

        areCarsPassing = checkColliders.Length > 0;
        stopNode.nodeType = areCarsPassing? Node.NodeType.Stop : Node.NodeType.None;
    }

    public void AutoFindStopNode()
    {
        Collider[] colliders = Physics.OverlapSphere(stopNodeChecker.position, stopNodeCheckerRadius, Physics.AllLayers);
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
            float distance = Vector3.Distance(stopNodeChecker.position, node.transform.position);
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
        if (Selection.transforms.Any(x => x.GetComponent<YieldSigns>() == this))
        {
            Gizmos.color = Color.yellow;
            if (stopNode != null)
                Gizmos.DrawLine(stopNodeIndicatorPoint.position, stopNode.transform.position);
            else
            {
                if(stopNodeChecker != null) Gizmos.DrawWireSphere(stopNodeChecker.position, stopNodeCheckerRadius);
            }
                
        }
    }
    #endif
}
