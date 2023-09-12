using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class Node : MonoBehaviour
{
    public Route route;
    public LayerMask carMask;

    public bool hasTraffic;
    public bool hasCar;
    public bool isIntersection;
    public bool isStartNode;
    public bool isEndNode;

    public List<Node> connectedNodes = new List<Node>();
    public List<CarAIController> carsInRadius;

    public NodeType nodeType;

    RoadManager manager;
    Color nodeColor = Color.white;

    public enum NodeType
    {
        None, Stop
    }

    void Awake()
    {
        RotateTowardsConnectedNodes();
        DeleteEmptyNodes();
    }

    void Start()
    {
        manager = RoadManager.Instance;
    }

    void Update()
    {
        CheckCarOnNode();
    }

    void CheckCarOnNode()
    {
        hasCar = Physics.OverlapSphere(transform.position, manager.carCheckRad, carMask)
                        .Any(x => x.GetComponent<CarAIController>() != null);
        carsInRadius = Physics.OverlapSphere(transform.position, manager.trafficCheckRad, carMask)
                        .Where(x => x.GetComponent<CarAIController>() != null)
                        .Select(x => x.GetComponent<CarAIController>())
                        .ToList();
        hasTraffic = carsInRadius.Count > 0 && carsInRadius.Any(x=>x.IsStoppedOrStopping()) && hasCar &&
                    carsInRadius.All(x=>x.nodePath.Contains(this));
    }
    

    public void AddNewNode(Node node)
    {
        if (!connectedNodes.Contains(node) || node != this)
        {
            connectedNodes.Add(node);
        }
    }

    public void RemoveNode(Node node) => connectedNodes.Remove(node);

    void RotateTowardsConnectedNodes()
    {
        if (connectedNodes.Count == 0)
            return;

        Vector3 targetDirection = Vector3.zero;

        if (connectedNodes.Count == 1)
        {
            targetDirection = connectedNodes[0].transform.position - transform.position;
        }
        else
        {
            Vector3 averageDirection = Vector3.zero;
            foreach (Node connectedNode in connectedNodes)
            {
                averageDirection += connectedNode.transform.position - transform.position;
            }
            targetDirection = averageDirection.normalized;
        }

        float targetAngle = Mathf.Atan2(targetDirection.x, targetDirection.z) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(0f, targetAngle, 0f);
        transform.rotation = targetRotation;
    }

    public void DeleteEmptyNodes() => connectedNodes.RemoveAll(x => x == null);

    public int GetRouteID() => route.id;

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        nodeColor = Selection.transforms.Any(x=>x.GetComponent<Route>() != null && x.GetComponent<Route>() == route)? Color.green : Color.white;
        nodeColor = nodeType == NodeType.Stop? Color.red : nodeColor;
        if (Selection.transforms.Any(x=>x.GetComponent<Route>() != null 
        || x.GetComponent<Node>() != null 
        || x.GetComponent<PathFinder>() != null 
        || x.GetComponent<RoadManager>() != null
        || x.GetComponent<TrafficLight>() != null
        || x.GetComponent<StopSign>() != null
        || x.GetComponent<YieldSigns>() != null))
        {
            Gizmos.color = nodeColor;
            Gizmos.DrawSphere(transform.position, 0.5f);

            if (connectedNodes.Count == 0) return;
            foreach (var connected in connectedNodes)
            {
                if (connected == null) continue;

                Color lineColor = connected.GetRouteID() == GetRouteID() ? route.defaultLineColor : Color.white;

                Gizmos.color = lineColor;
                Gizmos.DrawLine(transform.position, connected.transform.position);
                //GizmoDrawLine(transform.position, connected.transform.position, lineColor, "", Color.white);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if (manager == null) return;
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, manager.trafficCheckRad);
    }

    void GizmoDrawLine(Vector3 from, Vector3 to, Color lineColor, string text, Color textColor)
    {
        Handles.color = Selection.transforms.Any(x=>x.GetComponent<Route>() != null && x.GetComponent<Route>() == route)? Color.green : lineColor;
        Handles.DrawAAPolyLine(5f, from, to);

        Vector3 dir = (to - from).normalized;
        float distance = Vector3.Distance(from, to);

        for (float i = 0; i < distance; i += 1f)
        {
            Handles.DrawAAPolyLine(
                5f,
                from + dir * i,
                from + (dir * (i - .15f)) + Quaternion.AngleAxis(Time.realtimeSinceStartup * 360f, dir.normalized * 300f) * Vector3.up * .05f
            );
            Handles.DrawAAPolyLine(
                5f,
                from + dir * i,
                from + (dir * (i - .15f)) + Quaternion.AngleAxis(Time.realtimeSinceStartup * 360f + 180, dir.normalized * 300f) * Vector3.up * .05f
            );
        }

        GUIStyle style = new GUIStyle();
        style.alignment = TextAnchor.MiddleCenter;
        style.normal.textColor = textColor;
        Handles.Label(from + (dir * distance * .5f), text, style);
    }
#endif
}
