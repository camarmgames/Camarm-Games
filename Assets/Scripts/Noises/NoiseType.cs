using UnityEngine;

[CreateAssetMenu(fileName = "NoiseType", menuName = "Scriptable Objects/NoiseType")]
public class NoiseType : ScriptableObject
{
    [Header("Noise Settings"), Tooltip("Radius max where the sound can be hear")]
    public float radius = 5f;

    [Range(0, 1), Tooltip("Intensity of sound (1 = max, 0 = min)")]
    public float intensity = 1f;

    [Tooltip("Duration the sound is active")]
    public float duration = 1f;

    [Tooltip("Color of gizmo")]
    public Color gizmoColor = Color.yellow;

    [Tooltip("Sound of noise")]
    public AudioClip soundEffect;
}
