using UnityEngine;
using DG.Tweening;

public class MovimientoSeleccionPersonaje : MonoBehaviour
{
    public float duracion = 0.5f;
    public Ease curva = Ease.OutQuad;

    public void MoverX(float pos)
    {
        Vector3 destino = new Vector3(
            pos,
            transform.position.y,
            transform.position.z
        );
        transform.DOMove(destino, duracion).SetEase(curva);
    }
}
