using BehaviourAPI.Core;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class TrapSpawner: MonoBehaviour
{
    [Header("Trap Settings")]
    [SerializeField, Tooltip("Prefab of the trap")]
    private GameObject trapPrefab;

    [SerializeField, Tooltip("Heigh of colocation from the floor")]
    private float trapHeightOffset = 0.1f;

    [SerializeField, Tooltip("Limit of traps")]
    public int limitTraps = 3;

    [SerializeField]
    private Animator animator;

    [Header("Debug")]
    public bool debug;

    private StatsGomiNinja statsGomiNinja;

    private void Start()
    {
        statsGomiNinja = GetComponent<StatsGomiNinja>();
    }
    public void TrapSpawnerStarted()
    {
        Debug.Log("Intentando poner trampa");
        if(limitTraps > 0)
        {
            if(debug)
                Debug.Log($"Se puede poner trampa y me quedan {limitTraps}");
            animator.Play("PlaceTrap");
        }
        else
        {
            if(debug)
                Debug.Log("No me quedan trampas");
        }

        
    }
    public Status TrapSpawnerUpdate()
    {
        if (limitTraps <= 0) return Status.Success;

        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.75f)
            return Status.Running;

        PlaceTrap();


        return Status.Success;
    }

    private void PlaceTrap()
    {
        Vector3 trapPos = new Vector3(transform.position.x, transform.position.y + trapHeightOffset, transform.position.z);

        Trap trap = trapPrefab.GetComponent<Trap>();
        Trap.TrapType randomType = (Trap.TrapType)Random.Range(0, System.Enum.GetValues(typeof(Trap.TrapType)).Length);

        trap.trapType = randomType;
        trap.trapSpawner = this;

        Instantiate(trapPrefab, trapPos, transform.rotation);

        Debug.Log($"Trampa colocada: {randomType} en {transform.position}");
        limitTraps--;
        statsGomiNinja.ResetTimePatrol();
    }

    public void PlaceTrapPrueba()
    {
        //Debug.Log("Trampa colocadaaAAAAA");


    }
}
