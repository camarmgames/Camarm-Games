using BehaviourAPI.Core;
using UnityEngine;
using UnityEngine.AI;

public class Break: MonoBehaviour
{
    [Header("BreakSettings")]
    public EnemyStateIcon stateIcon;
    [SerializeField] private StatsGomiNinja statsGomiNinja;
    [SerializeField] private StatsGomiGeo statsGomiGeo;

    private bool isTakingABreak;
    private NavMeshAgent agent;


    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();  
    }

    public void TakeABreakStarted()
    {
        agent.isStopped = true;
        stateIcon.SetTakeABreak();

        Debug.Log("Descansando");
        // Ninja
        statsGomiNinja?.ModifyStats(40, 0);
        statsGomiNinja?.SetTakeABreak(0f);

        // Geo
        statsGomiGeo?.ModifyStats(50, 0); 
        statsGomiGeo?.SetTakeABreak(0f);

        isTakingABreak = true;
    }

    public Status TakeABreakUpdate()
    {
        if(statsGomiNinja?.estamina <= 80 || statsGomiGeo?.estamina <= 85)
            return Status.Running;

        statsGomiNinja?.SetTakeABreak(1f);

        statsGomiGeo?.SetTakeABreak(1f);

        return Status.Success;

    }

    public void TakeABreakStopped() => isTakingABreak = false;
    public bool IsTakingABreak() => isTakingABreak;
}
