using UnityEngine;
using UnityEngine.AI;

public class Patrol: MonoBehaviour
{
    [Header("Settings Patrol")]
    [SerializeField] private Transform[] patrolPoints;
    [SerializeField] private float waitTime = 2f;

    private int currentPointIndex = 0;
    private float timer = 0f;
    private NavMeshAgent agent;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        if(patrolPoints.Length == 0)
        {
            Debug.LogError("No hay puntos de patrulla asignados");
            enabled = false;
            return;
        }

        StartPathing();
    }

    private void Update()
    {
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance) {
            timer += Time.deltaTime;

            if (timer >= waitTime) {
                GoToNextPoint();
                timer = 0f;
            }
            
        }
    }


    private void GoToNextPoint()
    {
        if(patrolPoints.Length == 0)
            return;

        currentPointIndex = (currentPointIndex + 1) % patrolPoints.Length;
        agent.SetDestination(patrolPoints[currentPointIndex].position);
    }

    private void StartPathing()
    {
        agent.SetDestination(patrolPoints[currentPointIndex].position);
    }
}
