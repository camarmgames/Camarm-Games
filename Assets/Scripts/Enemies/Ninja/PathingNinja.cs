using BehaviourAPI.Core;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PathingNinja: MonoBehaviour
{
    [Header("Settings Patrol")]
    [SerializeField] public List<Transform> patrolPoints;
    [SerializeField] private float waitTime = 2f;
    [SerializeField] private Animator animator;
    [SerializeField] private StatsGomiNinja statsGomiNinja;

    public int currentPointIndex = 0;
    private int lastPointIndex = -2;
    private float timer = 0f;
    private NavMeshAgent agent;
    private bool isPatrolling = false;

    private Break breakScript;


    private void Start()
    {
        //statsGomiNinja = GetComponent<StatsGomiNinja>();
        breakScript = GetComponent<Break>();
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if (!isPatrolling)
            return;

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            timer += Time.deltaTime;

            if (timer >= waitTime)
            {
                statsGomiNinja?.ModifyStats(-5, 5);

                GoToNextPoint();
                timer = 0f;
            }

        }
    }


    private void GoToNextPoint()
    {
        if (patrolPoints.Count == 0)
            return;


        if(currentPointIndex - 1 == lastPointIndex)
        {
            currentPointIndex = lastPointIndex;
            lastPointIndex = -2;
        }

        agent.SetDestination(patrolPoints[currentPointIndex].position);
        currentPointIndex = (currentPointIndex + 1) % patrolPoints.Count;
    }

    public Status StartPatrol()
    {
        if (isPatrolling)
            return Status.Success;

        if(breakScript.IsTakingABreak())
            breakScript.TakeABreakStopped();

        statsGomiNinja?.ModifyStats(-4, 7);

        isPatrolling = true;
        agent.isStopped = false;
        GoToNextPoint();
        //Debug.Log("Empieza a patrullar");

        if (animator != null)
            animator.SetBool("isWalking", isPatrolling);
        return Status.Success;
    }

    public void StopPatrol()
    {
        if (!isPatrolling)
            return;

        isPatrolling = false;
        agent.isStopped = true;
        timer = 0f;
        lastPointIndex = ((currentPointIndex - 1) + patrolPoints.Count) % patrolPoints.Count;
        //Debug.Log("Patrulla detenida");

        if(animator != null)
            animator.SetBool("isWalking", isPatrolling);
    }
}
