using BehaviourAPI.Core;
using Cinemachine;
using System.Collections;
using System.Collections.Generic;
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
    public AudioClip effect;

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
    private EnemyStress enemyStress;
    private TileGenerator tileGenerator;

    private void Start()
    {
        tileGenerator = tileGeneratorParent.gameObject.GetComponent<TileGenerator>();
        enemyStress = GetComponent<EnemyStress>();
    }
    public void HitGroundStarted()
    {
        if (!canHitGround) return;

        animator.Play("HitGround");

        enemyStress?.AddStress(8);

    }

    public Status HitGroundUpdate()
    {
        if (!canHitGround) return Status.Success;

        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.7f)
            return Status.Running;

        // 1. Golpe al suelo => destrucción
        DestroyGroundForward();

        // 2. Temblor
        CameraShake.Instance.ShakeCamera(shakeProfile, shakeIntensity, shakeDuration);

        // 3. Ondas expansivas
        StartCoroutine(Shockwave());

        AudioManager.Instance.PlaySFXAtPosition(effect, transform.position, 2f, 1f);

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

        if (tileGenerator != null)
            tileGenerator.RemoveTile(oldGround);

        Destroy(oldGround);

        if (destroyedGroundPrefab != null)
        {
            GameObject tile = Instantiate(destroyedGroundPrefab, spawnPos, spawnRot, tileGeneratorParent);
            tile.transform.localScale = Vector3.one;

            if(tileGenerator != null)
                tileGenerator.AddTile(tile);
        }
    }

    private HashSet<Rigidbody> pushedBodies = new HashSet<Rigidbody>();

    private IEnumerator Shockwave()
    {
        float currentDistance = 0f;
        pushedBodies.Clear();

        while (currentDistance < shockwaveRange)
        {
            currentDistance += shockwaveSpeed * Time.deltaTime;

            Vector3 wavePos = transform.position + transform.forward * currentDistance;

            Collider[] cols = Physics.OverlapSphere(wavePos, shockwaveRadius);

            foreach (Collider c in cols)
            {
                Rigidbody rb = c.attachedRigidbody;
                if (rb == null) continue;

                // Solo empujar una vez
                if (pushedBodies.Contains(rb)) continue;

                // No empujar si está detrás de una pared
                Vector3 dirToRB = (rb.transform.position - wavePos).normalized;

                if (Physics.Raycast(wavePos, dirToRB, out RaycastHit hit, shockwaveRadius, obstacleMask))
                    continue;

                // Fuerza decreciente por distancia
                float distanceFactor = 1f - (currentDistance / shockwaveRange);
                float finalForce = shockwaveForce * distanceFactor;

                rb.AddForce(dirToRB * finalForce, ForceMode.Impulse);

                pushedBodies.Add(rb);
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
