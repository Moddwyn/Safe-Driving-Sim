using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarDistCheck : MonoBehaviour
{
    public CarAI car;
    void OnTriggerStay(Collider other)
    {
        if (other.transform.CompareTag("Collision") && other.transform.root != transform && !car.reverse) {
            car.speed = 0;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.transform.CompareTag("Collision") && other.transform.root != transform && !car.reverse) {
            car.speed = car.s;
        }
    }
}
