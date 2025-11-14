using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class DepartureLocation: MonoBehaviour
{
    [Header("Configuration of Spawn")]
    [SerializeField]
    [Tooltip("Minimum distance to player")]
    private float minDistance = 10f;
    [SerializeField]
    [Tooltip("Maximum distance to player")]
    private float maxDistance = 50f;
    [SerializeField]
    [Tooltip("List of posible locations")]
    private List<Transform> spawnPoints = new List<Transform>();
    [SerializeField]
    [Tooltip("Prefab of renderObject")]
    private GameObject renderPrefab;

    [Header("Timer Settings")]
    [SerializeField]
    [Tooltip("Cooldown time before allowing next spawn")]
    private float cooldownTime = 10f;

    private Vector3 playerPosition;

    private float timer = 0f;
    private bool timerRunning = false;

    private void Update()
    {
        if (timerRunning)
        {
            timer -= Time.deltaTime;
            if(timer <= 0f)
            {
                timerRunning = false;
                timer = 0f;
            }
        }
    }

    #region Actions
    public void CalculatePositionToExit()
    {
        playerPosition = GameObject.FindGameObjectWithTag("Player").transform.position;

        if (playerPosition == null || spawnPoints.Count == 0) return;

        // Reinicio timer
        timer = cooldownTime;
        timerRunning = true;


        // Lista temporal con las posiciones validas segun distancia
        List<Transform> validPoints = new List<Transform>();
        while (validPoints.Count == 0)
        {
            foreach (Transform point in spawnPoints)
            {
                float dist = Vector3.Distance(point.position, playerPosition);
                if (dist >= minDistance && dist <= maxDistance)
                {
                    validPoints.Add(point);
                }
            }
            if (validPoints.Count == 0)
            {
                maxDistance += 50;
            }
        }

        Transform chosenPoint = validPoints[Random.Range(0, validPoints.Count)];

        // Poner Posiciones de ruta
        PathingNinja pathing = GetComponent<PathingNinja>();
        pathing.patrolPoints.Clear();

        for (int i = 0; i < chosenPoint.childCount; i++)
        {
            pathing.patrolPoints.Add(chosenPoint.GetChild(i));
        }

        Vector3 departurePosition = new Vector3(chosenPoint.position.x, GetComponent<Transform>().position.y, chosenPoint.position.z);
        Debug.Log(departurePosition);
        // Animaciones y cosas


        // Hacer visible el personaje
        GetComponent<Transform>().position = departurePosition;
        renderPrefab.SetActive(true);
    }

    public void SetInvisible()
    {
        // Animaciones y cosas

        // Hacer invisible al personaje
        renderPrefab.SetActive(false);
    }

    #endregion

    #region Perceptions
    public bool CheckInvisible()
    {
        return !renderPrefab.activeSelf;
    }

    public bool FinishTimer()
    {
        if (!timerRunning)
        {
            SetInvisible();
            return true;
        }
        return false;
    }
    #endregion
}
