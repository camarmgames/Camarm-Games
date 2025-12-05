using UnityEngine;

public class AudioOcclusion: MonoBehaviour
{
    public Transform listener;
    public LayerMask obstacleMask;

    private AudioLowPassFilter lowPass;

    [Header("Cutoff Values")]
    public float normalCutoff = 20000f;
    public float occludedCutoff = 1000f;

    [Header("Speed")]
    public float transitionSpeed = 5f;

    private void Awake()
    {
        lowPass = GetComponent<AudioLowPassFilter>();
    }

    private void Update()
    {
        if (listener == null) return;

        bool blocked = Physics.Linecast(transform.position, listener.position, obstacleMask);

        float targetCutoff = blocked ? occludedCutoff : normalCutoff;

        lowPass.cutoffFrequency = Mathf.Lerp(lowPass.cutoffFrequency, targetCutoff, Time.deltaTime * transitionSpeed);
    }
}
