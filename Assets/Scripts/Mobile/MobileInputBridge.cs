using UnityEngine;

public class MobileInputBridge: MonoBehaviour
{
    public static MobileInputBridge Instance;

    private Vector2 moveInput;
    private bool sprint;
    private bool crouch;

    private void Awake()
    {
        Instance = this;
    }

    public Vector2 GetMove() => moveInput;
    public bool GetSprint() => sprint;
    public bool GetCrouch() => crouch;
    public void SetMove(Vector2 value)
    {
        moveInput = value;
    }

    public void SprintDown()
    {
        sprint = true;
    }

    public void SprintUp()
    {
        sprint = false;
    }

    public void CrouchDown()
    {
        crouch = true;
    }

    public void CrouchUp()
    {
        crouch = false;
    }
}
