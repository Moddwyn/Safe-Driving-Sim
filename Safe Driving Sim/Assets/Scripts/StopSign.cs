using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopSign : MonoBehaviour
{
    public StopEntryCheck stopEntryCheck;
    void Update()
    {
        if (stopEntryCheck.carController != null)
        {
            if (stopEntryCheck.carController.waitingStopped && stopEntryCheck.carController.allowPass)
            {
                GetComponent<Collider>().enabled = false;
            } else 
            {
                GetComponent<Collider>().enabled = true;
            }
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<CarController>(out CarController player))
        {
            player.waitingStopped = false;
            player.allowPass = false;
            stopEntryCheck.carController = null;
        }
    }
}
