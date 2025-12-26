using UnityEngine;

public class StatsGomiNinja: MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] public float estamina = 100f;
    [SerializeField] private float tiempoPatrullando = 0f;
    [SerializeField] public float agotamiento = 1f;

    [Header("Settings")]
    [SerializeField] public float changeSpeed = 10f;

    [Header("Debug")]
    [SerializeField] public bool debug = false;

    private float targetStamina;
    private float targetTimePatrol;

    private void Start()
    {
        targetStamina = estamina;
        targetTimePatrol = tiempoPatrullando;
    }


    private void Update()
    {
        if(Mathf.Abs(estamina - targetStamina) > 0.01f)
        {
            estamina = Mathf.Clamp(Mathf.MoveTowards(estamina, targetStamina, changeSpeed * Time.deltaTime), 0f, 100f);
        }

        if(Mathf.Abs(tiempoPatrullando - targetTimePatrol) > 0.01f)
        {
            tiempoPatrullando = Mathf.Clamp(Mathf.MoveTowards(tiempoPatrullando, targetTimePatrol, changeSpeed * Time.deltaTime), 0, 100f);
        }
    }

    public void ModifyStats(float staminaAmount, float timePatrolAmount)
    {
        targetStamina = Mathf.Clamp(targetStamina + staminaAmount, 0f, 100f);
        targetTimePatrol = Mathf.Clamp(targetTimePatrol + timePatrolAmount, 0f, 100f);

        if (debug)
        {
            Debug.Log($"Stamina: {targetStamina}");
            Debug.Log($"TimePatrol: {tiempoPatrullando}");
        }

    }

    public void ResetTimePatrol()
    {
        tiempoPatrullando = 0f;
    }

    public float GetStamina() => estamina;
    public float GetTimePatrol() => tiempoPatrullando;

    public float GetTakeABreak() => agotamiento;
}
