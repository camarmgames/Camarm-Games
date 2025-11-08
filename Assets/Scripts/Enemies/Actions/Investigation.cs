using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Investigation: MonoBehaviour
{
    [Header("Settings Investigation")]
    [SerializeField] private float inspectionRadius = 3f;
    [SerializeField] private float timePerPoint = 2f;
    [SerializeField] private int pointsToInvestigate = 5;

    [Header("Debug")]
    public Transform pointToInvestigateArea;

    private NavMeshAgent agent;
    private bool isInvestigating = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>(); 
        
        if(pointToInvestigateArea != null)
        {
            InvestigateArea(pointToInvestigateArea.position);
        }
    }

    public void InvestigateArea(Vector3 targetPosition)
    {
        if (!isInvestigating)
        {
            StartCoroutine(InspectArea(targetPosition));
        }
    }

    private IEnumerator InspectArea(Vector3 targetPosition)
    {
        isInvestigating = true;

        agent.SetDestination(targetPosition);

        while (!agent.pathPending && agent.remainingDistance > agent.stoppingDistance)
        {
            yield return null;
        }

        for (int i = 0; i < pointsToInvestigate; i++)
        {
            Vector3 randomPoint = GetRandomPointAround(targetPosition, inspectionRadius);
            agent.SetDestination(randomPoint);

            while (!agent.pathPending && agent.remainingDistance > agent.stoppingDistance)
            {
                yield return null;
            }

            float timer = 0f;
            while (timer < timePerPoint)
            {
                timer += Time.deltaTime;
                yield return null;
            }
        }

        isInvestigating = false;
    }

    private Vector3 GetRandomPointAround(Vector3 center, float radius)
    {
        Vector3 randomPos = center + Random.insideUnitSphere * radius;
        randomPos.y = center.y;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPos, out hit, radius, NavMesh.AllAreas))
        {
            return hit.position;
        }
        return center;
    }
}
