using System;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    private bool isPaused = false;

    [SerializeField]
    private GameObject pausePanel;
    [SerializeField]
    private GameObject pauseButtons;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TriggerPauseManager();
        }
    }

    public void TriggerPauseManager()
    {
        if (!isPaused)
        {
            pausePanel.SetActive(true);
            pauseButtons.SetActive(true);
            Time.timeScale = 0.0f;
            isPaused = true;
        }
        else
        {
            pausePanel.SetActive(false);
            pauseButtons.SetActive(false);
            Time.timeScale = 1.0f;
            isPaused = false;
        }
    }
}
