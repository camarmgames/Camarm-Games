using UnityEngine;

public class GomiNinjaBuf: MonoBehaviour, IEnemyBuffable
{
    public void OnBuffApplied()
    {
        GetComponent<cp_GomiNinja>().SetBTSeActivationPush();
        Debug.Log($"{name} ha sido buffeado");
    }

    public void OnBuffRemoved()
    {
        GetComponent<cp_GomiNinja>().SetBTStActivationPush();
        Debug.Log($"{name} vuelve a estado normal");
    }
}
