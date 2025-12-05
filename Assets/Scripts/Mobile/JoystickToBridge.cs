using UnityEngine;

public class JoystickToBridge: MonoBehaviour
{
    public JoystickFloating joystick;

    private void Update()
    {
        if(MobileInputBridge.Instance != null && joystick != null)
        {
            MobileInputBridge.Instance.SetMove(joystick.GetInput());
        }
    }
}
