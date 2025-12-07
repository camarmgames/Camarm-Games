using System.Collections;
using UnityEngine;

public class MagicLampDetector: MonoBehaviour
{
    [Header("Detector Settings")]
    [SerializeField] private GameObject player;
    [SerializeField]
    [Tooltip("Offset of first time it lights.")]
    private float offset;
    [SerializeField]
    [Tooltip("Time the magic ball is on.")]
    private float timeLight;
    [SerializeField]
    [Tooltip("Time the magic ball is off. Not the first time.")]
    private float timeBetween;
    [SerializeField]
    [Tooltip("Time your controls are inverted")]
    private float timingTrigger = 5f;

    [SerializeField] private Light lampLight;

    public float detectionRadius = 10f;
    public LayerMask enemyMask;
    public LayerMask obstacleMask;

    public float lampStressPool = 0f;

    private float timeLeftOff;
    private bool isOn = false;
    
    private PlayerMovement movement;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        offset = Random.Range(0.0f, offset);
        timeLight = Random.Range(5.0f, timeLight);
        timeBetween = Random.Range(5.0f, timeBetween);
        timeLeftOff = offset;


        //player = GameObject.Find("PlayerMovement");
        if (player == null)
            Debug.LogWarning("No player has been found in the sceen.");
        else
            movement = player.GetComponent<PlayerMovement>();
        if (movement == null)
            Debug.LogWarning("No playerMovement script could be found.");
    }

    // Update is called once per frame
    void Update()
    {
        UpdateLampStress();

        if (timeLeftOff <= 0.0f)
            timeLeftOff = TriggerLightOn();
        else
            timeLeftOff -= Time.deltaTime;
        if (isOn && movement.bewitched != -1)
        {
            float dist = Vector3.Distance(player.transform.position, transform.position);

            if (dist < 4.0f)
            {
                if (!Physics.Raycast(transform.position,
                                     (player.transform.position - transform.position).normalized,
                                     dist,
                                     obstacleMask))
                {
                    StartCoroutine(TriggerSpell());
                }
            }
        }
    }

    IEnumerator TriggerSpell()
    {
        if (movement.bewitched != -1)
        {
            movement.TriggerSpell();

            yield return new WaitForSeconds(timingTrigger);

            movement.TriggerSpell();
        }
    }

    public float TriggerLightOn()
    {
        if (isOn == false)
        {
            //Entonces hay que encender.
            isOn = true;
            Color onColor;
            if (lampLight != null && ColorUtility.TryParseHtmlString("#EA00FF", out onColor))
                lampLight.color = onColor;
            return (timeLight);

        }
        else
        {
            //Entonces hay que apagar.
            isOn = false;
            Color offColor;
            if (lampLight != null && ColorUtility.TryParseHtmlString("#0FFF00", out offColor))
                lampLight.color = offColor;
            return (timeBetween);
        }
    }

    private Vector2 GetPosition2D(Vector3 vector)
    {
        return (new Vector2(vector.x, vector.z));
    }

    void UpdateLampStress()
    {
        lampStressPool = 0;

        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius, enemyMask);

        foreach (var hit in hits)
        {
            EnemyStress e = hit.GetComponent<EnemyStress>();
            if (e != null)
            {
                lampStressPool += e.StressNormalized * 100f;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.8f, 0.2f, 0.4f);
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
