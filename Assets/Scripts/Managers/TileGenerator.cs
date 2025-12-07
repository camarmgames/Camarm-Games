using System.Collections.Generic;
using UnityEngine;

public class TileGenerator: MonoBehaviour
{
    [Header("MusicLevel")]
    public AudioClip music;

    [Header("Prefabs de Baldosa")]
    public GameObject hardTilePrefab;
    public GameObject softTilePrefab;

    [Header("Size of floor")]
    public int width = 10;
    public int length = 10;
    public float tileSize = 1f;

    [Header("Render in camera")]
    public Transform playerCamera;
    public float activationDistance = 40f;

    private Transform[] tiles;
    private List<Renderer> tileRenderers = new List<Renderer>();

    private void Awake()
    {
        AudioManager.Instance.PlayMusic(music);
        GenerateTiles();    
    }


    private void Update()
    {
        Vector3 camPos = playerCamera.transform.position;
        Vector3 camForward = playerCamera.transform.forward;

        foreach (Renderer r in tileRenderers)
        {
            Vector3 dir = (r.transform.position - camPos).normalized;

            bool inDistance = Vector3.Distance(r.transform.position, camPos) < activationDistance;
            bool inFov = Vector3.Dot(camForward, dir) > 0.4f; // 0.4 ~ 66° visión

            r.enabled = inDistance && inFov;
        }
    }


    void GenerateTiles()
    {
        for(int x = 0; x < width; x++)
        {
            for(int z = 0; z < length; z++)
            {
                GameObject prefabToSpawn = (Random.value > 0.25f) ? hardTilePrefab : softTilePrefab;

                Vector3 position = new Vector3(x * tileSize, 0, z * tileSize);

                Instantiate(prefabToSpawn, position, Quaternion.identity, transform);
            }
        }

        tiles = GetComponentsInChildren<Transform>();

        foreach (Transform t in tiles)
        {
            Renderer r = t.GetComponent<Renderer>();
            if (r != null)
                tileRenderers.Add(r);
        }
    }
}
