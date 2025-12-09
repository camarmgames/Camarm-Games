using System;
using UnityEngine;

public abstract class Collectable : MonoBehaviour
{
    public Sprite inventory;
    public float duration = 15f;

    public abstract void Execute(Action onFinish);

    public InventoryItem ToInventoryItem()
    {
        return new InventoryItem(inventory, this);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        Collectable collectable;
        if (other != null)
        {
            collectable = GetComponent<Collectable>();
            if (!PlayerInventory.instance.IsFull() && transform.childCount > 0)
            {
                PlayerInventory.instance.Add(collectable);
                Destroy(transform.GetChild(0).gameObject);
            }
        }
    }
}
