using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PedestrianCrosswalk : MonoBehaviour
{
    public Pedestrian pedestrian;
    public bool onCrosswalk;
    public bool greenLight;
    void Update()
    {
        pedestrian.isWaiting = onCrosswalk && !greenLight;
    }

    void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<Crosswalk>())
        {
            onCrosswalk = true;
            if(other.GetComponent<Crosswalk>().go) greenLight = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Crosswalk>())
        {
            onCrosswalk = false;
            greenLight = false;
        }
    }
}
