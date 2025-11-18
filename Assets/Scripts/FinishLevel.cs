using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishLevel: MonoBehaviour
{
    [SerializeField]
    private bool finishGame = false;

    [SerializeField]
    private string nameSceneToGo;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (finishGame) {
                if(EndGameManager.Instance != null)
                    EndGameManager.Instance.ShowWinScreen();
            }
            else
            {
                ChangeScene();
            }
        }
    }

    public void ChangeScene()
    {
        SceneManager.LoadScene(nameSceneToGo);
    }
}
