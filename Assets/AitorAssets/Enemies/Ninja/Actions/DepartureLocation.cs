using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class DepartureLocation: MonoBehaviour
{
    [Header("Configuration of Spawn")]
    [Tooltip("Timer in seconds")]
    public float checkInterval = 5f;

    [Tooltip("Minimum distance to player")]
    public float minDistance = 10f;

    [Tooltip("Maximum distance to player")]
    public float maxDistance = 50f;

    [Tooltip("List of posible locations")]
    public List<Transform> spawnPoints = new List<Transform>();

    public GameObject spawnPrefab;

    // Variables Publicas
    private float timer;
    

    private Vector3 playerPosition;

    private void Start()
    {
        
    }
    public void CalculatePositionToExit()
    {
        playerPosition = GameObject.FindGameObjectWithTag("Player").transform.position;

        if (playerPosition == null || spawnPoints.Count == 0) return;

        // Lista temporal con las posiciones validas segun distancia
        List<Transform> validPoints = new List<Transform>();
        while(validPoints.Count == 0)
        {
            foreach (Transform point in spawnPoints)
            {
                float dist = Vector3.Distance(point.position, playerPosition);
                Debug.Log(dist);
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
        Pathing pathing = GetComponent<Pathing>();
        pathing.positions.Clear();
      
        for (int i = 0; i < chosenPoint.childCount; i++) {
            pathing.positions.Add(chosenPoint.GetChild(i));

            Debug.Log(pathing.positions[i].position);
        }
        Debug.Log(pathing.positions.Count);

        Vector3 departurePosition = new Vector3(chosenPoint.position.x, GetComponent<Transform>().position.y, chosenPoint.position.z);

        // Animaciones y cosas
        

        // Hacer visible el personaje
        GetComponent<Transform>().position = departurePosition;
        spawnPrefab.SetActive(true);
    }

    public bool SetInvisible()
    {
        // Animaciones y cosas

        // Hacer invisible al personaje
        spawnPrefab.SetActive(false);
        return true;
    }

    public bool CheckInvisible()
    {
        if (spawnPrefab.activeSelf == true)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
