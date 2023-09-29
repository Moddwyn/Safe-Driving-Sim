using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEditor;
using UnityEngine;

public class TrafficLight : MonoBehaviour
{
    public LightColor trafficColor;
    public Node stopNode;
    public Transform stopNodeIndicatorPoint;
    public Transform stopNodeChecker;
    public float stopNodeCheckerRadius = 3;

    [HorizontalLine]
    public Material redGlow;
    public Material yellowGlow;
    public Material greenGlow;
    public Material offLight;
    public Material goWalk;
    public Material stopWalk;
    [HorizontalLine]
    public MeshRenderer red1;
    public MeshRenderer red2;
    public MeshRenderer yellow1;
    public MeshRenderer yellow2;
    public MeshRenderer green1;
    public MeshRenderer green2;
    public MeshRenderer pedestIndicator;
    public enum LightColor { Red, Yellow, Green };

    void Update()
    {
        red1.sharedMaterial = trafficColor == LightColor.Red ? redGlow : offLight;
        red2.sharedMaterial = trafficColor == LightColor.Red ? redGlow : offLight;
        yellow1.sharedMaterial = trafficColor == LightColor.Yellow ? yellowGlow : offLight;
        yellow2.sharedMaterial = trafficColor == LightColor.Yellow ? yellowGlow : offLight;
        green1.sharedMaterial = trafficColor == LightColor.Green ? greenGlow : offLight;
        green2.sharedMaterial = trafficColor == LightColor.Green ? greenGlow : offLight;
        pedestIndicator.sharedMaterial = trafficColor == LightColor.Green ? goWalk : stopWalk;

        if (stopNode != null)
        {
            stopNode.nodeType = trafficColor == LightColor.Red ? Node.NodeType.Stop : Node.NodeType.None;
            stopNode.stopType = trafficColor == LightColor.Red ? Node.StopType.Violation : Node.StopType.None;
        }
    }

    public void Green() => trafficColor = LightColor.Green;
    public void Yellow() => trafficColor = LightColor.Yellow;
    public void Red() => trafficColor = LightColor.Red;

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
        if (Selection.transforms.Any(x => x.GetComponent<TrafficLight>() == this
        || (x.GetComponent<TrafficLightRoutine>() != null && x.GetComponent<TrafficLightRoutine>().transform == transform.parent)))
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
