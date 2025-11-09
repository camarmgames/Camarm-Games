using System.Collections;
using UnityEngine;

public class NoiseEmitter : MonoBehaviour
{
    [Header("Enemy Layer"), Tooltip("Layer of the enemies hearing")]
    public LayerMask enemyMask;

    private bool isEmitting = false;
    private NoiseType noiseTypeG;
    public void EmitNoise(NoiseType noiseType)
    {
        if (noiseType == null || isEmitting)
            return;

        noiseTypeG = noiseType;

        StartCoroutine(EmitNoiseRoutine(noiseType));
    }

    private IEnumerator EmitNoiseRoutine(NoiseType noiseType)
    {
        isEmitting = true;

        // Buscar enemigos dentro del radio
        Collider[] listeners = Physics.OverlapSphere(transform.position, noiseType.radius, enemyMask);
        foreach(var listener in listeners)
        {
            // Listener
            NoiseListener hear = listener.GetComponent<NoiseListener>();
            if (hear != null)
                hear.OnNoiseHeard(transform.position, noiseType);
        }

        // Reproducir sonido 
        if(noiseType.soundEffect != null)
            AudioSource.PlayClipAtPoint(noiseType.soundEffect, transform.position);

        yield return new WaitForSeconds(noiseType.duration);
        isEmitting = false;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (noiseTypeG == null) return;
        Gizmos.color = noiseTypeG.gizmoColor;
        Gizmos.DrawWireSphere(transform.position, noiseTypeG.radius);
    }
#endif
}
