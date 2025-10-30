using DG.Tweening;
using UnityEngine;

public class menuCloudsRotation : MonoBehaviour
{
    RectTransform uiElement;
    [SerializeField] float timeToMakeALoop = 120f;

    void Start()
    {
        uiElement = GetComponent<RectTransform>();

        uiElement.DORotate(new Vector3(0, 0, -360), timeToMakeALoop, RotateMode.FastBeyond360).SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart);
    }
}
