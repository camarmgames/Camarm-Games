
using BehaviourAPI.Core;
using Cinemachine.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class Track: MonoBehaviour
{
    [Header("Detection Settings")]
    [SerializeField] private LayerMask footprintMask;
    [SerializeField] private float followSpeed = 3.5f;
    [SerializeField] private Animator animator;

    [Header("Parameters of vision")]
    [SerializeField, Tooltip("Distance the enemy can see to")]
    private float viewRadius = 10f;
    [UnityEngine.Range(0, 360)]
    [SerializeField, Tooltip("Angle of vision of the enemy")]
    private float viewAngle = 90f;
    [SerializeField, Tooltip("Layer of the objects the enemy can not see through them")]
    private LayerMask obstacleMask;

    [Header("Debug")]
    [SerializeField] private bool showGizmos = true;

    private NavMeshAgent agent;
    private bool isFollowingTrail = false;

    private Footprint currentTargetFootprint;
    private int lastSeenFootprintID = -1;


    private Coroutine followFootprintCoroutine;
    private Coroutine inspectAreaCoroutine;
    public bool beforeShowPrint = false;
    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent != null)
            agent.speed = followSpeed;
    }

    public bool DetectFootprint()
    {
        Collider[] footprints = Physics.OverlapSphere(transform.position, viewRadius, footprintMask);
        if (footprints.Length == 0) return false;
        List<Footprint> visibleFootprints = new List<Footprint>();
        foreach (var f in footprints)
        {
            Vector3 dirToFootprint = (f.transform.position - transform.position).normalized;
            float distToFootprint = Vector3.Distance(transform.position, f.transform.position);

            if (Vector3.Angle(transform.forward, dirToFootprint) < viewAngle / 2)
            {
                if (!Physics.Raycast(transform.position + Vector3.up * 1.5f, dirToFootprint, distToFootprint, obstacleMask))
                {
                    visibleFootprints.Add(f.GetComponent<Footprint>());
                }
            }
        }
        if (visibleFootprints.Count == 0) return false;
        visibleFootprints.Sort((a, b) => a.footprintID.CompareTo(b.footprintID));
        foreach (var fp in visibleFootprints)
            if (fp.footprintID > lastSeenFootprintID)
                return true;
        return false;
    }

    public bool HasTrack()
    {
        return followFootprintCoroutine == null;
    }

    public bool BeforeShowPrint()
    {
        return beforeShowPrint;
    }

    public void StopCoroutines()
    {
        if(followFootprintCoroutine != null || inspectAreaCoroutine != null)
            agent.isStopped = true;

        if (followFootprintCoroutine != null)
        {
            StopCoroutine(followFootprintCoroutine);
            followFootprintCoroutine = null;
        }
            
        if(inspectAreaCoroutine != null)
        {
            StopCoroutine(inspectAreaCoroutine);
            inspectAreaCoroutine = null;
        }
        
    }
    public Status Detect()
    {
        if(followFootprintCoroutine == null)
            TryDetectFootprints();

        return Status.Success;
    }
    public bool TryDetectFootprints()
    {
        Collider[] footprints = Physics.OverlapSphere(transform.position, viewRadius, footprintMask);
        if(footprints.Length == 0) return false;
        List<Footprint> visibleFootprints = new List<Footprint>();
        foreach (var f in footprints)
        {
            Vector3 dirToFootprint = (f.transform.position - transform.position).normalized;
            float distToFootprint = Vector3.Distance(transform.position, f.transform.position);

            if(Vector3.Angle(transform.forward, dirToFootprint) < viewAngle / 2)
            {
                if(!Physics.Raycast(transform.position + Vector3.up * 1.5f, dirToFootprint, distToFootprint, obstacleMask))
                {
                    visibleFootprints.Add(f.GetComponent<Footprint>());
                }
            }
        }
        if (visibleFootprints.Count == 0) return false;
        visibleFootprints.Sort((a,b) => a.footprintID.CompareTo(b.footprintID));
        foreach(var fp in visibleFootprints)
        {
            if(fp.footprintID > lastSeenFootprintID)
            {
                followFootprintCoroutine = StartCoroutine(FollowFootprint(fp));
                return true;
            }
        }

        return false;
    }

    private IEnumerator FollowFootprint(Footprint target)
    {
        beforeShowPrint = true;
        isFollowingTrail = true;
        currentTargetFootprint = target;
        agent.isStopped = false;

        if (animator != null)
            animator.SetBool("isWalking", isFollowingTrail);

        if (agent && NavMesh.SamplePosition(currentTargetFootprint.transform.position, out NavMeshHit hit, 2.0f, NavMesh.AllAreas))
        {
            agent.SetDestination(currentTargetFootprint.transform.position);
        }
        else
        {
            Debug.LogWarning($"{name}: Huella fuera del NavMesh o inaccesible");
            isFollowingTrail = false;
            yield break;
        }

        Vector3 lastPos = transform.position;
        float stuckTimer = 0f;


        while (Vector3.Distance(transform.position, currentTargetFootprint.transform.position) > 0.8f)
        {
            if (Vector3.Distance(transform.position, lastPos) < 0.05f)
                stuckTimer += Time.deltaTime;
            else
                stuckTimer = 0f;

            lastPos = transform.position;

            if(stuckTimer > 3f)
            {
                Debug.LogWarning($"{name}: Atascado, abortando seguimiento");
                
                break;
            }
            yield return null;
        }
        agent.ResetPath();
        //Debug.Log("Pongo la rotación como toca");
        

        lastSeenFootprintID = currentTargetFootprint.footprintID;

        if (DetectFootprint())
        {
            TryDetectFootprints();
        }
        else
        {
            isFollowingTrail = false;

            if (animator != null)
                animator.SetBool("isWalking", isFollowingTrail);

            yield return new WaitForSeconds(0.5f);
            
           
            inspectAreaCoroutine = StartCoroutine(InspectArea(currentTargetFootprint.transform.position));
        } 
    }



    private IEnumerator InspectArea(Vector3 lastPosition)
    {
        Debug.Log($"{name}: Inspeccionando la zona...");

        float inspectingTime = 5f;
        float timer = 0f;
        float angle = transform.eulerAngles.y;

        while(timer < inspectingTime)
        {
            transform.rotation = Quaternion.Euler(0, angle, 0);
            angle += 60f * Time.deltaTime;
            TryDetectFootprints();

            if (isFollowingTrail)
            {
                
                yield break;
            }

            timer += Time.deltaTime;
            yield return null;
        }

        
        Debug.Log($"{name}: no encontro mas huellas");
        followFootprintCoroutine = null;
        inspectAreaCoroutine = null;
    }

    private void OnDrawGizmosSelected()
{
    if (!showGizmos) return;

    // === DIBUJAR RADIO DE VISIÓN ===
    Gizmos.color = Color.cyan;
    Gizmos.DrawWireSphere(transform.position, viewRadius);

    // === DIBUJAR CONO DE VISIÓN ===
    Vector3 leftBoundary = DirectionFromAngle(-viewAngle / 2, false);
    Vector3 rightBoundary = DirectionFromAngle(viewAngle / 2, false);

    Gizmos.color = new Color(1f, 0.3f, 0.3f, 0.5f);
    Gizmos.DrawLine(transform.position, transform.position + leftBoundary * viewRadius);
    Gizmos.DrawLine(transform.position, transform.position + rightBoundary * viewRadius);

    // === DIBUJAR HUELLAS DETECTADAS (solo en editor) ===
#if UNITY_EDITOR
    var allFootprints = FindObjectsOfType<Footprint>();
    foreach (var f in allFootprints)
    {
        float dist = Vector3.Distance(transform.position, f.transform.position);
        if (dist <= viewRadius)
        {
            Vector3 dirToFootprint = (f.transform.position - transform.position).normalized;
            float angle = Vector3.Angle(transform.forward, dirToFootprint);

            // Si está dentro del ángulo, marcar en color distinto
            if (angle < viewAngle / 2)
            {
                Gizmos.color = Color.green;
            }
            else
            {
                Gizmos.color = Color.gray;
            }

            Gizmos.DrawSphere(f.transform.position, 0.1f);

            // Mostrar ID sobre la huella (solo editor)
            UnityEditor.Handles.color = Color.white;
            UnityEditor.Handles.Label(f.transform.position + Vector3.up * 0.1f, $"ID {f.footprintID}");
        }
    }
#endif

}

    Vector3 DirectionFromAngle(float angleInDegrees, bool global)
    {
        if (!global)
            angleInDegrees += transform.eulerAngles.y;

        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}
