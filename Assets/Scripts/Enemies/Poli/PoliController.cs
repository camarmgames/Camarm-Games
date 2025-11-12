using BehaviourAPI.UnityToolkit.GUIDesigner.Runtime;
using UnityEngine;

public class PoliController : MonoBehaviour
{
    [Tooltip("If the character exceeds or equals this level, it is considered to be stressed.")]
    private const int STRESS_LIMIT = 70;

    [SerializeField]
    [Tooltip("Level of stress for this character. From 0 to 100.")]
    [Range(0, 100)]
    public int stressLevel;

    void Awake()
    {
        stressLevel = 0;
        GameObject.FindFirstObjectByType<MageController>().OnMageAppeared += MustEvolve;
    }

    private void AddStress(int sum)
    {
        stressLevel += sum;
    }

    public bool isStressed()
    {
        return (stressLevel >= STRESS_LIMIT);
    }

    public void MustEvolve()
    {
        if (isStressed())
        {
            Debug.Log("APARECIÓ");
            //GameObject.FindFirstObjectByType<EditorBehaviourRunner>().Data.pushPerceptions.Find(
                //p => p.name.Equals("ApareceMago")).pushPerception.Fire();
        }
    }
}
