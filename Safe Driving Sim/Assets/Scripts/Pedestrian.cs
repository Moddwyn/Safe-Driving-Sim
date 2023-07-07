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
    public bool isWaiting;
    public bool currentCrosswalk;
    public bool waitingToCross;

    void Start()
    {
        agent.speed = Random.Range(speedRange.x, speedRange.y);
        animator.SetFloat("Speed", Random.Range(animSpeedMult.x, animSpeedMult.y));
        SetNewDestination();
    }

    void Update()
    {
        // Check if the agent has reached its current destination
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance && !isWaiting)
        {
            StartCoroutine(WaitBeforeNewDestination());
        }

        currentCrosswalk = PathContainsCrossWalk(agent.path) && agent.hasPath;

        animator.SetBool("Walking", !isWaiting && !waitingToCross);

        if(agent.hasPath)
            agent.isStopped = waitingToCross && currentCrosswalk;
    }

    private bool PathContainsCrossWalk(NavMeshPath path)
    {
        NavMeshHit hit;
        for (int i = 1; i < path.corners.Length; i++)
        {
            if (NavMesh.SamplePosition(path.corners[i], out hit, 0.1f, 8))
            {
                return true;
            }
        }
        return false;
    }

    void SetNewDestination()
    {
        Vector3 randomDestination = PedestrianSpawner.GetRandomPointOnNavMesh(maxAttempts, maxDistance);
        agent.SetDestination(randomDestination);
    }

    IEnumerator WaitBeforeNewDestination()
    {
        isWaiting = true;
        yield return new WaitForSeconds(Random.Range(idleTimeRange.x, idleTimeRange.y));
        SetNewDestination();
        isWaiting = false;
    }

    
}
