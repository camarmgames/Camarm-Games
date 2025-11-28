using DG.Tweening;
using UnityEngine;

public class BounceLoop : MonoBehaviour
{
    public float altura = 0.3f;        // Qué alto salta
    public float duracion = 0.4f;      // Cuánto tarda en subir o bajar
    public Ease curva = Ease.OutQuad;  // Curva del movimiento

    private Vector3 posicionInicial;

    void Start()
    {
        posicionInicial = transform.localPosition;

        // Animación infinita de rebote
        transform
            .DOLocalMoveY(posicionInicial.y + altura, duracion)
            .SetEase(curva)
            .SetLoops(-1, LoopType.Yoyo);
    }
}
