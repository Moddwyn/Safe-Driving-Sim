using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;

public class CarAIController : MonoBehaviour
{
    public PathFinder pathFinder;
    [ReadOnly] public List<Node> nodePath;

    [HorizontalLine]
    public bool autoCalcMaxSpeed;
    [InfoBox("Max Speed: Car's Max Miles Per Hour")]
    [HideIf("autoCalcMaxSpeed")] public float maxMPH = 13.57629966f;
    [InfoBox("Motor Torque: Car's Torque in Nm")]
    public float motorTorque = 130;
    [InfoBox("Brake Torque: Applied Brake Torque if car's speed exceed max vel")]
    public float brakeTorque = 100;
    [InfoBox("Max Rot Speed: Max car wheel turning speed")]
    public float maxRotationSpeed = 500;

    [HorizontalLine]
    public WheelCollider[] wheelColliders;
    public BoxCollider longCollider;
    public BoxCollider closeCollider;
    public bool printLog;

    [HorizontalLine]
    [ReadOnly][SerializeField] int currentNodeIndex = 0;
    [ReadOnly][SerializeField] Node currentNode;
    [ReadOnly][SerializeField] Node nextNodeStart;
    [ReadOnly][SerializeField] float targetDistance;
    [ReadOnly][SerializeField] float desiredSpeed = 0f;
    [ReadOnly][SerializeField] float desiredBrakeTorque = 0f;
    [ReadOnly][SerializeField] float desiredMaxVel;
    [ReadOnly][SerializeField] float maxVelCurr;
    [ReadOnly][SerializeField] float currentSpeed;
    [ReadOnly][SerializeField] Collider[] collidersInFront;
    [ReadOnly][SerializeField] BoxCollider colliderBeingUsed;

    [HorizontalLine]
    [ReadOnly][SerializeField] bool frontCollision;


    Vector3 direction;
    Vector3 targetPosition;
    Rigidbody rb;
    RoadManager roadManager;


    void Start()
    {
        roadManager = RoadManager.Instance;
        rb = GetComponent<Rigidbody>();
        CalculatePath();
    }

    public void CalculatePath()
    {
        if(pathFinder != null)
        {
            pathFinder.start = FindClosestNode(transform);
            pathFinder.end = roadManager.GetRandomWaypoint(pathFinder.start);
            pathFinder.AutoCalculate((x)=>
            {
                nodePath = x;
                SetNewTarget(0);
            });
        }
    }

    void Update()
    {
        // Remove all null nodes
        nodePath.RemoveAll(x => x == null);
        // Set bool if front collider has collision
        if(nodePath.Count > 0)
        {
            frontCollision = 
            CheckCollisionWithTag(
                (currentNode.isIntersection || (currentNodeIndex > 0 && nodePath[currentNodeIndex - 1].isIntersection))? 
                closeCollider : longCollider, "Collision");
        }
        // Set current speed to rigidbody velocity
        currentSpeed = rb.velocity.magnitude;
        // Set desired max velocity based on manual or auto calculated value
        desiredMaxVel = autoCalcMaxSpeed ? maxVelCurr / 2.23694f : maxMPH / 2.23694f;

        if (currentNode == null || nodePath.Count == 0) return;
        nextNodeStart = GetNextRouteStart();            
    }

    void FixedUpdate()
    {
        // No nodes, no target
        if (nodePath.Count == 0) currentNode = null;
        if (currentNode == null || nodePath.Count == 0) return;

        SteerSpeedUpdate();
        SteerRotationUpdate();
        NodeCheckUpdate();
    }

    void SteerSpeedUpdate()
    {
        // If there's a front collision, immediately pressed on brakes hard
        if (frontCollision)
        {
            desiredSpeed = 0;
            desiredBrakeTorque = float.MaxValue;
        }
        else
        // Normal node steering
        {
            // If our current speed is less than maximum desiered velocity, press on gas... if not, break to not go any faster
            if (currentSpeed <= desiredMaxVel)
            {
                desiredSpeed = motorTorque;
                desiredBrakeTorque = 0;
            }
            else
            {
                desiredSpeed = 0;
                desiredBrakeTorque = brakeTorque;
            }

            if(printLog)
            {
                print("1: " + (currentNode.nodeType == Node.NodeType.Stop));
                print("2: " + OnLastNode());
                print("3: " + (targetDistance <= (0 - (-2.4324f + -0.0175f * motorTorque)) / 1.2309f));
                print("4: " + GetNextRouteStart().hasTraffic);
                print("5: " + GetNextRouteStart().carsInRadius.Any(x=>x != this));
                print("6: " + currentNode.isStartNode);
            }
            // If the current node target is a stop node or the last node
            if(currentNode.nodeType == Node.NodeType.Stop || OnLastNode())
            {
                if (targetDistance <= (0 - (-2.4324f + -0.0175f * motorTorque)) / 1.2309f)
                {
                    desiredSpeed = 0;
                    desiredBrakeTorque = float.MaxValue;
                }
            }

            if (GetNextRouteStart().hasTraffic && GetNextRouteStart().carsInRadius.Any(x=>x != this))
            {
                if (targetDistance <= (0 - (-2.4324f + -0.0175f * motorTorque)) / 1.2309f || currentNode.isStartNode)
                {
                    desiredSpeed = 0;
                    desiredBrakeTorque = float.MaxValue;
                }
            }
        }

        // Accelerate or decelerate using Wheel Colliders
        foreach (WheelCollider wheel in wheelColliders)
        {
            wheel.motorTorque = desiredSpeed;
            wheel.brakeTorque = desiredBrakeTorque;
        }
    }

    void SteerRotationUpdate()
    {
        direction = (targetPosition - transform.position).normalized;

        float rotationStep = maxRotationSpeed * Time.deltaTime;
        float targetSteerAngle = Vector3.Angle(transform.forward, direction) * Mathf.Sign(transform.InverseTransformPoint(targetPosition).x);
        foreach (WheelCollider wheel in wheelColliders)
        {
            if (wheel.transform.localPosition.z > 0) // Front wheels
            {
                float currentSteerAngle = Mathf.MoveTowardsAngle(wheel.steerAngle, targetSteerAngle, rotationStep);
                wheel.steerAngle = currentSteerAngle;
            }
        }
    }

    void NodeCheckUpdate()
    {
        // Set target to the current node 
        if (currentNodeIndex <= nodePath.Count - 1) SetNewTarget(currentNodeIndex);

        // Set the target position of our current node
        targetPosition = new Vector3(currentNode.transform.position.x, transform.position.y, currentNode.transform.position.z);

        // Set distance to the target node
        targetDistance = Vector3.Distance(transform.position, targetPosition);

        // If we approached our target within set radius & this is not a stop node
        if (targetDistance <= RoadManager.Instance.carCheckRad && nodePath[currentNodeIndex].nodeType != Node.NodeType.Stop)
        {
            // If we are on the last node
            if (OnLastNode()) currentNodeIndex = nodePath.Count - 1;
            else currentNodeIndex++;

            if(currentNodeIndex == nodePath.Count - 2)
            {
                pathFinder.start = nodePath[^1];
                pathFinder.end = roadManager.GetRandomWaypoint(pathFinder.start);
                pathFinder.AutoCalculate((x)=>
                {
                    nodePath.RemoveRange(0, currentNodeIndex);
                    SetNewTarget(0);

                    nodePath.AddRange(pathFinder.bestPath);
                });
            }
        }
    }

    void SetNewTarget(int newIndex)
    {
        currentNodeIndex = newIndex;
        currentNode = nodePath[currentNodeIndex];

        maxVelCurr = Mathf.MoveTowards(maxVelCurr, currentNode.route.maxMPH, 0.05f);
    }

    public Node FindClosestNode(Transform current)
    {
        Node closestTransform = null;
        float closestDistance = Mathf.Infinity;

        foreach (Node otherTransform in roadManager.allNodes)
        {
            float distance = Vector3.Distance(current.position, otherTransform.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestTransform = otherTransform;
            }
        }

        return closestTransform;
    }

    bool CheckCollisionWithTag(BoxCollider collider, string tag)
    {
        colliderBeingUsed = collider;
        Bounds colliderBounds = collider.bounds;
        collidersInFront = Physics.OverlapBox(colliderBounds.center, colliderBounds.extents)
                            .Where(x=>x.GetComponent<CarAIController>() != null || x.GetComponent<PlayerCar>() != null)
                            .Where(x=>x.GetComponent<CarAIController>() != this)
                            .Where(x=>x.transform.root.GetComponent<CarAIController>() != this)
                            .Where(x=>x.CompareTag(tag)).ToArray();

        return collidersInFront.Length > 0;
    }

    bool OnLastNode() => currentNodeIndex >= nodePath.Count - 1;

    // Gets the next node that is a starting node of a route
    Node GetNextRouteStart()
    {
        if (currentNodeIndex == nodePath.Count - 1)
            return nodePath[nodePath.Count - 1];
        if(currentNode.isStartNode && !currentNode.isIntersection)
            return currentNode;
        if(currentNode == nodePath[0])
            return currentNode;

        int nodeCheck = currentNodeIndex + 1;
        bool isPartOfIntersection = true;
        while (isPartOfIntersection)
        {
            isPartOfIntersection = nodePath[nodeCheck].route.isIntersection;
            if (!isPartOfIntersection && nodePath[nodeCheck].isStartNode) break;
            nodeCheck++;

            if(nodeCheck >= nodePath.Count) return nodePath[currentNodeIndex + 1];
        }
        
        return (currentNode.isEndNode || nodePath[currentNodeIndex - 1].isEndNode)?
        nodePath[nodeCheck] : nodePath[currentNodeIndex + 1];
    }

    // Gets the next node after the given startNode
    public Node GetNodeAfter(Node startNode)
    {
        int indexOfStartNode = nodePath.IndexOf(startNode);
        if(indexOfStartNode == nodePath.Count - 1) return startNode;
        return nodePath[indexOfStartNode + 1];
    }

    public bool IsStoppedOrStopping() => desiredBrakeTorque > brakeTorque || currentSpeed <= 0.5f;

}