using BehaviourAPI.Core;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class GomiMagoAppearance: MonoBehaviour
{
    [Header("GomiMagoAppearance Settings")]
    [SerializeField] private GameObject gomiMagoPrefab;
    [SerializeField] private float spawnDelay = 20f;
    [SerializeField] private float visibleTime = 30f;
    public AudioClip effect;

    [Header("Buff Settings")]
    [SerializeField] private float speedMultiplier = 1.5f;
    [SerializeField] private NavMeshAgent enemyAgent;

    [Header("Materials")]
    [SerializeField][Tooltip("Prefab of renderObject")]private GameObject renderPrefab;
    [SerializeField] private Material buffMaterial;
    [SerializeField] private Material standardMaterial;

    private bool hasAppeared = false;
    private bool hasDisappeared = false;

    private float timer = 0f;
    private bool isVisible = false;

    private float originalEnemySpeed = 0f;

    private cp_GomiNinja cp_GomiNinja;

    private void Start()
    {
        cp_GomiNinja = GetComponent<cp_GomiNinja>();
        enemyAgent = GetComponent<NavMeshAgent>();
        timer = 0f;
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if(!isVisible && timer >= spawnDelay)
        {
            SpawnMago();
        }

        if(isVisible && timer >= visibleTime)
        {
            RemoveMago();
        }
    }

    public void SpawnMago()
    {
        isVisible = true;
        timer = 0f;
        gomiMagoPrefab?.SetActive(true);

        hasAppeared = true;
        hasDisappeared = false;
        cp_GomiNinja.SetBTSeActivationPush();
        AudioManager.Instance.PlaySFX(effect);
    }

    public void RemoveMago()
    {
        isVisible = false;
        timer = 0f;
        gomiMagoPrefab?.SetActive(false);

        hasDisappeared = true;
        hasAppeared = false;
        cp_GomiNinja.SetBTStActivationPush();
    }

    public Status BuffEnemy()
    {
        originalEnemySpeed = enemyAgent.speed;
        enemyAgent.speed *= speedMultiplier;

        SetMaterial(buffMaterial);
        return Status.Success;
    }

    public Status RestoreEnemyToNormal()
    {
        enemyAgent.speed = originalEnemySpeed;
        SetMaterial(standardMaterial);
        return Status.Success;
    }

    public bool HasAppeared()
    {
        return hasAppeared;
    }

    public bool HasDisappeared()
    {
        return hasDisappeared;
    }

    private void SetMaterial(Material mat)
    {
        Transform secondChild = renderPrefab.transform.GetChild(1);
        SkinnedMeshRenderer r = secondChild.GetComponent<SkinnedMeshRenderer>();
        if (r != null)
            r.material = mat;
    }
}
