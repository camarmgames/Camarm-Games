using System;
using System.Collections;
using UnityEngine;

public class Batido : Collectable
{
    public override void Execute(Action onFinish)
    {
        Debug.Log("Batido usado");
        CameraShake.Instance.powerUpBatido = true;

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
        CameraShake.Instance.powerUpBatido = false;
        onFinish?.Invoke();
    }
}
