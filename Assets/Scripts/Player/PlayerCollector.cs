using UnityEngine;

public class PlayerCollector : MonoBehaviour
{
    public Sprite inventorySprite;
    private void OnTriggerEnter(Collider other)
    {
        Collectable collectable;
        if (other != null)
        {
            collectable = other.GetComponent<Collectable>();
            if (collectable != null)
            {
                // TODO: LLAMAR AL INVENTARIO.
                Destroy(collectable.gameObject);
            }
        }
    }
}
