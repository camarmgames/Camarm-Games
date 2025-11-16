using UnityEditor;
using UnityEngine;

public class EndGameManager: MonoBehaviour
{
    public static EndGameManager Instance;

    public GameObject endScreen;
    public GameObject btnRetry;
    public GameObject btnPlayAgain;
    public TMPro.TextMeshProUGUI titleText;

    private bool isShowing = false;

    void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);

        endScreen.SetActive(false);
    }

    public void ShowLoseScreen()
    {
        if(isShowing) return;
        isShowing = true;

        //Time.timeScale = 0f;

        titleText.text = "! has perdido !";
        btnRetry.SetActive(true);
        btnPlayAgain.SetActive(false);

        endScreen.SetActive(true);
    }

    public void ShowWinScreen()
    {
        if(isShowing) return;
        isShowing = true;

        //Time.timeScale = 0f;

        titleText.text = "! victoria !";
        btnRetry.SetActive(false);
        btnPlayAgain.SetActive(true);

        endScreen.SetActive(true);
    }
}
