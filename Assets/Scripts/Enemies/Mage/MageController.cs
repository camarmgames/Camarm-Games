using System;
using UnityEngine;

public class MageController : MonoBehaviour
{
    public Action OnMageAppeared;

    void Start()
    {
        
    }

    public void MageAppeared()
    {
        OnMageAppeared?.Invoke();
    }
}
