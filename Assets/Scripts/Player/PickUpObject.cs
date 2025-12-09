using UnityEngine;

public class PickUpObject : MonoBehaviour
{
    public int showInUI;
    public GameObject doorOpen;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerInventory.instance.transform.GetChild(1).transform.GetChild(showInUI).gameObject.SetActive(true);
            doorOpen.SetActive(true);

            Destroy(gameObject);
        }
    }
}
