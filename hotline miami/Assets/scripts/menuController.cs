using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class menuController : MonoBehaviour
{
   [SerializeField] private Text text;

    void Start()
    {
        if(text != null)
        {
            mouseExit();
        }
    }
    public void StartGame()
    {
        SceneManager.LoadScene("game");
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void mouseEnter()
    {
        text.color = Color.green;
    }

    public void mouseExit()
    {
        text.color = Color.black;
    }

}
