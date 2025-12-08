using System;
using UnityEngine;

public class Maracas : Collectable
{
    public override void Execute(Action onFinish)
    {
        Debug.Log("Maracas usadas.");
    }
}
