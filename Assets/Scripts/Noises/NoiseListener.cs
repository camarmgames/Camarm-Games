using UnityEngine;

public class NoiseListener: MonoBehaviour
{
    [Header("Hearing Settings"), Tooltip("Minimum distance to consider it hear some sounds")]
    public float hearingThreshold = 0.5f;

    [Tooltip("Time that need to forget the sound")]
    public float forgetTime = 3f;

    [Header("Debug")]
    [SerializeField, Tooltip("Message console")]
    private bool debugLog;

    private float forgetTimer;
    private Vector3 lastHeardPosition;
    private NoiseType lastHeardNoise;

    private Transform playerT;
    private DetectPlayer detectPlayer;
    private Investigation investigation;

    private void Start()
    {
        detectPlayer = GetComponent<DetectPlayer>();
        investigation = GetComponent<Investigation>();
        PlayerMovement player = FindFirstObjectByType<PlayerMovement>();
        if (player != null)
            playerT = player.transform;
    }

    private void Update()
    {
        if(forgetTimer > 0)
        {
            forgetTimer -= Time.deltaTime;
        }
        else
        {
            lastHeardNoise = null;
        }
    }

    public void OnNoiseHeard(Vector3 position, NoiseType noise)
    {
        if(hearingThreshold == 0)
        {
            if (debugLog)
                Debug.Log("ESTOY SORDO");

            return;
        } 
        float distance = Vector3.Distance(transform.position, position);

        if (distance <= noise.radius * noise.intensity) { 
            lastHeardPosition = position;
            lastHeardNoise = noise;
            forgetTimer = forgetTime;
            if(debugLog)
                Debug.Log($"{name} escucho un ruido {noise.name} en {position}");
        }
    }

    public bool HighNoise()
    {
        if(lastHeardNoise != null && lastHeardNoise.intensity == 1)
        {
            if (debugLog)
                Debug.Log("Sonido fuerte");

            TeleportBehindPlayer teleportEnemy = GetComponent<TeleportBehindPlayer>();
            if (teleportEnemy != null)
                teleportEnemy.player = playerT;
            return true;
        }
            
        return false;
    }

    public bool LightNoise()
    {
        detectPlayer.PDetectPlayer();
        if (detectPlayer.IsInstantSuspicious())
        {
            if (investigation != null)
                investigation.pointToInvestigateArea = playerT.position;

            if (debugLog)
                Debug.Log("Vi algo sospechoso");

            return true;
        }else if ((lastHeardNoise != null && lastHeardNoise.intensity == 0.5))
        {
            if (investigation != null)
                investigation.pointToInvestigateArea = lastHeardPosition;

            if (debugLog)
                Debug.Log("Sonido leve");
            return true;
        }

        if(detectPlayer.IsPlayerDetected())
            return true;


        return false;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (lastHeardNoise != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, lastHeardPosition);
            Gizmos.DrawSphere(lastHeardPosition, 0.2f);
        }
    }
#endif
}
