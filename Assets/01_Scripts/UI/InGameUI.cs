using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameUI : UISystem
{
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] bool isPaused = false;

    void OnEnable()
    {
        Time.timeScale = 1f;
        isPaused = false;
        if (pauseMenu != null)
            pauseMenu.SetActive(false);
    }

    void OnDisable()
    {
        Time.timeScale = 0f;
        if (pauseMenu != null)
            pauseMenu.SetActive(true);
    }

    void Start()
    {
        if (pauseMenu != null)
            pauseMenu.SetActive(false);
    }

    public void OnPauseButton()
    {
        Debug.Log("Pause button clicked");
        if (!isPaused)
        {
            isPaused = true;
            pauseMenu.SetActive(true);
            Time.timeScale = 0f;
        }
        else
        {
            isPaused = false;
            pauseMenu.SetActive(false);
            Time.timeScale = 1f;
        }
    }

    public void OnExitButton()
    {
        GameManager.Instance.EndGame();
        UIManager.Instance.OnReturnToLobby();
    }
}