using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonUtils : MonoBehaviour
{
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit(0);
#endif

    }

    public void ChangeScene(string nameSceneToGo)
    {
        SceneManager.LoadScene(nameSceneToGo);
    }
}
