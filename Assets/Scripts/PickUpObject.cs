using UnityEngine;

public class PickUpObject : MonoBehaviour
{
    public GameObject showInUI;
    public GameObject doorOpen;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            showInUI.SetActive(true);
            doorOpen.SetActive(true);

            Destroy(gameObject);
        }
    }
}
