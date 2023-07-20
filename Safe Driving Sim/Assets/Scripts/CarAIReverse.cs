using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarAIReverse : MonoBehaviour
{
    public CarAI car;
    private void OnTriggerStay(Collider other)
    {
        if (other.transform.CompareTag("Collision") && other.transform.root != transform) {
            car.speed = -1;
            car.reverse = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.CompareTag("Collision") && other.transform.root != transform)
        {
            car.speed = car.currentSpeed;
            car.reverse = false;
        }
    }
}
