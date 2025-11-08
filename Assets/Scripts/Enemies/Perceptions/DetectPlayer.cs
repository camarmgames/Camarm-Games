using System.Collections;
using UnityEngine;

public class DetectPlayer: MonoBehaviour
{
    private Transform player;
    private bool isDetecting;
    private bool detectionConfirmed;
    private Coroutine loseSightCoroutine;

    [Header("Parameters of vision")]
    [SerializeField, Tooltip("Distance the enemy can see to")]
    private float viewRadius = 10f;
    [Range(0, 360)]
    [SerializeField, Tooltip("Angle of vision of the enemy")]
    private float viewAngle = 90f;
    [SerializeField, Tooltip("Layer of the objective, he have to identify")]
    private LayerMask playerMask;
    [SerializeField, Tooltip("Layer of the objects the enemy can not see through them")]
    private LayerMask obstacleMask;

    [Header("Detection timing")]
    [SerializeField, Tooltip("Seconds before confirming detection")]
    private float detectionDelay = 2f;
    [SerializeField, Tooltip("Seconds the enemy keeps the player detected after losing sight")]
    private float loseSightDelay = 3f;

    [Header("Debug")]
    [SerializeField, Tooltip("Gizmo player visible")]
    private bool playerVisible;
    [SerializeField, Tooltip("Message console")]
    private bool debugLog;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        bool dP = PDetectPlayer();
        bool nP = PNoDetectPlayer();
    }

    public bool PDetectPlayer()
    {
        if (CanSeePlayer())
        {
            if (!detectionConfirmed && !isDetecting) {
                StartCoroutine(ConfirmDetectionAfterDelay());
            }

            // Si ya lo detecta y esta contando para perderlo, reinicia
            if(loseSightCoroutine != null)
            {
                if (debugLog)
                    Debug.Log("Reseteo de perder vista");

                StopCoroutine(loseSightCoroutine);
                loseSightCoroutine = null;
            }
            return detectionConfirmed;
        }
        else
        {
            // Si no lo ve, pero estaba detectado, empieza el conteo
            if(detectionConfirmed && loseSightCoroutine == null)
            {
                loseSightCoroutine = StartCoroutine(LoseSightAfterDelay());
            }

            return detectionConfirmed;
        }
    }

    private bool CanSeePlayer()
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
                    return true;
                }
            }

        }
        return false;
    }

    private IEnumerator ConfirmDetectionAfterDelay()
    {
        isDetecting = true;
        if (debugLog)
            Debug.Log("Detectando jugador...");

        yield return new WaitForSeconds(detectionDelay);

        detectionConfirmed = true;
        playerVisible = true;
        isDetecting = false;
        if (debugLog)
            Debug.Log("Jugador detectado tras retraso");
    }

    private IEnumerator LoseSightAfterDelay()
    {
        if (debugLog)
            Debug.Log("Jugador fuera de vista, iniciando conteo de pérdida...");

        yield return new WaitForSeconds(loseSightDelay);

        detectionConfirmed = false;
        playerVisible = false;
        if (debugLog)
            Debug.Log("Jugador perdido tras " + loseSightDelay + "s sin verlo");
    }

    public bool PNoDetectPlayer()
    {
        return !playerVisible;
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
}
