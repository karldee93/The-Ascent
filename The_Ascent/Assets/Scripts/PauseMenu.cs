using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class PauseMenu : MonoBehaviour
{
    // bool is to tell the script whether or not the game is paused.
    public static bool gameIsPaused = false;
    // game object variable have been created to hold each UI panel so they can be activated and deactivated when required.
    public GameObject instructionsMenu, pauseMenu, winScreen;
    void Update()
    {
        // if player has pressed key "p" run code.
        if (Input.GetKeyDown(KeyCode.P))
        {
            // if gameIsPaused is true then call Resume function.
            if (gameIsPaused)
            {
                Resume();
            }
            // if gameIsPaused is false run pause function.
            else
            {
                Pause();
            }
        }
    }
    // this function will unpause the game when called.
    public void Resume()
    {
        // set the game object held in pauseMenu to inactive.
        pauseMenu.SetActive(false);
        // set the game object held in instructionsMenu to inactive.
        instructionsMenu.SetActive(false);
        // set the system time scale to 1 - this will "unfreeze the game".
        Time.timeScale = 1f;
        // set gameIsPaused to false.
        gameIsPaused = false;
    }
    // this function will pause the game when called.
    void Pause()
    {
        // set the game object held in pauseMenu to active.
        pauseMenu.SetActive(true);
        // set the system time to 0 - this will effectively "feeze the game".
        Time.timeScale = 0f;
        // set gameIsPaused to true;
        gameIsPaused = true;
    }
    // this will quit the application.
    public void Quit()
    {
        Application.Quit();
    }
    // works same as in main menu but pause menu replaces the former.
    public void Instructions()
    {
        instructionsMenu.SetActive(true);
        pauseMenu.SetActive(false);
    }
    // function will allow the various menus to be backed out of and take player back to pause menu.
    public void Back()
    {
        // this works the same as the above but has been set up to be used in the case of being used in either the instructions menu or the options menu.
        instructionsMenu.SetActive(false);
        pauseMenu.SetActive(true);
    }
    // this is an option for the win panel which will display when the game has ended and will restart teh game session when invoked.
    public void Restart()
    {
        winScreen.SetActive(false);
        SceneManager.LoadScene(1);
        Time.timeScale = 1f;
        gameIsPaused = false;
    }
}
