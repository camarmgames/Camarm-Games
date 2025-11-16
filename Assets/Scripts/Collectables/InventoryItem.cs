using System;
using UnityEngine;

[Serializable]
public class InventoryItem
{
    public Sprite sprite;
    [NonSerialized] public System.Action execute;
    public InventoryItem(Sprite sprite, System.Action execute)
    {
        this.sprite = sprite;
        this.execute = execute;
    }

    public void Use()
    {
        execute?.Invoke();
    }
}
