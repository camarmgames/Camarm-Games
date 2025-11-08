using UnityEngine;

[CreateAssetMenu(fileName = "NewSurfaceType", menuName = "Scriptable Objects/SurfaceType")]
public class SurfaceType : ScriptableObject
{
    public string surfaceName = "Default";
    public bool leavesFootprints = true;
    public float footprintLifetime = 10f;
    public GameObject footprintPrefab;
}
