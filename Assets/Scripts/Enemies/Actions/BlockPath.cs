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

    private bool isBlockingPath = false;
    private NavMeshAgent agent;

    

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            ABlockPath(pointsBlock);
        }
    }

    public void ABlockPath(List<Transform> posiblesPaths)
    {
        if (isBlockingPath || posiblesPaths == null || posiblesPaths.Count == 0) return;

        Transform target = null;
        float minDist = Mathf.Infinity;

        foreach (var path in posiblesPaths) {
            float dist = Vector3.Distance(transform.position, path.position);
            if ((dist < minDist))
            {
                minDist = dist;
                target = path;
            }
        }
        if (target != null) { 
            StartCoroutine(MoveAndBlock(target));
        }
    }

    private IEnumerator MoveAndBlock(Transform target)
    {
        isBlockingPath = true;
        Debug.Log($"{name}: Dirigiendose a bloquear el camino en {target.name}");

        if(agent && NavMesh.SamplePosition(target.position, out NavMeshHit hit, 2.0f, NavMesh.AllAreas)){
            Vector3 dirToTarget = (hit.position - transform.position).normalized;
            Vector3 adjustedDestination = hit.position - dirToTarget * (agent.radius + 1.5f);
            agent.SetDestination(adjustedDestination);
        }

        while(Vector3.Distance(transform.position, target.position) > blockRadius)
        {
            yield return null;
        }

        Debug.Log($"{name}: Bloqueando el camino...");
        yield return new WaitForSeconds(buildTime);

        if (wallPrefab != null) {
            Instantiate(wallPrefab, target.position, transform.rotation);
            Debug.Log($"{name}: Camino boqueado en {target.name}");
        }

        isBlockingPath = false;
    }
}
