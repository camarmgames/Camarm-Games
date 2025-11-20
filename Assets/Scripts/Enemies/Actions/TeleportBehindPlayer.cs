using BehaviourAPI.Core;
using System.Collections;
using UnityEngine;

public class TeleportBehindPlayer: MonoBehaviour
{
    [Header("Teleport Settings")]
    [SerializeField, Tooltip("Distance behind of the player")]
    private float behindDistance = 4f;

    [SerializeField, Tooltip("Cooldown between teleports")]
    private float teleportCooldown = 25f;

    [Header("Debug")]
    [SerializeField, Tooltip("Message console")]
    private bool debugLog;

    public Transform player;
    private bool canTeleport = true;
    public Status TeleportEnemyBehindPlayer()
    {
        if (!canTeleport)
        {
            if (debugLog)
                Debug.Log("Teleport en cooldown");
            return Status.Success;
        }

        StartCoroutine(TeleportCooldownRoutine());
        PerformTeleport();
        return Status.Success;
    }

    private void PerformTeleport()
    {
        Vector3 behindPosition = player.position - player.forward * behindDistance;

        behindPosition.y = transform.position.y;

        transform.position = behindPosition;

        transform.LookAt(player);

        Debug.Log($"{name} se ha teletransportado detras de {player.name}");
    }

    private IEnumerator TeleportCooldownRoutine()
    {
        canTeleport = false;

        yield return new WaitForSeconds(teleportCooldown);

        canTeleport = true;
    }
}
