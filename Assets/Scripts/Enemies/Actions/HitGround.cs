using BehaviourAPI.Core;
using Cinemachine;
using System.Collections;
using UnityEngine;

public class HitGround:MonoBehaviour
{

    [Header("Ground Hit Settings")]
    public float hitRange = 5f;               // Hasta dónde destruye
    public float hitRadius = 2f;              // Anchura del golpe
    public LayerMask groundMask;              // Suelos a destruir/reemplazar
    public LayerMask obstacleMask;            // Cosas que bloquean el impacto
    public GameObject destroyedGroundPrefab;  // Prefab del suelo destruido
    public Transform tileGeneratorParent;
    public float hitGroundCooldown = 2f;
    public Animator animator;

    [Header("Shockwave Settings")]
    public float shockwaveRange = 10f;
    public float shockwaveSpeed = 20f;
    public float shockwaveRadius = 1f;
    public float shockwaveForce = 20f;

    [Header("Camera Shake")]
    public NoiseSettings shakeProfile;
    public float shakeIntensity = 2f;
    public float shakeDuration = 1f;

    private bool canHitGround = true;
    public void HitGroundStarted()
    {
        if (!canHitGround) return;

        animator.Play("HitGround");
    }

    public Status HitGroundUpdate()
    {
        if (!canHitGround) return Status.Success;

        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.85f)
            return Status.Running;

        // 1. Golpe al suelo => destrucción
        DestroyGroundForward();

        // 2. Temblor
        CameraShake.Instance.ShakeCamera(shakeProfile, shakeIntensity, shakeDuration);

        // 3. Ondas expansivas
        StartCoroutine(Shockwave());

        StartCoroutine(hitGroundCooldownRoutine());

        return Status.Success;
    }

    private void DestroyGroundForward()
    {
        Vector3 origin = transform.position + Vector3.up * 0.5f;
        Vector3 dir = transform.forward;

        RaycastHit[] hits = Physics.SphereCastAll(origin, hitRadius, dir, hitRange, groundMask);

        foreach (RaycastHit h in hits)
        {
            // 1. Comprobar si hay obstáculo entre enemigo y suelo
            if (Physics.Raycast(origin, (h.point - origin).normalized, out RaycastHit block, hitRange, obstacleMask))
            {
                Debug.DrawLine(origin, block.point, Color.red, 1f);
                continue; // NO destruye si hay obstáculo
            }

            Debug.DrawLine(origin, h.point, Color.green, 1f);

            // 2. Reemplazar prefab
            ReplaceGround(h.collider.gameObject, h.point);
        }
    }

    private void ReplaceGround(GameObject oldGround, Vector3 pos)
    {
        Vector3 spawnPos = oldGround.transform.position;
        Quaternion spawnRot = oldGround.transform.rotation;

        GameObject.Destroy(oldGround);

        if (destroyedGroundPrefab != null)
        {
            GameObject tile = Instantiate(destroyedGroundPrefab, spawnPos, spawnRot, tileGeneratorParent);
            tile.transform.localScale = Vector3.one;
        }
    }

    private IEnumerator Shockwave()
    {
        float currentDistance = 0f;

        while (currentDistance < shockwaveRange)
        {
            currentDistance += shockwaveSpeed * Time.deltaTime;

            // Detectar objetos impactados por la onda
            Collider[] cols = Physics.OverlapSphere(
                transform.position + transform.forward * currentDistance,
                shockwaveRadius
            );

            foreach (Collider c in cols)
            {
                if (c.attachedRigidbody != null)
                {
                    Vector3 dir = (c.transform.position - transform.position).normalized;

                    // Comprobar si la onda está bloqueada
                    if (!Physics.Raycast(transform.position, dir, out RaycastHit hit, currentDistance, obstacleMask))
                    {
                        c.attachedRigidbody.AddForce(dir * shockwaveForce, ForceMode.Impulse);
                    }
                }
            }

            yield return null;
        }
    }

    private IEnumerator hitGroundCooldownRoutine()
    {
        canHitGround = false;

        yield return new WaitForSeconds(hitGroundCooldown);

        canHitGround = true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + transform.forward * hitRange, hitRadius);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, shockwaveRange);

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * hitRange);
    }
}
