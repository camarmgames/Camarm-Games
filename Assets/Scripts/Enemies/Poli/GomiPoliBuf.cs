using UnityEngine;
using UnityEngine.AI;

public class GomiPoliBufIEnemyBuffable: MonoBehaviour, IEnemyBuffable
{
    public void OnBuffApplied()
    {
        GetComponent<GomiPoliBehaviour>().FireMageAppear();
        Debug.Log($"{name} ha sido buffeado");
    }

    public void OnBuffRemoved()
    {
        GetComponent<NavMeshAgent>().speed = GetComponent<GomiPoliBehaviour>().speedOriginal;
        Debug.Log($"{name} vuelve a estado normal");
    }
}