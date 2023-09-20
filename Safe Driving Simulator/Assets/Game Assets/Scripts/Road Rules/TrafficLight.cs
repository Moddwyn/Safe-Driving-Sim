using System.Collections;
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
        red1.sharedMaterial = trafficColor == LightColor.Red? redGlow : offLight;
        red2.sharedMaterial = trafficColor == LightColor.Red? redGlow : offLight;
        yellow1.sharedMaterial = trafficColor == LightColor.Yellow? yellowGlow : offLight;
        yellow2.sharedMaterial = trafficColor == LightColor.Yellow? yellowGlow : offLight;
        green1.sharedMaterial = trafficColor == LightColor.Green? greenGlow : offLight;
        green2.sharedMaterial = trafficColor == LightColor.Green? greenGlow : offLight;
        pedestIndicator.sharedMaterial = trafficColor == LightColor.Green? goWalk : stopWalk;
        
        if(stopNode != null) 
        stopNode.nodeType = trafficColor == LightColor.Red? Node.NodeType.Stop : Node.NodeType.None;
    }

    public void Green() => trafficColor = LightColor.Green;
    public void Yellow() => trafficColor = LightColor.Yellow;
    public void Red() => trafficColor = LightColor.Red;

    #if UNITY_EDITOR
    void OnDrawGizmos()
    {
        if(Selection.transforms.Any(x=>x.GetComponent<TrafficLight>() == this 
        || (x.GetComponent<TrafficLightRoutine>() != null && x.GetComponent<TrafficLightRoutine>().transform == transform.parent)))
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
