using BehaviourAPI.Core;
using UnityEngine;
using UnityEngine.AI;

public class Break: MonoBehaviour
{
    [Header("BreakSettings")]
    public EnemyStateIcon stateIcon;

    private bool isTakingABreak;
    private NavMeshAgent agent;
    private StatsGomiNinja statsGomiNinja;

    private void Start()
    {
        statsGomiNinja = GetComponent<StatsGomiNinja>();
        agent = GetComponent<NavMeshAgent>();  
    }

    public Status TakeABreakStarted()
    {
        agent.isStopped = true;
        // StateIcon

        Debug.Log("Descansando");
        statsGomiNinja.ModifyStats(5, 0);
        isTakingABreak = true;

        return Status.Success;
    }

    public void TakeABreakStopped() => isTakingABreak = false;
    public bool IsTakingABreak() => isTakingABreak;
}
