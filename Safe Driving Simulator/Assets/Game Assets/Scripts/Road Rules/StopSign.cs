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

    void Update()
    {
        if(stopNode != null) 
        stopNode.nodeType = stop? Node.NodeType.Stop : Node.NodeType.None;
    }

    #if UNITY_EDITOR
    void OnDrawGizmos()
    {
        if(Selection.transforms.Any(x=>x.GetComponent<StopSign>() == this))
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
