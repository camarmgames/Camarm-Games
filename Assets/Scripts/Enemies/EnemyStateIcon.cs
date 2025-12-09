using UnityEngine;

public class EnemyStateIcon: MonoBehaviour
{
    public SpriteRenderer iconRenderer;

    public Sprite calm;
    public Sprite detected;
    public Sprite alert;
    public Sprite takeABreak;
    public Sprite investigation;

    private void LateUpdate()
    {
        iconRenderer.transform.forward = Camera.main.transform.forward;
    }

    public void SetCalm() => iconRenderer.sprite = calm;
    public void SetDetected() => iconRenderer.sprite = detected;
    public void SetAlert() => iconRenderer.sprite = alert;

    public void SetTakeABreak() => iconRenderer.sprite = takeABreak;

    public void SetInvestigation() => iconRenderer.sprite = investigation;
}
