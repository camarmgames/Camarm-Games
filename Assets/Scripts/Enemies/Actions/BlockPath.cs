using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BlockPath: MonoBehaviour
{
    [Header("Blocking Settings")]
    [SerializeField] private GameObject wallPrefab;
    [SerializeField] private float blockRadius = 2f;
    [SerializeField] private float buildTime = 2f;
    [SerializeField] private List<Transform> pointsBlock;
    [SerializeField] private Animator animator;

    private NavMeshAgent agent;
    private Coroutine blockCoroutine;
    private Track trackScript;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        trackScript = GetComponent<Track>();
    }

    public bool IsBlockingPath()
    {
        return blockCoroutine == null;
    }

    public void StopCoroutine()
    {
        if (blockCoroutine != null)
        {
            StopCoroutine(blockCoroutine);
            blockCoroutine = null;
            agent.isStopped = true;
            trackScript.beforeShowPrint = false;
        }
    }

    public void ABlockPath()
    {
        if (blockCoroutine != null || pointsBlock == null || pointsBlock.Count == 0) return;

        Transform target = null;
        float minDist = Mathf.Infinity;

        foreach (var path in pointsBlock) {
            float dist = Vector3.Distance(transform.position, path.position);
            if ((dist < minDist))
            {
                minDist = dist;
                target = path;
            }
        }
        if (target != null) { 
            blockCoroutine = StartCoroutine(MoveAndBlock(target));
        }
    }
    private IEnumerator MoveAndBlock(Transform target)
    {
        Debug.Log($"{name}: Dirigiendose a bloquear el camino en {target.name}");

        if(agent && NavMesh.SamplePosition(target.position, out NavMeshHit hit, 2.0f, NavMesh.AllAreas)){
            Vector3 dirToTarget = (hit.position - transform.position).normalized;
            Vector3 adjustedDestination = hit.position - dirToTarget * (agent.radius + 1.5f);
            agent.isStopped = false;
            agent.SetDestination(adjustedDestination);
            if (animator != null)
                animator.SetBool("isWalking", true);
        }

        float stuckTimer = 0f;

        Vector3 lastPosition = transform.position;

        while (true)
        {
            float dist = Vector3.Distance(transform.position, target.position);

            if (Vector3.Distance(transform.position, lastPosition) < 0.05f)
                stuckTimer += Time.deltaTime;
            else
                stuckTimer = 0f;

            lastPosition = transform.position;

            // Si el enemigo lleva 3 segundos sin moverse
            if (stuckTimer > 3f)
            {
                Debug.LogWarning($"{name}: Atascado mientras intentaba bloquear. Reintentando con mayor radio");
                blockRadius += 1f;             // aumentamos radio
                stuckTimer = 0f;               // reseteamos
            }

            
            if (dist <= blockRadius)
                break;

            yield return null;
        }

        if (animator != null)
        {
            animator.SetBool("isWalking", false);
            animator.SetBool("isBlocking", true);
        }
            

        Debug.Log($"{name}: Bloqueando el camino...");
        yield return new WaitUntil(() =>
                animator.GetCurrentAnimatorStateInfo(0).IsName("Blocking"));

        yield return new WaitUntil(() =>
            animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);

        if (animator != null)
            animator.SetBool("isBlocking", false);

        if (wallPrefab != null) {
            Instantiate(wallPrefab, target.position, transform.rotation);
            Debug.Log($"{name}: Camino boqueado en {target.name}");
        }

        trackScript.beforeShowPrint = false;
        blockCoroutine = null;
    }
}
