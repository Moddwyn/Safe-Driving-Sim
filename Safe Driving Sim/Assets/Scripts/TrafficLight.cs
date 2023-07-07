using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficLight : MonoBehaviour
{
    public LightColor trafficColor;

    [Space(20)]
    public Material redGlow;
    public Material yellowGlow;
    public Material greenGlow;
    public Material offLight;
    [Space(20)]
    public MeshRenderer red1;
    public MeshRenderer red2;
    public MeshRenderer yellow1;
    public MeshRenderer yellow2;
    public MeshRenderer green1;
    public MeshRenderer green2;
    public enum LightColor { Red, Yellow, Green }; 

    void Update()
    {
        red1.sharedMaterial = trafficColor == LightColor.Red? redGlow : offLight;
        red2.sharedMaterial = trafficColor == LightColor.Red? redGlow : offLight;
        yellow1.sharedMaterial = trafficColor == LightColor.Yellow? yellowGlow : offLight;
        yellow2.sharedMaterial = trafficColor == LightColor.Yellow? yellowGlow : offLight;
        green1.sharedMaterial = trafficColor == LightColor.Green? greenGlow : offLight;
        green2.sharedMaterial = trafficColor == LightColor.Green? greenGlow : offLight;
    }
}
