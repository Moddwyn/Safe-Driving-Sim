using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    [HideInInspector] public float startTime;

    private void Awake()
    {
        startTime = Time.time;
    }


}
