using System.Collections.Generic;
using UnityEngine;

public class Route : MonoBehaviour
{
    public int id;
    public int maxMPH = 12;
    public Color defaultLineColor = Color.yellow;
    public bool isIntersection;

    public List<CarAIController> cars = new List<CarAIController>();

    public bool connectNodes = true;
    public List<Node> nodes = new List<Node>();

    void OnValidate()
    {
        GenerateRandomID();
    }

    void Start()
    {
        GenerateRandomID();
        SpawnCars();

        isIntersection = GetComponent<IntersectionConnector>() != null;
        foreach (var node in nodes)
        {
            node.isStartNode = GetStartNode() == node;
            node.isEndNode = GetEndNode() == node;
            node.isIntersection = isIntersection;
        }
    }

    void SpawnCars()
    {
        for (int i = 0; i < cars.Count; i++)
        {
            CarAIController newCar = Instantiate(cars[i], nodes[i].transform.position, nodes[i].transform.rotation);
        }
    }

    public void GenerateRandomID() => id = Random.Range(0, int.MaxValue);
    public Node GetStartNode() => nodes.Count > 0 ? nodes[0] : null;
    public Node GetEndNode() => nodes.Count > 0 ? nodes[nodes.Count - 1] : null;
    public IntersectionConnector GetExistingIntersection() => GetComponent<IntersectionConnector>();

    #if UNITY_EDITOR
    void OnDrawGizmos() {
        if (cars.Count == 0) return;
        for (int i = 0; i < cars.Count; i++)
        {
            Gizmos.DrawIcon(nodes[i].transform.position + (Vector3.up * 2), "car.png", true);
        }
    }
    #endif
}
