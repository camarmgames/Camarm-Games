using BehaviourAPI.Core;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class LevelWizardController: MonoBehaviour
{
    public static LevelWizardController Instance;

    [Header("Wizard State")]
    public float totalStress { get; private set; }
    public float tiempoVisible { get; private set; }
    public float tiempoEscondido {  get; private set; }

    public float tiempoPersiguiendo { get; private set; }

    [Header("Wizard Settings")]
    [SerializeField] private GameObject gomiMagoPrefab;
    public float maxTiempoVisible = 6f;
    public float maxTiempoEscondido = 10f;
    public float maxTiempoPersiguiendo = 5f;
    
    public Transform player;
    public Transform lampsParent;

    public AudioClip effectAparicion;
    public AudioClip effectHechizo;

    private bool isVisible = false;
    private MagicLampDetector[] lamps;
    private List<EnemyStress> enemiesSpells = new List<EnemyStress>();

    private void Awake()
    {
        Instance = this;
        gomiMagoPrefab.SetActive(false);
        totalStress = 0;
        tiempoVisible = 0;
        tiempoEscondido = 0;
        tiempoPersiguiendo = 25;
        
        lamps = lampsParent.GetComponentsInChildren<MagicLampDetector>();
    }

    private void Update()
    {
        UpdateStress();
        UpdateTimers();
    }

    void UpdateStress()
    {
        totalStress = 0f;
        foreach (var lamp in lamps)
        {
            totalStress += lamp.lampStressPool;
        }

        totalStress = Mathf.Clamp(totalStress, 0f, 100f);
    }

    void UpdateTimers()
    {
        if (isVisible)
        {
            tiempoVisible += Time.deltaTime;
        }
        else
        {
            tiempoEscondido += Time.deltaTime;
        }
    }

    public void ActionAppear()
    {
        if (isVisible) return;

        // Factors
        tiempoEscondido = 0f;
        isVisible = true;
        StopFollowing();

        transform.position = player.position + Vector3.up * 2f;
        gomiMagoPrefab.SetActive(true);

        AudioManager.Instance.PlaySFXAtPosition(effectAparicion, transform.position, 1f, 1f);

        PlayerInventory.instance.transform.GetChild(6).GetComponent<ActiveItemBlink>().Activate(maxTiempoVisible);

        Debug.Log("MAGO APARECE");
    }

    public void ActionDisappear()
    {
       
        if (!isVisible) return;

        isVisible = false;
        tiempoVisible = 0f;

        gomiMagoPrefab.SetActive(false);

        tiempoPersiguiendo = 25;
        RestoreStressedEnemies();


        PlayerInventory.instance.transform.GetChild(6).GetComponent<ActiveItemBlink>().Desactivate();


        Debug.Log("MAGO DESAPARECE");
    }

    public void AffectStressedEnemies()
    {
        
        if (!isVisible) return;
        EnemyStress[] allEnemies = FindObjectsOfType<EnemyStress>();

        foreach(var e in allEnemies)
        {
            if(e.StressNormalized > 0.6f && !enemiesSpells.Contains(e))
            {
                Debug.Log($"Hechizo lanzado al enemigo {e.name}");
                e.ApplyBuff();
                enemiesSpells.Add(e);
            }
        }

        AudioManager.Instance.PlaySFXAtPosition(effectHechizo, transform.position, 1f, 1f);
    }

    void RestoreStressedEnemies()
    {
        EnemyStress[] allEnemies = FindObjectsOfType<EnemyStress>();

        foreach (var e in allEnemies)
        {
            if (enemiesSpells.Contains(e))
            {
                Debug.Log($"Quitando Hechizo lanzado al enemigo {e.name}");
                e.RestoreNormalState();
            }
        }
        enemiesSpells.Clear();
    }

    public void ActionFollowPlayer()
    {
        
        if (!isVisible) return;

        tiempoPersiguiendo += Time.deltaTime;

        Vector3 target = player.position + Vector3.up * 3f;

        transform.position = Vector3.Lerp(transform.position, target, Time.deltaTime * 2f);
    }

    public void StopFollowing()
    {
        tiempoPersiguiendo = 0f;
    }

    public Status onUpdate()
    {
        return Status.Success;
    }

    public void Nothing()
    {

    }

    #region Normalized Factors (0–1)

    public float Stress01 =>
        Mathf.Clamp01(totalStress / 100f);

    public float TiempoVisible01 =>
        Mathf.Clamp01(tiempoVisible / maxTiempoVisible);

    public float TiempoEscondido01 =>
        Mathf.Clamp01(tiempoEscondido / maxTiempoEscondido);

    public float TiempoPersiguiendo01 =>
        Mathf.Clamp01(tiempoPersiguiendo / maxTiempoPersiguiendo);

    #endregion
}
