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

    [SerializeField]
    private Animator animator;

    public Vector3 playerPosition;
    private bool canLaunch = true;
    private Coroutine launchCoroutine;
    private bool isLaunchingAnimation = false;

    private void Update()
    {
        if (animator != null)
        {
            animator.SetBool("isLaunching", isLaunchingAnimation);
        }
    }

    public void StopLaunchCoroutine()
    {
        if (launchCoroutine != null)
        {
            StopCoroutine(launchCoroutine);
            launchCoroutine = null;
            isLaunchingAnimation = false;
        }
    }

    public void Attack()
    {
        if(!canLaunch)
            return;

        launchCoroutine = StartCoroutine(LaunchRoutine());

        StartCoroutine(LaunchCooldownRoutine());
    }
    private IEnumerator LaunchRoutine()
    {
        if (animator != null)
        {
            isLaunchingAnimation = true;

            yield return new WaitUntil(() =>
                animator.GetCurrentAnimatorStateInfo(0).IsName("Launch"));

            yield return new WaitUntil(() =>
                animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);

            isLaunchingAnimation = false;
        }

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

        launchCoroutine = null;
    }

    private IEnumerator LaunchCooldownRoutine()
    {
        canLaunch = false;

        yield return new WaitForSeconds(launchCooldown);

        canLaunch = true;
    }

    public bool FinishLaunching()
    {
        return launchCoroutine == null;
    }
}
