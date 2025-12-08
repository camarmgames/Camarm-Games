using UnityEngine;
using UnityEngine.InputSystem;
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
        EndGameManager.Instance.Reset();
        PlayerInput playerInput = FindFirstObjectByType<PlayerInput>();
        playerInput?.actions["Move"].Enable();
        Time.timeScale = 1.0f;
    }

    public void PauseTimeGame()
    {
        Time.timeScale = 0.0f;
    }

    public void RestoreTimeGame()
    {
        Time.timeScale = 1.0f;
    }
}
