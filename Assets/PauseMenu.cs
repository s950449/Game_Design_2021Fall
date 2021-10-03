using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class PauseMenu : MonoBehaviour
{
    public static bool GamePaused = false;
    public GameObject pauseMenuUI;
    public GameObject pauseFirstSelect;
    // Update is called once per frame
    private void Start()
    {
        pauseMenuUI.SetActive(false);
    }
    void Update()
    {
        if (Input.GetButtonDown("Pause"))
        {
            if (GamePaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }

    }
    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1;
        GamePaused = false;
    }
    void Pause()
    {
        EventSystem.current.SetSelectedGameObject(null);
        pauseMenuUI.SetActive(true);
        EventSystem.current.SetSelectedGameObject(pauseFirstSelect);
        Time.timeScale = 0;
        GamePaused = true;
    }
    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
        Time.timeScale = 1;
        GamePaused = false;
    }
    public void Restart()
    {
        SceneManager.LoadScene("Game");
        Time.timeScale = 1;
        GamePaused = false;
    }
}

