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

    public void OnSelectCharacter(int index)
    {
        CharacterSelection.Instance.SelectCharacter(index);
    }

    public void Retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    }

    public void NotShowEndGame()
    {
        EndGameManager.Instance.endScreen.SetActive(false);

    }
}
