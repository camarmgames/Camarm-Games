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
        images[i].color = new Color(1f, 1f, 1f, 1f);
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
        images[i].color.WithAlpha(0f);
        images[i].sprite = null;
        occupiedslots[i] = false;
    }

    public void RemoveAtSlot(int slotIndex, InventoryItem item)
    {
        Debug.Log("Eliminado");
        if (objects.Contains(item))
            objects.Remove(item);

        
        images[slotIndex].sprite = null;
        images[slotIndex].color = new Color(1f, 1f, 1f, 0f); 
        images[slotIndex].enabled = false;
        occupiedslots[slotIndex] = false;

        
        ActiveItemBlink blink = images[slotIndex].GetComponent<ActiveItemBlink>();
        if (blink != null)
            Destroy(blink);
    }

    public bool IsFull()
    {
        return (objects.Count == INVENTORY_SIZE);
    }

    public void Use(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= INVENTORY_SIZE)
            return;

        if (!occupiedslots[slotIndex])
            return;

        
        Sprite slotSprite = images[slotIndex].sprite;

        
        int objListIndex = objects.FindIndex(it => it.sprite == slotSprite);
        if (objListIndex == -1)
        {
            Debug.LogWarning("No se encontró el item en la lista de objects para el slot " + slotIndex);
            return;
        }

        InventoryItem item = objects[objListIndex];

        
        ActiveItemBlink blink = images[slotIndex].GetComponent<ActiveItemBlink>();
        if (blink == null)
            blink = images[slotIndex].gameObject.AddComponent<ActiveItemBlink>();

        blink.Activate(item.collectable.duration);

        
        int localSlot = slotIndex;

        
        item.Use(() =>
        {
            
            RemoveAtSlot(localSlot, item);
        });

    }
}
