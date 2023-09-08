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

    void Update() {
        Collider[] checkColliders = Physics.OverlapBox(
            checkCollider.bounds.center,
            checkCollider.bounds.size / 2,
            Quaternion.identity).Where(x=>x.GetComponent<CarAIController>() != null || x.GetComponent<PlayerCar>() != null).ToArray();

        areCarsPassing = checkColliders.Length > 0;
        stopNode.nodeType = areCarsPassing? Node.NodeType.Stop : Node.NodeType.None;
    }

    #if UNITY_EDITOR
    void OnDrawGizmos()
    {
        if(Selection.transforms.Any(x=>x.GetComponent<YieldSigns>() == this))
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
