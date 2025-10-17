using System.Collections;
using UnityEngine;

public class Trap: MonoBehaviour
{
    [Header("Configuration of trap")]
    public float timingStuck = 5f;

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
        playerMovement.moveSpeed *= 0.4f;

        yield return new WaitForSeconds(timingStuck);

        playerMovement.moveSpeed = originalSpeed;
        Destroy(gameObject);
    }
}
