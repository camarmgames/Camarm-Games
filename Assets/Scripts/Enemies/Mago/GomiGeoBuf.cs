using UnityEngine;
using UnityEngine.AI;

public class GomiGeoBuf: MonoBehaviour, IEnemyBuffable
{
    private float speedNormal;
    private NavMeshAgent navMeshAgent;
    private void Start()
    {
        speedNormal = GetComponent<NavMeshAgent>().speed;
        navMeshAgent = GetComponent<NavMeshAgent>();
    }
    public void OnBuffApplied()
    {
        navMeshAgent.speed *= 1.5f;
        Debug.Log($"{name} ha sido buffeado");
    }

    public void OnBuffRemoved()
    {
        navMeshAgent.speed = speedNormal;
        Debug.Log($"{name} vuelve a estado normal");
    }
}
