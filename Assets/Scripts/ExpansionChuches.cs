using UnityEngine;
using System.Collections;

public class ExpansionChuches : MonoBehaviour
{
    [Header("Referencias UI")]
    public Transform candiesParent;  // el grupo que contiene todas las chuches

    [Header("Ajustes de animación")]
    public float scatterDistance = 300f;
    public float scatterTime = 1f;

    private bool clicked = false;

    void Update()
    {
        if (!clicked && (Input.GetMouseButtonDown(0) || Input.touchCount > 0))
        {
            clicked = true;
            StartCoroutine(PlayAnimation());
        }
    }

    IEnumerator PlayAnimation()
    {
        // Iniciar dispersión de chuches
        foreach (Transform candy in candiesParent)
        {
            Vector2 dir = Random.insideUnitCircle.normalized;
            Vector3 target = candy.localPosition + (Vector3)(dir * scatterDistance);
            StartCoroutine(MoveCandy(candy, target, scatterTime));
        }

        // Esperar a que terminen las animaciones
        yield return new WaitForSeconds(scatterTime + 0.3f);

        // Ocultar el splash
        gameObject.SetActive(false);
    }

    IEnumerator MoveCandy(Transform candy, Vector3 target, float time)
    {
        Vector3 start = candy.localPosition;
        float elapsed = 0;

        while (elapsed < time)
        {
            elapsed += Time.deltaTime;
            candy.localPosition = Vector3.Lerp(start, target, elapsed / time);
            yield return null;
        }

        candy.localPosition = target;
    }
}
