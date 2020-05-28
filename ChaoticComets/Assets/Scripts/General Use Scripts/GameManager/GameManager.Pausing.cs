using Rewired;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public partial class GameManager : MonoBehaviour
{
    public void PauseGame(int intent)
    {
        if (intent == 0)
        { // Pause game
            Cursor.visible = true;
            if (musicManager != null)
            {
                musicManager.PauseMusic();
                musicManager.FindAllSfxAndPlayPause(0);
            }
            if (!player1dead)
                StartCoroutine(Refs.playerShip1.GetComponent<PlayerInput>().DelayNewInputs());
            if (!player2dead)
                StartCoroutine(Refs.playerShip2.GetComponent<PlayerInput>().DelayNewInputs());

            Refs.gamePausePanel.SetActive(true);
            Refs.buttonWhenPaused.Select();
            Time.timeScale = 0;
        }
        else if (intent == 1)
        { // Resume game
            Cursor.visible = false;
            if (PlayerPrefs.GetFloat("Music") > 0f && musicManager != null)
            {
                musicManager.ResumeMusic();
                musicManager.FindAllSfxAndPlayPause(1);
            }

            Refs.gamePausePanel.SetActive(false);
            Refs.buttonWhenLeavingPauseBugFix.Select();
            Time.timeScale = 1;
        }
    }

    public void ExitGame()
    {
        SceneManager.LoadScene("StartMenu");
        Time.timeScale = 1;
    }
}
