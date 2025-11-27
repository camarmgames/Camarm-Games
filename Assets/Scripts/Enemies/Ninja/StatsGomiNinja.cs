using UnityEngine;

public class StatsGomiNinja: MonoBehaviour
{
    // Que no se pasen de 100
    [Header("Stats")]
    [SerializeField] private float stamina = 100f;
    [SerializeField] private float timePatrol = 0f;

    [Header("Settings")]
    [SerializeField] private float changeSpeed = 10f;

    private float targetStamina;
    private float targetTimePatrol;

    private void Start()
    {
        targetStamina = stamina;
        targetTimePatrol = timePatrol;
    }


    private void Update()
    {
        if(Mathf.Abs(stamina - targetStamina) > 0.01f)
        {
            stamina = Mathf.Clamp(Mathf.MoveTowards(stamina, targetStamina, changeSpeed * Time.deltaTime), 0f, 100f);
        }

        if(Mathf.Abs(timePatrol - targetTimePatrol) > 0.01f)
        {
            timePatrol = Mathf.Clamp(Mathf.MoveTowards(timePatrol, targetTimePatrol, changeSpeed * Time.deltaTime), 0, 100f);
        }
    }

    public void ModifyStats(float staminaAmount, float timePatrolAmount)
    {
        targetStamina = Mathf.Clamp(targetStamina + staminaAmount, 0f, 100f);
        targetTimePatrol = Mathf.Clamp(targetTimePatrol + timePatrolAmount, 0f, 100f);
    }

    public void ResetTimePatrol()
    {
        timePatrol = 0f;
    }

    public float GetStamina() => stamina;
    public float GetTimePatrol() => timePatrol;
}
