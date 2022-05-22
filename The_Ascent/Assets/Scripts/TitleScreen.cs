using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class TitleScreen : MonoBehaviour
{
    public GameObject instructionsMenu, titleMenu;

    public void PlayGame()
    {
        SceneManager.LoadScene(1);
    }

    public void Instructions()
    {
        instructionsMenu.SetActive(true);
        titleMenu.SetActive(false);
    }

    public void Back()
    {
        instructionsMenu.SetActive(false);
        titleMenu.SetActive(true);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
