using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEditor;

public class Pedestrian : MonoBehaviour
{
    public Vector2 speedRange;
    public Vector2 idleTimeRange; // Idle time when it reaches destination
    public Vector2 animSpeedMult;
    public int maxAttempts = 30; // Maximum number of attempts to find a valid random point
    public float maxDistance = 100f; // Maximum distance from the agent to sample a random point
    public NavMeshAgent agent;
    public Animator animator;

    [Space(20)]
    public bool isIdling;
    public bool isWaiting;

    void Start()
    {
        agent.speed = Random.Range(speedRange.x, speedRange.y);
        animator.SetFloat("Speed", Random.Range(animSpeedMult.x, animSpeedMult.y));
        SetNewDestination();
    }

    void Update()
    {
        // Check if the agent has reached its current destination
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance && !isIdling)
            StartCoroutine(WaitBeforeNewDestination());
        animator.SetBool("Walking", !isIdling && !isWaiting);
        if(agent.hasPath) agent.isStopped = isWaiting;
    }

    void SetNewDestination()
    {
        Vector3 randomDestination = PedestrianSpawner.GetRandomPointOnNavMesh(maxAttempts, maxDistance);
        agent.SetDestination(randomDestination);
    }

    IEnumerator WaitBeforeNewDestination()
    {
        isIdling = true;
        yield return new WaitForSeconds(Random.Range(idleTimeRange.x, idleTimeRange.y));
        SetNewDestination();
        isIdling = false;
    }

    
}
