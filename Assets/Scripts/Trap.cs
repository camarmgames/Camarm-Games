using UnityEngine;
using System.Collections;

public class Trap: MonoBehaviour
{
    public enum TrapType
    {
        Slow,
        Stuck,
        GomiMago
    }

    [Header("Configuration of trap")]
    [SerializeField, Tooltip("Type of trap effect")]
    public TrapType trapType = TrapType.Slow;
    [SerializeField, Tooltip("Timing the trap stuck the player")]
    private float timingEffect = 5f;
    [SerializeField, Tooltip("If its temporal and disapear with the time")]
    private bool temporal = false;
    [SerializeField, Tooltip("Time of live of the trap")]
    private float lifeTime = 5f;

    [Header("Trap Materials")]
    public Material slowMaterial;
    public Material stuckMaterial;
    public Material magoMaterial;

    public Renderer rend;


    private float initialPlayerSpeed;
    private GomiMagoAppearance gomiMagoAppearance;
    public TrapSpawner trapSpawner;

    private void Start()
    {
        switch (trapType)
        {
            case TrapType.Slow:
                rend.material = slowMaterial;
                break;

            case TrapType.Stuck:
                rend.material = stuckMaterial;
                break;

            case TrapType.GomiMago:
                rend.material = magoMaterial;
                break;
        }

        gomiMagoAppearance = GetComponent<GomiMagoAppearance>();
        initialPlayerSpeed = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().initialSpeed;

        if (temporal)
            Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ActivateTrapEffect(other);
        }
    }

    public void ActivateTrapEffect(Collider target)
    {
        PlayerMovement playerMovement = target.GetComponent<PlayerMovement>();
        if (playerMovement == null) return;

        switch (trapType)
        {
            case TrapType.Slow:
                StartCoroutine(SlowEffect(playerMovement));
                break;

            case TrapType.Stuck:
                StartCoroutine(StuckEffect(playerMovement));
                break;

            case TrapType.GomiMago:
                ActivarMago();
                break;
        }
        
        if(trapSpawner != null)
            trapSpawner.limitTraps++;
    }

    IEnumerator SlowEffect(PlayerMovement player)
    {
        float originalSpeed = player.moveSpeed;

        if (initialPlayerSpeed == originalSpeed)
        {
            player.trapEffect = true;
            player.moveSpeed *= 0.4f;

            yield return new WaitForSeconds(timingEffect);

            player.moveSpeed = originalSpeed;
            player.trapEffect = false;
            Destroy(gameObject);
        }
    }

    IEnumerator StuckEffect(PlayerMovement player)
    {
        float originalSpeed = player.moveSpeed;
        player.trapEffect = true;
        player.moveSpeed = 0f;
        Debug.Log("Jugador atrapado");

        yield return new WaitForSeconds(timingEffect);

        player.moveSpeed = originalSpeed;
        player.trapEffect = false;
        Debug.Log("Jugador liberado");
        Destroy(gameObject);
    }

    public void ActivarMago()
    {
        Debug.Log("Activacion de Mago");
        //gomiMagoAppearance.SpawnMago();
        Destroy(gameObject);
    }
}
