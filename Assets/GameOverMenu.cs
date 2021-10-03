using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameOverMenu : MonoBehaviour
{
    public GameObject GameOverMenuUI;
    public GameObject FirstSelect;
    [SerializeField] private Text _scoreText;
    public static bool alreadyOver = false;
    GameObject CarrierObject;
    private static int score;
    // Update is called once per frame
    private void Start()
    {
        CarrierObject = GameObject.Find("persistentObject");
        score = PassingValue.score;
        _scoreText.text = score.ToString();
        alreadyOver = false;
    }
    private void Update()
    {
        if (!alreadyOver)
           CallGameOver();
    }
    public void CallGameOver()
    {
        EventSystem.current.SetSelectedGameObject(null);
        GameOverMenuUI.SetActive(true);
        EventSystem.current.SetSelectedGameObject(FirstSelect);
        Time.timeScale = 0;
        alreadyOver = true;

    }
    public void Quit()
    {
        Debug.Log("Quit the game now");
        Application.Quit();
        Time.timeScale = 1;
    }
    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
        alreadyOver = false;
        Time.timeScale = 1;
    }
    public void Restart()
    {

        SceneManager.LoadScene("Game");
        alreadyOver = false;
        Time.timeScale = 1;
    }
}

