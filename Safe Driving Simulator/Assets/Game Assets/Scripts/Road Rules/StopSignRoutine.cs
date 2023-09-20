using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;

public class StopSignRoutine : MonoBehaviour
{
    public List<StopSign> stopSigns = new List<StopSign>();
    public Queue<StopSign> stopSignQueue = new Queue<StopSign>();
    [ReadOnly] public List<StopSign> queueList = new List<StopSign>();

    [HorizontalLine]
    public BoxCollider carCheckBounds;
    public float durationOnSwitch = 2;
    public float durationOnGo = 2;

    void Awake() {
        foreach (var detector in GetComponentsInChildren<StopSignDetector>())
        {
            detector.routine = this;
        }
    }

    void Start()
    {
        StartCoroutine(StartPattern());
    }

    void Update() {
        queueList = stopSignQueue.ToList();
    }

    IEnumerator StartPattern()
    {
        while (true)
        {
            if(stopSignQueue.Count > 0)
            {
                Collider[] colliders = null;
                bool carInIntersection = true;

                while(carInIntersection)
                {
                    colliders = Physics.OverlapBox(carCheckBounds.bounds.center, carCheckBounds.bounds.extents, Quaternion.identity);
                    carInIntersection = colliders.Any(collider => collider.GetComponent<CarAIController>() != null);
                    yield return null;
                }

                yield return new WaitForSeconds(durationOnSwitch);
                StopSign lastGreenStop = null;

                StopSign nextGreen = stopSignQueue.Dequeue();
                foreach (var sign in stopSigns)
                {
                    if(sign == nextGreen)
                    {
                        lastGreenStop = sign;
                        sign.stop = false;
                    }
                    else
                    {
                        sign.stop = true;
                    }
                }

                yield return new WaitForSeconds(durationOnGo);
                lastGreenStop.stop = true;
            }

            yield return null;
        }
        
    }

    [Button("Auto Add Stop Signs")]
    public void FillStopSigns()
    {
        stopSigns.Clear();
        stopSigns = GetComponentsInChildren<StopSign>().ToList();
    }
}
