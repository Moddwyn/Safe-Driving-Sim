using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficLight : MonoBehaviour
{
    public LightColor trafficColor;
    public Crosswalk[] crosswalks;
    public GameObject carStop;

    [Space(20)]
    public bool inverseAuto;
    public LightColor inverseColor;

    [Space(20)]
    public Material redGlow;
    public Material yellowGlow;
    public Material greenGlow;
    public Material offLight;
    public Material goWalk;
    public Material stopWalk;
    [Space(20)]
    public MeshRenderer red1;
    public MeshRenderer red2;
    public MeshRenderer yellow1;
    public MeshRenderer yellow2;
    public MeshRenderer green1;
    public MeshRenderer green2;
    public MeshRenderer pedestIndicator;
    public MeshRenderer pedestIndicator2;
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
        pedestIndicator2.sharedMaterial = inverseAuto? trafficColor == LightColor.Green? stopWalk : 
                                                    goWalk : inverseColor ==  LightColor.Green? goWalk : stopWalk;

        carStop.SetActive(trafficColor !=  LightColor.Green);

        foreach (var cw in crosswalks)
        {
            cw.go = trafficColor == LightColor.Green;
        }
    }

    public void Green() => trafficColor = LightColor.Green;
    public void Yellow() => trafficColor = LightColor.Yellow;
    public void Red() => trafficColor = LightColor.Red;
}
