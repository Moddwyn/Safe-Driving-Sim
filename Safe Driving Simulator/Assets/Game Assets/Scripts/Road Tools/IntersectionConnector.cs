using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using System.Linq;

public class IntersectionConnector : MonoBehaviour
{
    public Route route;
    public IntersectionType intersectionType;
    public enum IntersectionType { Four, Three, Two };

    [HorizontalLine]
    public Node abNode;
    public Node cbNode;
    public Node adNode;
    public Node cdNode;
    public Node centerNodeAB;
    public Node centerNodeCB;
    public Node centerNodeCD;
    public Node centerNodeAD;
    [HorizontalLine]
    public Node aPairHelper;
    public Node bPairHelper;
    public Node cPairHelper;
    public Node dPairHelper;
    [HorizontalLine]
    public float searchRadius = 10;
    public PairNodes aPair;
    public PairNodes bPair;
    public PairNodes cPair;
    public PairNodes dPair;

    public void OnValidate()
    {
        if (intersectionType == IntersectionType.Four)
        {
            adNode.gameObject.SetActive(true);
            cdNode.gameObject.SetActive(true);
            cbNode.gameObject.SetActive(true);
            
            cPairHelper.gameObject.SetActive(true);
            dPairHelper.gameObject.SetActive(true);
        }
        else if(intersectionType == IntersectionType.Three)
        {
            adNode.gameObject.SetActive(false);
            cdNode.gameObject.SetActive(false);
            cbNode.gameObject.SetActive(true);

            cPairHelper.gameObject.SetActive(true);
            dPairHelper.gameObject.SetActive(false);
        } 
        else
        {
            cdNode.gameObject.SetActive(false);
            adNode.gameObject.SetActive(false);
            cbNode.gameObject.SetActive(false);
            cPairHelper.gameObject.SetActive(false);
            dPairHelper.gameObject.SetActive(false);
        }
    }

    [Button("Auto Find Pairs")]
    public void AutoFindPairs()
    {
        ResetConnections();
        ResetPairs();
        if (intersectionType == IntersectionType.Four)
        {
            AutoFindPairLetter("a");
            AutoFindPairLetter("b");
            AutoFindPairLetter("c");
            AutoFindPairLetter("d");
        }
        else if (intersectionType == IntersectionType.Three)
        {
            AutoFindPairLetter("a");
            AutoFindPairLetter("b");
            AutoFindPairLetter("c");
        } else if(intersectionType == IntersectionType.Two)
        {
            AutoFindPairLetter("a");
            AutoFindPairLetter("b");
        }
    }

    [Button("Auto Connect")]
    public void ConnectAll()
    {
        ResetConnections();
        AutoFindPairs();

        if (intersectionType == IntersectionType.Four)
        {
            aPair.endNode.AddNewNode(adNode);
            aPair.endNode.AddNewNode(cPair.startNode);
            aPair.endNode.AddNewNode(centerNodeAB);

            bPair.endNode.AddNewNode(abNode);
            bPair.endNode.AddNewNode(dPair.startNode);
            bPair.endNode.AddNewNode(centerNodeCB);

            cPair.endNode.AddNewNode(cbNode);
            cPair.endNode.AddNewNode(aPair.startNode);
            cPair.endNode.AddNewNode(centerNodeCD);

            dPair.endNode.AddNewNode(centerNodeAD);
            dPair.endNode.AddNewNode(bPair.startNode);
            dPair.endNode.AddNewNode(cdNode);

            abNode.AddNewNode(aPair.startNode);
            adNode.AddNewNode(dPair.startNode);
            cbNode.AddNewNode(bPair.startNode);
            cdNode.AddNewNode(cPair.startNode);
            
            centerNodeAD.AddNewNode(aPair.startNode);
            centerNodeAB.AddNewNode(bPair.startNode);
            centerNodeCB.AddNewNode(cPair.startNode);
            centerNodeCD.AddNewNode(dPair.startNode);
        }
        else if(intersectionType == IntersectionType.Three)
        {
            aPair.endNode.AddNewNode(cPair.startNode);
            aPair.endNode.AddNewNode(centerNodeAB);

            bPair.endNode.AddNewNode(abNode);
            bPair.endNode.AddNewNode(centerNodeCB);

            cPair.endNode.AddNewNode(cbNode);
            cPair.endNode.AddNewNode(aPair.startNode);

            abNode.AddNewNode(aPair.startNode);
            cbNode.AddNewNode(bPair.startNode);
            cdNode.AddNewNode(cPair.startNode);
            
            centerNodeAB.AddNewNode(bPair.startNode);
            centerNodeCB.AddNewNode(cPair.startNode);
        }
        else
        {
            aPair.endNode.AddNewNode(centerNodeAB);
            bPair.endNode.AddNewNode(abNode);
            
            abNode.AddNewNode(aPair.startNode);
            centerNodeAB.AddNewNode(bPair.startNode);
        }
    }

    [Button("Reset Connections")]
    public void ResetConnections()
    {
        if(aPair.endNode == null && bPair.endNode == null & cPair.endNode == null) return;
        abNode.connectedNodes.Clear();
        cbNode.connectedNodes.Clear();
        adNode.connectedNodes.Clear();
        cdNode.connectedNodes.Clear();
        centerNodeAB.connectedNodes.Clear();
        centerNodeCB.connectedNodes.Clear();
        centerNodeCD.connectedNodes.Clear();
        centerNodeAD.connectedNodes.Clear();

        aPair.endNode.connectedNodes.Clear();
        bPair.endNode.connectedNodes.Clear();

        if(intersectionType != IntersectionType.Two)
            cPair.endNode.connectedNodes.Clear();
        
        if (intersectionType == IntersectionType.Four && aPair.endNode != null)
        {
            dPair.endNode.connectedNodes.Clear();
        }
    }

    public PairNodes GetPairNodesWithEndNode(Node endNode)
    {
        if(aPair.endNode == endNode) return aPair;
        if(bPair.endNode == endNode) return bPair;
        if(cPair.endNode == endNode) return cPair;
        if(dPair.endNode == endNode) return dPair;
        return null;
    }
    
    [Button("Reset Pairs")]
    public void ResetPairs()
    {
        ResetConnections();
        aPair = new PairNodes();
        bPair = new PairNodes();
        cPair = new PairNodes();
        dPair = new PairNodes();
    }

    public void AutoFindPairLetter(string letter)
    {
        if (letter == "a")
        {
            aPair.startNode = GetNearestNode(FindNodesWithinRadius(aPairHelper).Where(x => x.connectedNodes.Select(x=>x != null).ToList().Count > 0).ToList(), aPairHelper.transform);
            aPair.endNode = GetNearestNode(FindNodesWithinRadius(aPairHelper).Where(x => x.connectedNodes.Select(x=>x != null).ToList().Count == 0).ToList(), aPairHelper.transform);
        }
        else if (letter == "b")
        {
            bPair.startNode = GetNearestNode(FindNodesWithinRadius(bPairHelper).Where(x => x.connectedNodes.Select(x=>x != null).ToList().Count > 0).ToList(), bPairHelper.transform);
            bPair.endNode = GetNearestNode(FindNodesWithinRadius(bPairHelper).Where(x => x.connectedNodes.Select(x=>x != null).ToList().Count == 0).ToList(), bPairHelper.transform);
        }
        else if (letter == "c")
        {
            cPair.startNode = GetNearestNode(FindNodesWithinRadius(cPairHelper).Where(x => x.connectedNodes.Select(x=>x != null).ToList().Count > 0).ToList(), cPairHelper.transform);
            cPair.endNode = GetNearestNode(FindNodesWithinRadius(cPairHelper).Where(x => x.connectedNodes.Select(x=>x != null).ToList().Count == 0).ToList(), cPairHelper.transform);
        }
        else
        {
            dPair.startNode = GetNearestNode(FindNodesWithinRadius(dPairHelper).Where(x => x.connectedNodes.Select(x=>x != null).ToList().Count > 0).ToList(), dPairHelper.transform);
            dPair.endNode = GetNearestNode(FindNodesWithinRadius(dPairHelper).Where(x => x.connectedNodes.Select(x=>x != null).ToList().Count == 0).ToList(), dPairHelper.transform);
        }

    }

    List<Node> FindNodesWithinRadius(Node center)
    {
        Collider[] colliders = Physics.OverlapSphere(center.transform.position, searchRadius, Physics.AllLayers);
        List<Node> nodes = new List<Node>();

        foreach (var collider in colliders)
        {
            Node node = collider.GetComponent<Node>();
            if (node != null && !route.nodes.Contains(node))
            {
                nodes.Add(node);
            }
        }

        return nodes;
    }

    Node GetNearestNode(List<Node> nodes, Transform center)
    {
        Node nearestNode = null;
        float shortestDistance = Mathf.Infinity;

        foreach (Node node in nodes)
        {
            float distance = Vector3.Distance(center.position, node.transform.position);
            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                nearestNode = node;
            }
        }

        return nearestNode;
    }

    Vector3 GetCenterPair(PairNodes pair, float yOffset)
    {
        Vector3 centerPosition = (pair.startNode.transform.position + pair.endNode.transform.position) * 0.5f;
        centerPosition.y += yOffset;
        return centerPosition;
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (aPair.startNode != null && aPair.endNode != null) Gizmos.DrawIcon(GetCenterPair(aPair, 2), "letter-a.png", true);
        if (bPair.startNode != null && bPair.endNode != null) Gizmos.DrawIcon(GetCenterPair(bPair, 2), "letter-b.png", true);
        if (cPair.startNode != null && cPair.endNode != null) Gizmos.DrawIcon(GetCenterPair(cPair, 2), "letter-c.png", true);
        if (dPair.startNode != null && dPair.endNode != null) Gizmos.DrawIcon(GetCenterPair(dPair, 2), "letter-d.png", true);
    }
#endif

    [System.Serializable]
    public class PairNodes
    {
        public Node startNode;
        public Node endNode;
    }
}
