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
            if (!player1dead) { Refs.playerShip1.plrUiSound.CheckSounds(1); }
            if (!player2dead) { Refs.playerShip2.plrUiSound.CheckSounds(1); }

            musicLoop.Pause();

            GameObject[] listOfUfos = GameObject.FindGameObjectsWithTag("ufo");
            foreach (GameObject ufo in listOfUfos)
            {
                ufo.GetComponent<Ufo>().CheckAlienSounds(1);
            }

            Refs.gamePausePanel.SetActive(true);
            Refs.buttonWhenPaused.Select();
            Time.timeScale = 0;
        }
        else if (intent == 1)
        { // Resume game
            Cursor.visible = false;
            if (!player1dead)
            {
                Refs.playerShip1.plrUiSound.CheckSounds(2);
            }
            if (!player2dead)
            {
                Refs.playerShip1.plrUiSound.CheckSounds(2);
            }

            if (BetweenScenesScript.MusicVolume > 0f && !tutorialMode) { musicLoop.Play(); }

            GameObject[] listOfUfos = GameObject.FindGameObjectsWithTag("ufo");
            foreach (GameObject ufo in listOfUfos)
            {
                ufo.GetComponent<Ufo>().CheckAlienSounds(2);
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
