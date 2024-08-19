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
    public float outOfLaneViolationDelay = 0.25f;
    public int speedingViolation = 1;
    public float speedingViolationDelay = 0.25f;
    public float rotationMaxFail = 15;
    private int currentSpeedLimit;

    [HorizontalLine]
    [ReadOnly] public bool canGoThroughStop;
    [ReadOnly] public bool centered;
    [ReadOnly] public bool outOfLane;
    [ReadOnly] public bool speeding;
    [ReadOnly] public bool failed;

    [HorizontalLine]
    public PlayerCarUI carUI;

    [HorizontalLine]
    [ReadOnly] public Road currentRoad;

    private PlayerCar player;
    private ScenerioManager scenerioManager;

    public static PointSystem Instance;


    void Awake()
    {
        Instance = this;
        player = GetComponent<PlayerCar>();
        scenerioManager = FindObjectOfType<ScenerioManager>();
    }

    void Start()
    {
        carUI.StartResumeTimer();
        
        StartCoroutine(CheckSpeeding());
        StartCoroutine(CheckOutOfLane());
    }

    void Update()
    {
        if (points <= 0 && !failed)
        {
            failed = true;
            carUI.cause = "Too many unsafe maneuvers";
            OnFail?.Invoke();
        }
        
        Vector3 eulerRot = transform.rotation.eulerAngles;
        eulerRot = new Vector3(NormalAngle(eulerRot.x), NormalAngle(eulerRot.y), NormalAngle(eulerRot.z));
        if (Mathf.Abs(eulerRot.z) >= rotationMaxFail & !failed)
        {
            failed = true;
            carUI.cause = "Flipped Over";
            OnFail?.Invoke();
        }

        CheckRoad();
    }

    public void ResetPoints() {
        scenerioManager.hasReachedEnd = false;
        points = 100;
    }


    float NormalAngle(float angle)
    {
        while(angle > 180) angle -= 360;
        while(angle < -180) angle += 360;
        return angle;
    }

    private IEnumerator CheckSpeeding() {
        yield return new WaitForSeconds(speedingViolationDelay);
        speeding = player.currentSpeed > currentSpeedLimit;
        if (speeding) points -= speedingViolation;
        StartCoroutine(CheckSpeeding());
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
        yield return new WaitForSeconds(outOfLaneViolationDelay);
        if(outOfLane) points -= outOfLaneViolation;
        StartCoroutine(CheckOutOfLane());
    }


    void OnCollisionEnter(Collision other)
    {
        if (other.transform.CompareTag("Collision") && other.transform != transform && !failed)
        {
            failed = true;
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


        if (other.TryGetComponent<Node>(out Node node))
        {
            if (node.route != null) currentSpeedLimit = node.route.maxMPH;
        }
        

        if (!scenerioManager.hasReachedEnd && scenerioManager.endNode != null && other.transform.GetComponent<Node>() == scenerioManager.endNode)
        {
            scenerioManager.hasReachedEnd = true;
            Debug.Log("yes");
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
