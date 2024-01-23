using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Barmetler.RoadSystem;
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
    public int outOfLaneViolation = 1;
    [ReadOnly] public bool canGoThroughStop;
    [ReadOnly] public bool outOfLane;

    [HorizontalLine]
    public PlayerCarUI carUI;

    [HorizontalLine]
    [ReadOnly] public Road currentRoad;

    private ScenerioManager scenerioManager;

    public static PointSystem Instance;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        carUI.StartResumeTimer();
        scenerioManager = FindObjectOfType<ScenerioManager>();

        StartCoroutine(CheckOutOfLane());
    }

    void Update()
    {
        if (points <= 0)
        {
            carUI.cause = "Too many unsafe maneuvers";
            OnFail?.Invoke();
        }

        CheckRoad();
    }

    void CheckRoad()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, 2).Where(x => x.GetComponent<Road>() != null).ToArray();
        if (colliders.Length == 0)
        {
            outOfLane = false;
            return;
        }
        currentRoad = colliders[0].GetComponent<Road>();

        var points = currentRoad.GetEvenlySpacedPoints(1, 1).Select(e => e.ToWorldSpace(currentRoad.transform)).ToArray();
        Barmetler.Bezier.OrientedPoint closestPoint = points.OrderBy(p => Vector3.Distance(p.position, transform.position)).First();
        float distanceToPoint = Vector3.Distance(closestPoint.position, transform.position);
        outOfLane = distanceToPoint <= 0.7f;
    }

    IEnumerator CheckOutOfLane()
    {
        yield return new WaitForSeconds(1);
        if(outOfLane) points -= outOfLaneViolation;
        StartCoroutine(CheckOutOfLane());
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

    void OnTriggerEnter(Collider other)
    {
        if (other.transform.parent.TryGetComponent<StopSignRoutine>(out StopSignRoutine routine))
        {
            if (!canGoThroughStop) points -= stoppedViolation;
            if (routine.carInIntersection)
            {
                if (routine.collidersInIntersection.Length > 1)
                {
                    routine.UpdateCollidersInIntersection();
                    if (routine.collidersInIntersection.ToList().Contains(GetComponent<Collider>()))
                    {
                        points -= stoppedViolation;
                    }
                }
            }
        }

        if (other.TryGetComponent<StopSignDetector>(out StopSignDetector detector))
        {
            StartCoroutine(CheckForCarStop(detector));
        }

        if (!scenerioManager.hasReachedEnd && scenerioManager.endNode != null && other.transform.GetComponent<Node>() == scenerioManager.endNode)
        {
            scenerioManager.hasReachedEnd = true;
            scenerioManager.OnReachEnd?.Invoke();
        }
    }
    IEnumerator CheckForCarStop(StopSignDetector detector)
    {
        yield return new WaitForSeconds(1);
        while (PlayerCar.Instance.currentSpeed > 0.05f && detector.enteredColliders.Contains(PlayerCar.Instance.gameObject))
        {
            yield return null;
        }

        if (detector.enteredColliders.Contains(PlayerCar.Instance.gameObject))
        {
            canGoThroughStop = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<StopSignDetector>(out StopSignDetector detector))
        {
            canGoThroughStop = false;
        }

        if (other.transform.TryGetComponent<Node>(out Node node))
        {
            if (node.nodeType == Node.NodeType.Stop && node.stopType == Node.StopType.Violation)
            {
                points -= stoppedViolation;
            }
        }
    }
}
