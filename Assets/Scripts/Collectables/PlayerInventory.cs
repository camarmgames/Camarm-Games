using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory instance;

    public List<InventoryItem> objects = new List<InventoryItem>();

    public GameObject slotsHolder;

    public Image[] images = new Image[INVENTORY_SIZE];
    public bool[] occupiedslots;

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

    public void Start()
    {
        for (int i = 0; i < INVENTORY_SIZE; i++)
        {
            Transform slot = slotsHolder.transform.GetChild(i);
            images[i] = slot.GetChild(0).GetComponent<Image>();
        }
        occupiedslots = new bool[INVENTORY_SIZE];
        for (int i = 0; i < occupiedslots.Length; i++)
        {
            occupiedslots[i] = false;
        }
    }

    public void Clean()
    {
        objects.Clear();
    }

    public void Add(Collectable obj)
    {
        if (IsFull())
            return;

        InventoryItem item = obj.ToInventoryItem();
        objects.Add(item);

        int i = 0;
        for (; i < INVENTORY_SIZE; i++)
        {
            if (!occupiedslots[i])
            {
                occupiedslots[i] = true;
                break;
            }
        }

        images[i].sprite = item.sprite;
        images[i].preserveAspect = true;
        images[i].enabled = true;
    }
    public void Remove(Collectable obj)
    {
        int i = 0;
        objects.Remove(obj.ToInventoryItem());
        for(; i < images.Length; i++)
        {
            if (images[i].sprite.Equals(obj.inventory))
                break;
        }
        images[i].sprite = null;
        occupiedslots[i] = false;
    }

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
            images[index].sprite = null;
            occupiedslots[index] = false;
        }
    }
}
