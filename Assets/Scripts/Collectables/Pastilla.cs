using System;
using System.Collections;
using UnityEngine;

public class Pastilla : Collectable
{
    public enum TipoPastilla
    {
        Verde,
        Naranja,
        Amarilla
    }

    public TipoPastilla tipo;

    public override void Execute(Action onFinish)
    {
        Debug.Log("Pastilla usado");

        switch (tipo)
        {
            case TipoPastilla.Naranja:
                Trap.pastillaNaranja = true;
                break;

            case TipoPastilla.Verde:
                Trap.pastillaVerde = true;
                break;

            case TipoPastilla.Amarilla:
                Trap.pastillaAmarilla = true;
                break;
        }
            
        StartCoroutine(UseRoutine(onFinish));
    }

    private IEnumerator UseRoutine(Action onFinish)
    {
        float timer = duration;
        while (timer > 0f)
        {
            timer -= Time.deltaTime;
            yield return null;
        }


        Debug.Log("Se acabo");

        switch (tipo)
        {
            case TipoPastilla.Naranja:
                Trap.pastillaNaranja = false;
                break;

            case TipoPastilla.Verde:
                Trap.pastillaVerde = false;
                break;

            case TipoPastilla.Amarilla:
                Trap.pastillaAmarilla = false;
                break;
        }


        onFinish?.Invoke();
    }
}
