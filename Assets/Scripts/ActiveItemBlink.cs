using UnityEngine;
using UnityEngine.UI;

public class ActiveItemBlink : MonoBehaviour
{
    public Image img;

    [Header("Blink Settings")]
    public float baseSpeed = 2f;
    public float maxSpeed = 10f;

    private float duration;
    private float remainingTime;
    private float t;

    private bool isActive;
    private Color initialColor;

    void Awake()
    {
        if (img == null)
            img = GetComponent<Image>();

        if (img != null)
            initialColor = img.color;

        Desactivate(); // estado inicial limpio
    }

    public void Activate(float totalTime)
    {
        if (img == null)
            return;

        duration = totalTime;
        remainingTime = totalTime;
        t = 0f;
        isActive = true;
        enabled = true;
    }

    public void Desactivate()
    {
        isActive = false;
        enabled = false;

        if (img != null)
            img.color = initialColor;
    }

    void Update()
    {
        if (!isActive || img == null)
            return;

        if (remainingTime <= 0f)
        {
            Desactivate();
            return;
        }

        // Velocidad aumenta a medida que queda menos tiempo
        float pct = 1f - (remainingTime / duration);
        float speed = Mathf.Lerp(baseSpeed, maxSpeed, pct);

        t += Time.deltaTime * speed;

        float alpha =
            Mathf.Lerp(0.3f, initialColor.a, (Mathf.Sin(t) + 1f) * 0.5f);

        Color col = initialColor;
        col.a = alpha;
        img.color = col;

        remainingTime -= Time.deltaTime;
    }
}
