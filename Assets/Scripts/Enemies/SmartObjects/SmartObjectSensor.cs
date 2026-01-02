using UnityEngine;
using BehaviourAPI.SmartObjects;
using BehaviourAPI.UnityToolkit;
using System.Collections.Generic;
using System.Collections;

public class SmartObjectSensor: MonoBehaviour
{
    [Header("Detection")]
    [SerializeField] float viewRadius = 10f;
    [SerializeField, Range(0, 360)] float viewAngle = 120f;

    [Header("Layers")]
    [SerializeField] LayerMask smartObjectMask;
    [SerializeField] LayerMask obstacleMask;

    [Header("Scan")]
    [SerializeField] float scanInterval = 0.2f;
    [SerializeField] float memoryDuration = 2f; 

    List<SmartObject> visibleSmartObjects = new List<SmartObject>();

    public bool hasSmartObject;
    public bool hasSmartObjectInHand;
    public bool canSeeSmartObjects;

    float lastSeenTime;
    float nextScanTime;

    public float SmartObjectSignal()
    {
        if (!hasSmartObjectInHand)
        {
            return hasSmartObject ? 1f : 0f;
        }
        else
        {
            return 1f;
        }
        
    }
        


    void Update()
    {
        if (!canSeeSmartObjects)
        {
            // Escaneo discreto
            if (Time.time >= nextScanTime)
            {
                nextScanTime = Time.time + scanInterval;
                Scan();
            }

            // Memoria temporal (NO parpadea)
            if (hasSmartObject && Time.time - lastSeenTime > memoryDuration)
            {
                hasSmartObject = false;
                visibleSmartObjects.Clear();
            }
        }
        
    }

    void Scan()
    {
        Collider[] targets = Physics.OverlapSphere(
            transform.position,
            viewRadius,
            smartObjectMask
        );

        foreach (var col in targets)
        {
            SmartObject smartObject = col.GetComponentInParent<SmartObject>();

            if (smartObject == null)
                continue;

            Vector3 dirToTarget =
                (smartObject.transform.position - transform.position).normalized;

            // Ángulo de visión
            if (Vector3.Angle(transform.forward, dirToTarget) > viewAngle * 0.5f)
                continue;

            float distance =
                Vector3.Distance(transform.position, smartObject.transform.position);

            // Línea de visión
            if (Physics.Raycast(
                transform.position + Vector3.up * 1.5f,
                dirToTarget,
                distance,
                obstacleMask))
                continue;


            lastSeenTime = Time.time;
            hasSmartObject = true;

            if (!visibleSmartObjects.Contains(smartObject))
                visibleSmartObjects.Add(smartObject);
        }
    }

    public IEnumerator BlockVisionCoroutine(float duration)
    {
        canSeeSmartObjects = true;

        hasSmartObject = false;
        visibleSmartObjects.Clear();

        yield return new WaitForSeconds(duration);

        canSeeSmartObjects = false;
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, viewRadius);

        Vector3 leftBoundary =
            Quaternion.Euler(0f, -viewAngle * 0.5f, 0f) * transform.forward;
        Vector3 rightBoundary =
            Quaternion.Euler(0f, viewAngle * 0.5f, 0f) * transform.forward;

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, transform.position + leftBoundary * viewRadius);
        Gizmos.DrawLine(transform.position, transform.position + rightBoundary * viewRadius);

        Gizmos.color = Color.green;
        foreach (var so in visibleSmartObjects)
        {
            if (so != null)
                Gizmos.DrawLine(transform.position, so.transform.position);
        }
    }
#endif
}
