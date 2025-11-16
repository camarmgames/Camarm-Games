using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory instance;

    [SerializeField] public List<InventoryItem> objects = new List<InventoryItem>();

    const int INVENTORY_SIZE = 4;
    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    public void Clean()
    {
        objects.Clear();
    }

    public void Add(Collectable obj) { objects.Add(obj.ToInventoryItem()); }
    public void Remove(Collectable obj) { objects.Remove(obj.ToInventoryItem()); }

    public bool IsFull()
    {
        return (objects.Count == INVENTORY_SIZE);
    }

    public void Use(int index)
    {
        if (index < 0 || index >= objects.Count)
            return;
        Debug.Log("Pressed " + index);
        if (objects[index] != null)
        {
            objects[index].Use();
            objects.RemoveAt(index);
        }
    }
}
