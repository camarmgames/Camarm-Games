using UnityEngine;

public class MagicBallController : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Offset of first time it lights.")]
    private float offset;
    [SerializeField]
    [Tooltip("Time the magic ball is on.")]
    private float timeLight;
    [SerializeField]
    [Tooltip("Time the magic ball is off. Not the first time.")]
    private float timeBetween;

    private float timeLeftOff;
    private bool isOn = false;
    private GameObject player;
    private PlayerMovement movement;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GetComponent<Renderer>().material.color = Color.blue;
        offset = Random.Range(0.0f, 3.0f);
        timeLight = Random.Range(5.0f, 10.0f);
        timeBetween = Random.Range(5.0f, 8.0f);
        timeLeftOff = offset;
        player = GameObject.Find("PlayerMovement");
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
        if (timeLeftOff <= 0.0f)
            timeLeftOff = TriggerLightOn();
        else
            timeLeftOff -= Time.deltaTime;
        if (Vector2.Distance(GetPosition2D(player.transform.position), GetPosition2D(transform.position)) < 4.0f && isOn && movement.bewitched != -1)
        {
            Debug.Log("Player is inside the magic ball.");
            movement.TriggerSpell();
        }
    }

    public float TriggerLightOn()
    {
        if (isOn == false)
        {
            //Entonces hay que encender.
            isOn = true;
            GetComponent<Renderer>().material.color = Color.yellow;
            return (timeLight);

        }
        else
        {
            //Entonces hay que apagar.
            isOn = false;
            GetComponent<Renderer>().material.color = Color.blue;
            return (timeBetween);
        }
    }

    private Vector2 GetPosition2D(Vector3 vector)
    {
        return (new Vector2(vector.x, vector.z));
    }
}
