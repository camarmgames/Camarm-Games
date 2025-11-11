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

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
            Debug.LogError("El personaje no tiene un componente NavMeshAgent.");
        if (patrolPoints.Length <= 1)
        {
            Debug.LogError("No hay puntos de patrulla asignados o son insuficientes.");
            enabled = false;
            return;
        }
        SetPath();
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
        SetPath();
    }
    private void SetPath()
    {
        agent.SetDestination(patrolPoints[currentPointIndex].position);
    }
    public void EnablePatrol()
    {
        SetPath();
        enabled = true;
    }
    public void DisablePatrol()
    {
        enabled = false;
        agent.ResetPath();
    }
}
