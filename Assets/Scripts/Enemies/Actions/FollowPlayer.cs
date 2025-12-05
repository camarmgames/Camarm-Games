using BehaviourAPI.Core;
using UnityEngine;
using UnityEngine.AI;

public class FollowPlayer: MonoBehaviour
{
    [Header("FollowPlayer Settings")]
    [SerializeField] private float stopDistance = 2f;
    [SerializeField] private Animator animator;

    private NavMeshAgent agent;
    private Transform player;

    private bool isFollowing = false;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        player = GameObject.FindGameObjectWithTag("Player").transform;
    }


    private void Update()
    {
        if (isFollowing)
        {
            agent.SetDestination(player.position);
        }
    }
    public bool IsPlayerWithinDistance()
    {
        if(player == null) return false;

        return Vector3.Distance(transform.position, player.position) <= stopDistance;
    }

    public Status StartFollow()
    {
        if (isFollowing) return Status.Failure;

        isFollowing = true;
        agent.isStopped = false;
        if (animator != null)
            animator.SetBool("isWalking", isFollowing);

        return Status.Success;
    }

    public void StopFollow()
    {
        if (!isFollowing) return;

        isFollowing =false;
        agent.isStopped = true;

        if (animator != null)
            animator.SetBool("isWalking", isFollowing);
    }
}
