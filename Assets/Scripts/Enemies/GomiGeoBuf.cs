using UnityEngine;

public class GomiGeoBuf: MonoBehaviour, IEnemyBuffable
{
    public void OnBuffApplied()
    {
        Debug.Log($"{name} ha sido buffeado");
    }

    public void OnBuffRemoved()
    {
        Debug.Log($"{name} vuelve a estado normal");
    }
}
