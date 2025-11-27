using BehaviourAPI.Core;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Attack: MonoBehaviour
{
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private Animator animator;


    void Start()
    {
        playerInput = FindFirstObjectByType<PlayerInput>();    
    }

    public Status AttackP()
    {
        if(animator != null)
            animator.SetBool("isAttacking", true);

        playerInput.actions["Move"].Disable();

        EndGameManager.Instance.ShowLoseScreen();
        return Status.Success;
    }

    public IEnumerator DelayPlayerEnable()
    {
        yield return new WaitForSeconds(2);
        playerInput.actions["Move"].Enable();
    }
}
