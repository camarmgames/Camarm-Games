using BehaviourAPI.Core;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Investigation: MonoBehaviour
{
    [Header("Settings Investigation")]
    [SerializeField] private float inspectionRadius = 3f;
    [SerializeField] private float timePerPoint = 2f;
    [SerializeField] private int pointsToInvestigate = 5;
    [SerializeField] private float maxAngleDeviation = 30f;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private Animator animator;

    [Header("Debug")]
    public Vector3 pointToInvestigateArea;

    private NavMeshAgent agent;
    public bool isInvestigating = false;
    private Coroutine investigateCoroutine;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        
    }

    public void InvestigateArea()
    {
        if (!isInvestigating)
        {
            isInvestigating = true;
            agent.isStopped = false;
            investigateCoroutine = StartCoroutine(InspectArea(pointToInvestigateArea));
            if (animator != null)
                animator.SetBool("isWalking", isInvestigating);
        }
    }

    public void StopInvestigation()
    {
        if(investigateCoroutine != null)
        {
            StopCoroutine(investigateCoroutine);
            investigateCoroutine = null;
            isInvestigating = false;
            if (animator != null)
                animator.SetBool("isWalking", isInvestigating);
        } 
    }

    private IEnumerator InspectArea(Vector3 targetPosition)
    {
        agent.SetDestination(targetPosition);

        while (!agent.pathPending && agent.remainingDistance > agent.stoppingDistance)
        {
            yield return null;
        }

        Vector3 baseDirection = (targetPosition - transform.position).normalized;

        for (int i = 0; i < pointsToInvestigate; i++)
        {
            Vector3 randomPoint = GetRandomPointAround(targetPosition, baseDirection,inspectionRadius);
            agent.SetDestination(randomPoint);

            while (!agent.pathPending && agent.remainingDistance > agent.stoppingDistance)
            {
                SmoothLookAt(agent.steeringTarget);
                yield return null;
            }

            if(i > 0)
            {
                float timer = 0f;
                while (timer < timePerPoint)
                {
                    timer += Time.deltaTime;
                    SmoothLookAt(agent.steeringTarget);
                    yield return null;
                }
            }
        }

        isInvestigating = false;
        investigateCoroutine = null;
    }

    private Vector3 GetRandomPointAround(Vector3 center, Vector3 forwardDirection, float radius)
    {
        float angle = Random.Range(-maxAngleDeviation, maxAngleDeviation);
        Quaternion rotation = Quaternion.Euler(0, angle, 0);
        Vector3 direction = rotation * forwardDirection;

        Vector3 randomPos = center + direction * Random.Range(radius * 0.5f, radius);
        randomPos.y = center.y;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPos, out hit, radius, NavMesh.AllAreas))
            return hit.position;
        
        return center;
    }

    private void SmoothLookAt(Vector3 lookTarget)
    {
        Vector3 direction = (lookTarget - transform.position).normalized;
        direction.y = 0f;

        if(direction.magnitude > 0.1f)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
        }
    }
}
