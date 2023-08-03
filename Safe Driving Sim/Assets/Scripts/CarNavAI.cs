using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.AI;

public class CarNavAI : MonoBehaviour
{
    public NavMeshAgent navAgent;
    [SerializeField] List<Node> nodes;
    [ReadOnly] public int currIndex = 0;
    [HorizontalLine]
    public Vector2 speedRange;

    [HorizontalLine]
    [ReadOnly] public float speed = 1f;
    [ReadOnly] public float currentSpeed;

    void Awake()
    {
        speed = Random.Range(speedRange.x, speedRange.y);
        currentSpeed = speed;
    }

    void Update()
    {
        navAgent.speed = currentSpeed;
        CarMovement();
    }

    void CarMovement()
    {
        if (nodes.Count == 0) return;

        Vector3 targetPosition = nodes[currIndex].transform.position;

        navAgent.SetDestination(targetPosition);
        float remainingDistance = navAgent.remainingDistance;

        // Check if the car has reached the target position
        if (Mathf.Abs(transform.root.position.x - targetPosition.x) < 0.1f && Mathf.Abs(transform.root.position.z - targetPosition.z) < 0.1f) {
            currIndex++;
            if (currIndex >= nodes.Count)
                currIndex = 0;
        }
    }
}
