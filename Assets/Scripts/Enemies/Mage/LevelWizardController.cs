using BehaviourAPI.Core;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class LevelWizardController: MonoBehaviour
{
    public static LevelWizardController Instance;

    [Header("Wizard Settings")]
    [SerializeField] private GameObject gomiMagoPrefab;
    public float spawnThreshold = 50f;
    public float timeVisible = 6f;
    public float spawnProbabilityPerSecond = 0.1f;
    
    public Transform player;
    public Transform lampsParent;
    

    public AudioClip effectAparicion;
    public AudioClip effectHechizo;

    private bool isVisible = false;
    private float visibleTimer = 0f;
    private float totalStress;
    private MagicLampDetector[] lamps;


    private void Awake()
    {
        Instance = this;
        gomiMagoPrefab.SetActive(false);

        
        lamps = lampsParent.GetComponentsInChildren<MagicLampDetector>();
    }

    private void Update()
    {
        CalculateStress();

        if (!isVisible)
        {
            TrySpawnWizard();
        }
        else
        {
            ManageWizardLife();
        }
    }

    void CalculateStress()
    {
        totalStress = 0;
        foreach (var lamp in lamps)
        {
            totalStress += lamp.lampStressPool;
        }
    }

    void TrySpawnWizard()
    {
        if (totalStress < spawnThreshold) return;

        float probability = (totalStress / 100f) * spawnProbabilityPerSecond;

        if (Random.value < probability * Time.deltaTime)
        {
            SpawnWizard();
        }
    }

    public void SpawnWizard()
    {
        isVisible = true;
        visibleTimer = timeVisible;

        transform.position = player.position + Vector3.up * 2f;
        gomiMagoPrefab.SetActive(true);

        AudioManager.Instance.PlaySFXAtPosition(effectAparicion, transform.position, 1f, 1f);

        PlayerInventory.instance.transform.GetChild(6).GetComponent<ActiveItemBlink>().Activate(visibleTimer);

        Debug.Log("MAGO APARECE");

        AffectStressedEnemies();
    }

    void ManageWizardLife()
    {
        visibleTimer -= Time.deltaTime;
        if (visibleTimer <= 0f)
        {
            gomiMagoPrefab.SetActive(false);
            RestoreStressedEnemies();
            isVisible = false;

        }
    }

    void AffectStressedEnemies()
    {
        EnemyStress[] allEnemies = FindObjectsOfType<EnemyStress>();

        foreach(var e in allEnemies)
        {
            if(e.StressNormalized > 0.6f)
            {
                Debug.Log($"Hechizo lanzado al enemigo {e.name}");
                e.ApplyBuff();
            }
        }
    }

    void RestoreStressedEnemies()
    {
        EnemyStress[] allEnemies = FindObjectsOfType<EnemyStress>();

        foreach (var e in allEnemies)
        {
            if (e.StressNormalized > 0.6f)
            {
                Debug.Log($"Hechizo lanzado al enemigo {e.name}");
                e.RestoreNormalState();
            }
        }
    }
}
