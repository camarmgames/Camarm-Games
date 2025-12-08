using BehaviourAPI.Core;
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

    public AudioClip effect;

    [SerializeField]
    private Animator animator;

    public Vector3 playerPosition;
    private bool canLaunch = true;
    private EnemyStress enemyStress;

    private void Start()
    {
        enemyStress = GetComponent<EnemyStress>();
    }

    public void AttackStarted()
    {
        if (!canLaunch)
            return;

        animator.Play("Launch");
        enemyStress?.AddStress(8);
    }

    public Status AttackUpdate()
    {
        if(!canLaunch)
            return Status.Success;

        if(animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.75f)
            return Status.Running;

        LaunchProyectile();

        
        return Status.Success;
    }

    private void LaunchProyectile()
    {
        Trap trap = proyectilePrefab.GetComponent<Trap>();
        Trap.TrapType randomType = (Trap.TrapType)Random.Range(0, System.Enum.GetValues(typeof(Trap.TrapType)).Length);

        trap.trapType = randomType;

        Vector3 firePoint = new Vector3(transform.position.x, firePosition.position.y, transform.position.z);
        GameObject proyectile = Instantiate(proyectilePrefab, firePoint, Quaternion.identity);

        // Calcula direccion hacia el jugador
        Vector3 impactPosition = new Vector3(playerPosition.x, playerPosition.y + 0.5f, playerPosition.z);
        Vector3 direction = (impactPosition - firePoint).normalized;

        // Lanza el proyectil con fuerza
        Rigidbody rb = proyectile.GetComponent<Rigidbody>();

        rb.AddForce(direction * launchForce, ForceMode.VelocityChange);

        // Efecto Musica
        AudioManager.Instance.PlaySFXAtPosition(effect, transform.position, 1f, 1f);

        StartCoroutine(LaunchCooldownRoutine());
    }

    private IEnumerator LaunchCooldownRoutine()
    {
        canLaunch = false;

        yield return new WaitForSeconds(launchCooldown);

        canLaunch = true;
    }

    public Status pruebaFinish()
    {
        return Status.Success;
    }
}
