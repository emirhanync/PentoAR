using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public GameObject Levels;

    private void Start()
    {
        Levels.SetActive(false);
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Level_1");
    }
    public void ShowLevel() 
    {
        Levels.SetActive(true);
    }
    public void ExitGame() 
    { 
        Application.Quit();
    }

    public void ExitLevels() 
    {
        Levels.SetActive(false);
    }
}
