using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopSignDetector : MonoBehaviour
{
    [HideInInspector] public StopSignRoutine routine;
    public StopSign stopSign;

    public List<GameObject> enteredColliders = new List<GameObject>();

    void OnTriggerEnter(Collider other)
    {
        if (routine != null && (other.GetComponent<CarAIController>() != null || other.GetComponent<PlayerCar>() != null))
        {
            enteredColliders.Add(other.gameObject);
            routine.stopSignQueue.Enqueue(stopSign);
            stopSign.stop = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (routine != null && (other.GetComponent<CarAIController>() != null || other.GetComponent<PlayerCar>() != null))
        {
            enteredColliders.Remove(other.gameObject);
        }
    }
}
