using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class stopMenu : MonoBehaviour
{
    [SerializeField] private GameObject stopMenuObject;

    void Update() 
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            StopGame();
        }
    }
    public void GoToMenu()
    {
        SceneManager.LoadScene("menu");
        Time.timeScale = 1;
    }

    public void ResumeGame()
    {
        stopMenuObject.SetActive(false);
        Time.timeScale = 1;
    }

    public void StopGame()
    {
        stopMenuObject.SetActive(true);
        Time.timeScale = 0;
    }
}
