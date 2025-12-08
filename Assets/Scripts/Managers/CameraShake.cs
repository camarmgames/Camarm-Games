using UnityEngine;
using Cinemachine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance { get; private set; }

    private CinemachineBasicMultiChannelPerlin perlin;
    private float shakeTimer;
    private bool shaking = false;
    public bool powerUpBatido;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        CinemachineVirtualCamera vCam = GetComponent<CinemachineVirtualCamera>();
        perlin = vCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    private void Update()
    {
        if (!shaking) return;

        shakeTimer -= Time.deltaTime;

        if (shakeTimer <= 0f)
        {
            StopShakeImmediate();
        }
    }

    public void ShakeCamera(NoiseSettings noiseProfile, float intensity, float duration)
    {
        if(powerUpBatido) return;

        perlin.m_NoiseProfile = noiseProfile;
        perlin.m_AmplitudeGain = intensity;

        shakeTimer = duration;
        shaking = true;
    }

    private void StopShakeImmediate()
    {
        shaking = false;
        shakeTimer = 0f;

        if (perlin != null)
        {
            perlin.m_AmplitudeGain = 0f; 
        }
    }
}
