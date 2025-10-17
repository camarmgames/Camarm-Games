using System;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    private bool isPaused = false;

    [SerializeField]
    private GameObject pausePanel;

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
            Time.timeScale = 0.0f;
            isPaused = true;
        }
        else
        {
            pausePanel.SetActive(false);
            Time.timeScale = 1.0f;
            isPaused = false;
        }
    }
}
