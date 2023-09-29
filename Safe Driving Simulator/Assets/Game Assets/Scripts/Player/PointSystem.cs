using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class PointSystem : MonoBehaviour
{
    public UnityEvent OnFail;
    public int points = 100;
    [HorizontalLine]
    public int stoppedViolation = 10;

    [HorizontalLine]
    public PlayerCarUI carUI;

    void Start()
    {
        carUI.StartResumeTimer();
    }

    void Update()
    {
        if (points <= 0)
        {
            carUI.cause = "Too many unsafe maneuvers";
            OnFail?.Invoke();
        }
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.transform.CompareTag("Collision") && other.transform != transform)
        {
            if (other.transform.root.GetComponent<NavMeshAgent>()) carUI.cause = "Hit a pedestrian";
            else if (other.transform.root.GetComponent<CarAIController>()) carUI.cause = "Hit/Been hit by another car";
            else carUI.cause = "Hit an object";
            OnFail?.Invoke();
        }
    }

    void OnTriggerEnter(Collider other) {
        if (other.transform.root.TryGetComponent<StopSignRoutine>(out StopSignRoutine routine))
        {
            if(routine.carInIntersection)
            {
                if(routine.collidersInIntersection.Length > 1 && routine.collidersInIntersection.ToList().Contains(GetComponent<Collider>()))
                {
                    points -= stoppedViolation;
                }
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.transform.TryGetComponent<Node>(out Node node))
        {
            if (node.nodeType == Node.NodeType.Stop && node.stopType == Node.StopType.Violation)
            {
                points -= stoppedViolation;
            }
        }
    }
}
