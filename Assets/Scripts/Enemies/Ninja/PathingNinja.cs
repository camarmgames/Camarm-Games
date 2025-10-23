using System.Collections.Generic;
using UnityEngine;

public class PathingNinja: MonoBehaviour
{
    [Header("Configuration of pathing")]
    [Tooltip("Enemy speed")]
    public float speed;
    [Tooltip("Positions that have to follow")]
    public List<Transform> positions;
    [Tooltip("The distance the agent must be from one point to go to the next")]
    public float distanceThreshold;
    [Tooltip("The trap is going to put")]
    public GameObject trapPrefab;
    [Tooltip("Number of trap it can put")]
    public int trapsAvailable;

    [Header("Parameters of vision")]
    public float viewRadius = 10f;
    [Range(0, 360)]
    public float viewAngle = 90f;
    public LayerMask playerMask;
    public LayerMask obstacleMask;

    [Header("References of projectile")]
    public Transform firePosition;
    public GameObject proyectilePrefab;
    public float launchForce = 15f;

    [Header("Debug")]
    public bool playerVisible;


    private Transform player;

    int currentTargetPosId = 0;
    private int trapCount = 0;
    private Vector3 target;

    bool isMoving;

    bool changeLocation = false;


    public void CancelMove()
    {
        target = Vector3.zero;
        isMoving = false;
    }

    public bool IsNotMoving()
    {
        return !isMoving;
    }

    public bool CanMove(Vector3 targetPos)
    {
        return true;
    }

    public Vector3 GetTarget()
    {
        return target;
    }

    public bool HasArrived()
    {
        return Vector3.Distance(transform.position, target) < distanceThreshold;
    }

    public void MoveInstant(Vector3 targetPos, Quaternion targetRot = default)
    {
        transform.SetLocalPositionAndRotation(targetPos, targetRot);
    }

    public void SetTarget(Vector3 targetPos)
    {
        target = targetPos;
        isMoving = true;
    }

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (isMoving)
        {
            transform.LookAt(target);
            transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);


            if (HasArrived())
            {
                currentTargetPosId++;

                if (currentTargetPosId >= positions.Count)
                {
                    currentTargetPosId = 0;
                    CancelMove();
                }
                else
                {
                    CancelMove();
                }
            }
        }
    }


    public bool checkChangeLocation()
    {
        return changeLocation;
    }

    private bool CanPlaceTrap(Vector3 position, float checkRadius)
    {
        Collider[] overlaps = Physics.OverlapSphere(position, checkRadius);
        foreach (Collider collision in overlaps)
        {
            if(collision.gameObject.GetComponent<TrapNinja>() != null)
            {
                return false;
            }
        }   
        return true;
    }

    

    
    public bool DetectPlayer()
    {
        playerVisible = false;

        // Comprobar si el jugador esta dentro del radio
        Collider[] targets = Physics.OverlapSphere(transform.position, viewRadius, playerMask);

        if (targets.Length > 0)
        {
            Transform target = targets[0].transform;
            Vector3 dirtToPlayer = (target.position - transform.position).normalized;

            // Comprobar si esta dentro del angulo de vision
            if(Vector3.Angle(transform.forward, dirtToPlayer) < viewAngle / 2)
            {
                float distanceToPlayer = Vector3.Distance(transform.position, target.position);

                // Comrpobar si hay linea de vision (sin obstaculos en medio)
                if(!Physics.Raycast(transform.position + Vector3.up * 1.5f, dirtToPlayer, distanceToPlayer, obstacleMask))
                {
                    playerVisible = true;
                    Debug.Log("Jugador detectado");
                    return true;
                }
            }

        }

        return false;
    }

    public bool NoDetectPlayer()
    {
        // Comprobar si el jugador esta dentro del radio
        Collider[] targets = Physics.OverlapSphere(transform.position, viewRadius, playerMask);

        if (targets.Length > 0)
        {
            Transform target = targets[0].transform;
            Vector3 dirtToPlayer = (target.position - transform.position).normalized;

            // Comprobar si esta dentro del angulo de vision
            if (Vector3.Angle(transform.forward, dirtToPlayer) < viewAngle / 2)
            {
                float distanceToPlayer = Vector3.Distance(transform.position, target.position);

                // Comrpobar si hay linea de vision (sin obstaculos en medio)
                if (!Physics.Raycast(transform.position + Vector3.up * 1.5f, dirtToPlayer, distanceToPlayer, obstacleMask))
                {
                    return false;
                }
            }

        }
        Debug.Log("Jugador NO detectado");
        return true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, viewRadius);

        Vector3 leftBoundary = DirectionFromAngle(-viewAngle / 2, false);
        Vector3 rightBoundary = DirectionFromAngle(viewAngle / 2, false);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + leftBoundary * viewRadius);
        Gizmos.DrawLine(transform.position, transform.position + rightBoundary * viewRadius);

        if (playerVisible && player != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, player.position);
        }

    }

    Vector3 DirectionFromAngle(float angleInDegrees, bool global)
    {
        if (!global)
            angleInDegrees += transform.eulerAngles.y;

        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }




    #region Actions
    public void newLocation()
    {
        changeLocation = false;
    }
    public void ChangeLocation()
    {
        if (NoDetectPlayer())
        {
            changeLocation = true;
            currentTargetPosId = 0;
            CancelMove();
        }
    }
    public void PathingMove()
    {
        if (positions.Count > 0)
        {
            SetTarget(positions[currentTargetPosId].position);
        }
    }
    public void PutTrap()
    {
        if ((trapCount != trapsAvailable))
        {
            Debug.Log("Maximo de trampas alcanzado");
        }

        Vector3 trapPosition = new Vector3(GetComponent<Transform>().position.x, trapPrefab.GetComponent<Transform>().position.y, GetComponent<Transform>().position.z);

        if (CanPlaceTrap(trapPosition, 1.0f))
        {
            trapCount++;
            Instantiate(trapPrefab, trapPosition, Quaternion.identity);
        }
    }

    public void Alert()
    {
        Debug.Log("ALERTA JUGADOR");
    }

    public void Attack()
    {
        Vector3 firePoint = new Vector3(transform.position.x, firePosition.position.y, transform.position.z);
        GameObject proyectile = Instantiate(proyectilePrefab, firePoint, Quaternion.identity);

        // Calcula direccion hacia el jugador
        Vector3 impactPosition = new Vector3(player.position.x, player.position.y +1.5f, player.position.z);
        Vector3 direction = (impactPosition - firePoint).normalized;

        // Lanza el proyectil con fuerza
        Rigidbody rb = proyectile.GetComponent<Rigidbody>();

        rb.AddForce(direction * launchForce, ForceMode.VelocityChange);

    }
    #endregion
}
