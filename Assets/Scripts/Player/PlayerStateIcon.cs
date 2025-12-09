using UnityEngine;

public class PlayerStateIcon: MonoBehaviour
{
    public SpriteRenderer iconRenderer;

    public Sprite crouch;
    public Sprite walk;
    public Sprite run;
    public Sprite stunned;

    private void LateUpdate()
    {
        iconRenderer.transform.forward = Camera.main.transform.forward;
    }

    public void SetCrouch() => iconRenderer.sprite = crouch;
    public void SetWalk() => iconRenderer.sprite = walk;
    public void SetRun() => iconRenderer.sprite = run;

    public void SetStunned() => iconRenderer.sprite = stunned;
}
