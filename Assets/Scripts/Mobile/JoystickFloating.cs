using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class JoystickFloating: MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [Header("Joystick References")]
    [SerializeField] private Image bg;
    [SerializeField] private Image handle;

    [Header("Settings")]
    [SerializeField] private float handleLimit = 1f;

    private Vector2 inputVector;
    private bool isDragging = false;

    public Vector2 GetInput() => inputVector;

    private void Start()
    {
        bg.enabled = false;
        handle.enabled = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isDragging = true;

        bg.rectTransform.position = eventData.position;

        bg.enabled = true;
        handle.enabled = true;

        OnDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 pos;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            bg.rectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out pos
        );

        pos /= bg.rectTransform.sizeDelta / 2f;

        inputVector = pos.magnitude > 1 ? pos.normalized : pos;

        handle.rectTransform.anchoredPosition = inputVector * (bg.rectTransform.sizeDelta / 2f) * handleLimit;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isDragging = false;
        inputVector = Vector2.zero;

        bg.enabled = false;
        handle.enabled = false;
    }
}
