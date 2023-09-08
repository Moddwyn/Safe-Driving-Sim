using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopEntryCheck : MonoBehaviour
{
    public CarController carController;
    void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent<CarController>(out CarController player))
        {
            carController = player;
            player.waitingStopped = true;
            player.allowPass = false;
        }
    }
    void OnTriggerStay(Collider other)
    {
        if(other.TryGetComponent<CarController>(out CarController player))
        {
            if(player.waitingStopped && player.currentSpeed <= 0.1f)
            {
                player.allowPass = true;
            }
        }
    }
}
