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

    [HorizontalLine]
    [ReadOnly] public bool carInIntersection;
    [ReadOnly] public Collider[] collidersInIntersection;

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

        UpdateCollidersInIntersection();
    }

    public void UpdateCollidersInIntersection()
    {
        collidersInIntersection = Physics.OverlapBox(carCheckBounds.bounds.center, carCheckBounds.bounds.extents, Quaternion.identity);
        collidersInIntersection = collidersInIntersection.Where(cars => cars.GetComponent<CarAIController>() != null || cars.GetComponent<PlayerCar>() != null).ToArray();
        if(collidersInIntersection != null)
                        carInIntersection = collidersInIntersection.Any(collider => collider.GetComponent<CarAIController>() != null || collider.GetComponent<PlayerCar>() != null);
    }

    [Button("Auto Connect All Stop Nodes")]
    public void AutoConnectStopNodes()
    {
        foreach (var sign in GetComponentsInChildren<StopSign>())
        {
            sign.AutoFindStopNode();
        }
    }

    IEnumerator StartPattern()
    {
        while (true)
        {
            if(stopSignQueue.Count > 0)
            {
                collidersInIntersection = null;

                while(carInIntersection)
                {
                    yield return null;
                }

                yield return new WaitForSeconds(durationOnSwitch);
                StopSign lastGreenStop = null;
                
                List<GameObject> enteredColliders = stopSignQueue.Peek().GetComponentInChildren<StopSignDetector>().enteredColliders;
                while(enteredColliders.Count > 0 && enteredColliders[0] == PlayerCar.Instance.gameObject)
                {
                    yield return null;
                }

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
