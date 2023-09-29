using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEditor;
using NaughtyAttributes;

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
    [ReadOnly] public bool isIdling;
    [ReadOnly] public bool isWaiting;
    [ReadOnly] public bool onCrossWalk;

    NavMeshHit hit;

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
        animator.SetBool("Walking", !agent.isStopped || !isWaiting);
        if(agent.hasPath) agent.isStopped = isWaiting && !onCrossWalk;
        onCrossWalk = NavMesh.SamplePosition(agent.transform.position, out hit, 0.1f, 8);
    }

    void SetNewDestination()
    {
        Vector3 randomDestination = PedestrianManager.Instance.GetRandomPointOnNavMesh(maxAttempts, maxDistance);
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