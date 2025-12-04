using UnityEditor;
using UnityEngine;

public class EndGameManager: MonoBehaviour
{
    public static EndGameManager Instance;

    public GameObject endScreen;
    public GameObject backgroundVictory;
    public GameObject backgroundLost;
    public GameObject btnRetry;
    public GameObject btnMainMenu;
    public TMPro.TextMeshProUGUI titleText;

    public bool isShowing = false;

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

        Time.timeScale = 0f;

        backgroundLost.SetActive(true);
        titleText.text = "Derrota";
        btnRetry.SetActive(true);
        btnMainMenu.SetActive(false);

        endScreen.SetActive(true);
    }

    public void ShowWinScreen()
    {
        if(isShowing) return;
        isShowing = true;

        Time.timeScale = 0f;

        backgroundVictory.SetActive(true);
        titleText.text = "Victoria";
        btnRetry.SetActive(false);
        btnMainMenu.SetActive(true);

        endScreen.SetActive(true);
    }

    public void Reset()
    {
        endScreen.SetActive(false);
        backgroundVictory.SetActive(false);
        backgroundLost.SetActive(false);
        btnRetry.SetActive(false);
        btnMainMenu.SetActive(false);
        isShowing = false;
    }
}
