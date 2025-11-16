using UnityEngine;

public abstract class Collectable : MonoBehaviour
{
    public Sprite inventory;

    public abstract void Execute();

    public InventoryItem ToInventoryItem()
    {
        return new InventoryItem(inventory, Execute);
    }

}
