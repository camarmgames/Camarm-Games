using System;
using UnityEngine;

[Serializable]
public class InventoryItem
{
    public Sprite sprite;
    [NonSerialized] public Collectable collectable;
    public InventoryItem(Sprite sprite, Collectable collectable)
    {
        this.sprite = sprite;
        this.collectable = collectable;
    }

    public void Use(Action onFinish = null)
    {
        collectable.Execute(onFinish);
    }
}
