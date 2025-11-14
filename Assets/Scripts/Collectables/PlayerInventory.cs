using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory instance;

    public List<Collectable> objects;

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

    public void Add(Collectable obj) { objects.Add(obj); }
    public void Remove(Collectable obj) { objects.Remove(obj); }


}
