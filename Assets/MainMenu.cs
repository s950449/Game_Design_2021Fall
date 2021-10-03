using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public GameObject MainMenuUI;
    public GameObject FirstSelect;
    // Update is called once per frame
    private void Start()
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(FirstSelect);
    }
    public void Quit()
    {
        EventSystem.current.SetSelectedGameObject(null);
        Debug.Log("Quit the game now");
        Application.Quit();
    }

    public void Play()
    {
        EventSystem.current.SetSelectedGameObject(null);
        SceneManager.LoadScene("Game");
        Time.timeScale = 1;
    }
}

