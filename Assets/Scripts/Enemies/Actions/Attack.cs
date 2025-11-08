using UnityEngine;
using UnityEngine.InputSystem;

public class Attack: MonoBehaviour
{
    [SerializeField]
    private PlayerInput playerInput;

    void Start()
    {
        playerInput = FindFirstObjectByType<PlayerInput>();    
    }

    public void AttackP()
    {
        playerInput.actions["Move"].Disable();
        Debug.Log("Jugador paralizado");
    }
}
