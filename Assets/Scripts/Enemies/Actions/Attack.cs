using System.Collections;
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
        StartCoroutine(DelayPlayerEnable());
        Debug.Log("Jugador paralizado");
    }

    public IEnumerator DelayPlayerEnable()
    {
        yield return new WaitForSeconds(2);
        playerInput.actions["Move"].Enable();
    }
}
