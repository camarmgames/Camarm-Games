using System.Collections;
using UnityEngine;

public class LaunchFire: MonoBehaviour
{
    [Header("References of projectile")]
    [SerializeField, Tooltip("Position to fire")]
    private Transform firePosition;
    [SerializeField, Tooltip("Prefab of proyectile")]
    private GameObject proyectilePrefab;
    [SerializeField, Tooltip("Force of launch")]
    private float launchForce = 15f;

    [SerializeField, Tooltip("Cooldown between launchs")]
    private float launchCooldown = 5f;

    public Vector3 playerPosition;
    private bool canLaunch = true;

    public void Attack()
    {
        if(!canLaunch)
            return;

        Trap trap = proyectilePrefab.GetComponent<Trap>();
        Trap.TrapType randomType = (Trap.TrapType)Random.Range(0, System.Enum.GetValues(typeof(Trap.TrapType)).Length);

        trap.trapType = randomType;

        Vector3 firePoint = new Vector3(transform.position.x, firePosition.position.y, transform.position.z);
        GameObject proyectile = Instantiate(proyectilePrefab, firePoint, Quaternion.identity);

        // Calcula direccion hacia el jugador
        Vector3 impactPosition = new Vector3(playerPosition.x, playerPosition.y + 1.5f, playerPosition.z);
        Vector3 direction = (impactPosition - firePoint).normalized;

        // Lanza el proyectil con fuerza
        Rigidbody rb = proyectile.GetComponent<Rigidbody>();

        rb.AddForce(direction * launchForce, ForceMode.VelocityChange);

        StartCoroutine(LaunchCooldownRoutine());
    }

    private IEnumerator LaunchCooldownRoutine()
    {
        canLaunch = false;

        yield return new WaitForSeconds(launchCooldown);

        canLaunch = true;
    }
}
