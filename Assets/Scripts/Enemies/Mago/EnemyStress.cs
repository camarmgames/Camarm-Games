using UnityEngine;

public class EnemyStress: MonoBehaviour
{
    [Header("Stress Settings")]
    public float stress = 0f;
    public float maxStress = 100f;

    [Header("Materials")]
    [SerializeField][Tooltip("Prefab of renderObject")] private GameObject renderPrefab;
    [SerializeField] private Material buffMaterial;
    [SerializeField] private Material standardMaterial;

    private Renderer rend;
    private bool isBuffed = false;

    public float StressNormalized => stress / maxStress;

    void Awake()
    {
        SetMaterial(standardMaterial);
    }

    public void AddStress(float amount)
    {
        stress = Mathf.Clamp(stress + amount, 0f, maxStress);

    }

    // Reduce estrés
    public void RemoveStress(float amount)
    {
        stress = Mathf.Clamp(stress - amount, 0f, maxStress);

    }


    public void ApplyBuff()
    {
        if (isBuffed) return;

        isBuffed = true;

        SetMaterial(buffMaterial);

        // Aquí puedes activar tus efectos especiales
        GetComponent<IEnemyBuffable>()?.OnBuffApplied();
    }


    public void RestoreNormalState()
    {
        if (!isBuffed) return;

        isBuffed = false;

        SetMaterial(standardMaterial);
        stress = 0f;

        // Desactivar efectos
        GetComponent<IEnemyBuffable>()?.OnBuffRemoved();
    }

    private void SetMaterial(Material mat)
    {
        //Transform secondChild = renderPrefab.transform.GetChild(1);
        SkinnedMeshRenderer r = renderPrefab.GetComponent<SkinnedMeshRenderer>();
        if (r != null)
            r.material = mat;
    }
}
