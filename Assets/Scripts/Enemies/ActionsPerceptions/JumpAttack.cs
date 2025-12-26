
using BehaviourAPI.Core;
using Cinemachine;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class JumpAttack: MonoBehaviour
{
    [Header("Objetivo")]
    public GameObject playerTarget;
    public LayerMask playerMask;        // Jugador
    public LayerMask obstacleMask;      // Obstáculos (mismos que usas en tu visión)

    [Header("Parálisis")]
    public float paralysisRange = 10f;
    public float paralysisDuration = 3f;
    public float shakeIntensity = 2f;

    [Header("Salto")]
    public float jumpCooldown = 3f;
    public float bodyRadius = 0.6f; // Para SphereCast
    public AudioClip jumpSound;

    [Header("Animations Settings")]
    public Animator animator;
    public NoiseSettings typeNoiseCamera;
    public float intensityNoiseCamera;

    [Header("Debug")]
    public bool debug;

    private NavMeshAgent agent;
    private bool canJump = true;
    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();

    }

    public bool CanJump() { return canJump; }

    public void JumpAttackStarted()
    {
        if (!canJump) return;

        animator.Play("JumpAttack");
    }

    public Status JumpAttackUpdate()
    {
        if(!canJump) return Status.Success;

        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.75f)
            return Status.Running;

        AudioManager.Instance?.PlaySFXAtPosition(jumpSound, transform.position, 1f, 1f);

        TryParalyzePlayer();

        StartCoroutine(JumpCooldownRoutine());

        return Status.Success;
    }

    private bool TryParalyzePlayer()
    {
        float dist = Vector3.Distance(transform.position, playerTarget.transform.position);

        if (dist > paralysisRange)
            return false;

        if (!HasLineOfSight())
            return false;

        StartCoroutine(ParalyzeRoutine());
        
        return true;
    }

    IEnumerator ParalyzeRoutine()
    {
        // Sacudir cámara
        CameraShake.Instance?.ShakeCamera(typeNoiseCamera, intensityNoiseCamera, paralysisDuration);

        // Llamar parálisis al jugador
        PlayerMovement playerMovement = playerTarget.GetComponent<PlayerMovement>();
        float originalSpeed = playerMovement.moveSpeed;
        playerMovement.stunnedTrap = true;
        playerMovement.moveSpeed = 0f;
        if(debug)
            Debug.Log("Jugador atrapado");

        yield return new WaitForSeconds(paralysisDuration);

        playerMovement.moveSpeed = originalSpeed;
        playerMovement.stunnedTrap = false;
        if(debug)
            Debug.Log("Jugador liberado");
    }

    private IEnumerator JumpCooldownRoutine()
    {
        canJump = false;

        yield return new WaitForSeconds(jumpCooldown);

        canJump = true;
    }

    private bool HasLineOfSight()
    {
        Vector3 origin = transform.position + Vector3.up * 1.5f;
        Vector3 dir = (playerTarget.transform.position - transform.position).normalized;
        float dist = Vector3.Distance(transform.position, playerTarget.transform.position);

        // Evitar obstáculos entre medio
        if (Physics.SphereCast(origin, bodyRadius, dir, out RaycastHit hit, dist, obstacleMask))
        {
            Debug.DrawRay(origin, dir * hit.distance, Color.red, 0.3f);
            return false;
        }

        Debug.DrawRay(origin, dir * dist, Color.green, 0.3f);
        return true;
    }

    private void OnDrawGizmosSelected()
    {
        // 1. Rango de parálisis/salto
        Gizmos.color = new Color(0f, 0.5f, 1f, 0.25f);   // Azul semitransparente
        Gizmos.DrawWireSphere(transform.position, paralysisRange);

        if (playerTarget == null) return;

        // 2. Línea hacia el jugador
        Vector3 origin = transform.position + Vector3.up * 0.5f;
        Vector3 dirToPlayer = (playerTarget.transform.position - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, playerTarget.transform.position);

        // 3. Comprobar si hay obstáculo (igual que HasLineOfSight)
        bool blocked = Physics.SphereCast(
            origin,
            bodyRadius,
            dirToPlayer,
            out RaycastHit hit,
            distance,
            obstacleMask
        );

        // 4. Línea de visión
        Gizmos.color = blocked ? Color.red : Color.green;
        Gizmos.DrawLine(origin, origin + dirToPlayer * distance);

        // 5. Punto del jugador
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(playerTarget.transform.position, 0.2f);
    }
}
