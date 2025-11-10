using UnityEngine;

public class TeleportBehindPlayer: MonoBehaviour
{
    [SerializeField, Tooltip("Distance behind of the player")]
    private float behindDistance = 2f;

    public Transform playerT;

    public void TeleportEnemyBehindPlayer(Transform player)
    {
        Vector3 behindPosition = player.position - player.forward*behindDistance;

        behindPosition.y = transform.position.y;

        transform.position = behindPosition;

        transform.LookAt(player);

        Debug.Log($"{name} se ha teletransportado detras de {player.name}");
    }

    public void ATeleportEnemyBehindPlayer()
    {
        if(playerT != null)
            TeleportEnemyBehindPlayer(playerT);
    }
}
