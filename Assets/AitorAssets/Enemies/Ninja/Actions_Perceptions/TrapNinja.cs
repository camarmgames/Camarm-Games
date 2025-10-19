using System.Collections;
using UnityEngine;

public class TrapNinja: MonoBehaviour
{
    [Header("Configuration of trap")]
    public float timingStuck = 5f;
    public bool temporal = false;
    public float lifeTime = 5f;

    private float initialPlayerSpeed;

    private void Start()
    {
        initialPlayerSpeed = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().initialSpeed;

        if (temporal)
            Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(StickEffect(other));
        }
    }

    IEnumerator StickEffect(Collider target)
    {
        PlayerMovement playerMovement = target.GetComponent<PlayerMovement>();

        float originalSpeed = playerMovement.moveSpeed;


        if (initialPlayerSpeed == originalSpeed)
        {
            playerMovement.moveSpeed *= 0.4f;

            yield return new WaitForSeconds(timingStuck);

            playerMovement.moveSpeed = originalSpeed;
            Destroy(gameObject);
        }
    }
}
