using System;
using UnityEngine;

public class Martillo : Collectable
{
    public override void Execute(Action onFinish)
    {
        Debug.Log("Martillo usado,");
    }
}
