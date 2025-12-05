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

    private void Awake()
    {
        AudioManager.Instance.PlayMusic(music);
        GenerateTiles();    
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
    }
}
