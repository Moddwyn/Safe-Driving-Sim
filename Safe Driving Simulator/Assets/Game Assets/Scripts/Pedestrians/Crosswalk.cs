using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;

public class Crosswalk : MonoBehaviour
{
    public List<TrafficLight> greenLightCheck = new List<TrafficLight>();
    [ReadOnly] public bool allowCross;

    void Update() {
        allowCross = greenLightCheck.Any(x=>x.trafficColor == TrafficLight.LightColor.Green);
    }

    void OnTriggerStay(Collider other)
    {
        if(other.transform.root.TryGetComponent<Pedestrian>(out Pedestrian pedestrian))
        {
            pedestrian.isWaiting = !allowCross;
        }
    }

    void OnTriggerExit(Collider other) {
        if(other.transform.root.TryGetComponent<Pedestrian>(out Pedestrian pedestrian))
        {
            pedestrian.isWaiting = false;
        }
    }
}
