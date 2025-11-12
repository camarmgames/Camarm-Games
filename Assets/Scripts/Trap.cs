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

    private float initialPlayerSpeed;
    public TrapSpawner trapSpawner;

    private void Start()
    {
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
            player.moveSpeed *= 0.4f;

            yield return new WaitForSeconds(timingEffect);

            player.moveSpeed = originalSpeed;
            Destroy(gameObject);
        }
    }

    IEnumerator StuckEffect(PlayerMovement player)
    {
        float originalSpeed = player.moveSpeed;
        player.moveSpeed = 0f;
        Debug.Log("Jugador atrapado");

        yield return new WaitForSeconds(timingEffect);

        player.moveSpeed = originalSpeed;
        Debug.Log("Jugador liberado");
        Destroy(gameObject);
    }

    public void ActivarMago()
    {
        Debug.Log("Activacion de Mago");
        Destroy(gameObject);
    }
}
