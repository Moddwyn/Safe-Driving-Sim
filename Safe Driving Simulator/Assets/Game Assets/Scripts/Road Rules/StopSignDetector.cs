using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopSignDetector : MonoBehaviour
{
    [HideInInspector] public StopSignRoutine routine;
    public StopSign stopSign;

    void OnTriggerEnter(Collider other) {
        if(routine != null && (other.GetComponent<CarAIController>() != null || other.GetComponent<PlayerCar>() != null))
        {
            routine.stopSignQueue.Enqueue(stopSign);
            stopSign.stop = true;
        }
    }
}
