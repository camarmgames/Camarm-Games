using UnityEngine;
using UnityEngine.UI;

public class BlinkEffect : MonoBehaviour
{
    public float speed = 2f; // velocidad del parpadeo
    public float minAlpha = 0.3f;
    public float maxAlpha = 1f;

    private SpriteRenderer spriteRenderer;
    private Image uiImage;
    private float t = 0f;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        uiImage = GetComponent<Image>();
    }

    void Update()
    {
        t += Time.deltaTime * speed;
        float alpha = Mathf.Lerp(minAlpha, maxAlpha, (Mathf.Sin(t) + 1f) / 2f);

        if (spriteRenderer != null)
        {
            Color c = spriteRenderer.color;
            c.a = alpha;
            spriteRenderer.color = c;
        }
        else if (uiImage != null)
        {
            Color c = uiImage.color;
            c.a = alpha;
            uiImage.color = c;
        }
    }
}