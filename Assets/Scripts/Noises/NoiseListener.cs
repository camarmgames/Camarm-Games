using UnityEngine;

public class NoiseListener: MonoBehaviour
{
    [Header("Hearing Settings"), Tooltip("Minimum distance to consider it hear some sounds")]
    public float hearingThreshold = 0.5f;

    [Tooltip("Time that need to forget the sound")]
    public float forgetTime = 3f;

    private float forgetTimer;
    private Vector3 lastHeardPosition;
    private NoiseType lastHeardNoise;

    private Transform playerT;

    private void Start()
    {
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

        HighNoise();
        LightNoise();
    }

    public void OnNoiseHeard(Vector3 position, NoiseType noise)
    {
        float distance = Vector3.Distance(transform.position, position);

        if (distance <= noise.radius * noise.intensity) { 
            lastHeardPosition = position;
            lastHeardNoise = noise;
            forgetTimer = forgetTime;

            Debug.Log($"{name} escucho un ruido {noise.name} en {position}");
        }
    }

    public bool HighNoise()
    {
        if(lastHeardNoise != null && lastHeardNoise.intensity == 1)
        {
            Debug.Log("Sonido fuerte");

            TeleportBehindPlayer teleportEnemy = GetComponent<TeleportBehindPlayer>();
            if (teleportEnemy != null)
                teleportEnemy.playerT = playerT;
            return true;
        }
            
        return false;
    }

    public bool LightNoise()
    {
        if(lastHeardNoise != null && lastHeardNoise.intensity == 0.5)
        {
            Debug.Log("Sonido leve");
            return true;
        }
            
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
