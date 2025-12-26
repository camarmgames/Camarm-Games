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

    public void TakeABreakStarted()
    {
        agent.isStopped = true;
        stateIcon.SetTakeABreak();

        Debug.Log("Descansando");
        statsGomiNinja.ModifyStats(40, 0);
        statsGomiNinja.agotamiento = 0f;
        isTakingABreak = true;
    }

    public Status TakeABreakUpdate()
    {
        if(statsGomiNinja.estamina <= 80)
            return Status.Running;

        statsGomiNinja.agotamiento = 1f;

        return Status.Success;

    }

    public void TakeABreakStopped() => isTakingABreak = false;
    public bool IsTakingABreak() => isTakingABreak;
}
