using UnityEngine;

public class EnemyStateIcon: MonoBehaviour
{
    public SpriteRenderer iconRenderer;

    public Sprite calm;
    public Sprite detected;
    public Sprite alert;
    public Sprite takeABreak;
    public Sprite investigation;

    public Material materialVisionField;

    private void LateUpdate()
    {
        iconRenderer.transform.forward = Camera.main.transform.forward;
    }

    public void SetCalm()
    {
        materialVisionField?.SetColor("_VisionColor", Color.yellow);
        iconRenderer.sprite = calm;
    }

    public void SetDetected()
    {
        materialVisionField?.SetColor("_VisionColor", Color.red);
        iconRenderer.sprite = detected;
    }

    public void SetAlert()
    {
        materialVisionField?.SetColor("_VisionColor", Color.magenta);
        iconRenderer.sprite = alert;
    }

    public void SetTakeABreak() => iconRenderer.sprite = takeABreak;

    public void SetInvestigation() => iconRenderer.sprite = investigation;
}
