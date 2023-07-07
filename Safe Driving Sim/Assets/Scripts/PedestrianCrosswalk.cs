using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PedestrianCrosswalk : MonoBehaviour
{
    public Pedestrian pedestrian;

    void OnTriggerStay(Collider other)
    {
        if (other.tag == "Stop Walk" && pedestrian.currentCrosswalk)
        {
            if (!other.transform.parent.GetComponent<Crosswalk>().go)
                pedestrian.waitingToCross = true;
            else
                pedestrian.waitingToCross = false;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Stop Walk" && pedestrian.currentCrosswalk)
        {
                pedestrian.waitingToCross = false;
        }
    }
}
