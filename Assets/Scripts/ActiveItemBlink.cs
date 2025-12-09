using UnityEngine;
using UnityEngine.UI;

public class ActiveItemBlink : MonoBehaviour
{
    public Image img;
    public float baseSpeed = 2f;
    public float maxSpeed = 10f;

    public float duration;
    public float remainingTime;

    private float t;

    void Start()
    {
        if (img == null)
            img = GetComponent<Image>();
    }

    public void Activate(float totalTime)
    {
        duration = totalTime;
        remainingTime = totalTime;
        enabled = true;
        t = 0;
    }

    void Update()
    {
        if (remainingTime <= 0)
        {
            Color c = img.color;           
            img.color = c;
            enabled = false;
            return;
        }

        // velocidad aumenta a medida que queda menos
        float pct = 1f - (remainingTime / duration);
        float speed = Mathf.Lerp(baseSpeed, maxSpeed, pct);

        t += Time.deltaTime * speed;
        float alpha = Mathf.Lerp(0.3f, 1f, (Mathf.Sin(t) + 1f) / 2f);

        Color col = img.color;
        col.a = alpha;
        img.color = col;

        remainingTime -= Time.deltaTime;
    }
}
