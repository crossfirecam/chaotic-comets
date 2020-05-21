using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public partial class GameManager : MonoBehaviour
{

    // Swap player 1's controls with this button
    public void SwapP1Controls()
    {
        if (BetweenScenesScript.ControlTypeP1 == 0)
        {
            BetweenScenesScript.ControlTypeP1 = 1;
            Refs.swapP1Text.text = "Swap P1 to gamepad controls";
        }
        else
        {
            BetweenScenesScript.ControlTypeP1 = 0;
            Refs.swapP1Text.text = "Swap P1 to keyboard controls";
        }
    }
    // If player 2 exists, then swap their controls with this button
    public void SwapP2Controls()
    {
        if (BetweenScenesScript.PlayerCount == 2)
        {
            if (BetweenScenesScript.ControlTypeP2 == 0)
            {
                BetweenScenesScript.ControlTypeP2 = 1;
                Refs.swapP2Text.text = "Swap P2 to gamepad controls";
            }
            else
            {
                BetweenScenesScript.ControlTypeP2 = 0;
                Refs.swapP2Text.text = "Swap P2 to keyboard controls";
            }
        }
    }

    // Each time the pause menu is used, change the buttons for controller swapping depending on their state
    public void CheckPlayerControls()
    {
        if (BetweenScenesScript.ControlTypeP1 == 0) { Refs.swapP1Text.text = "Swap P1 to keyboard controls"; }
        else { Refs.swapP1Text.text = "Swap P1 to gamepad controls"; }
        if (BetweenScenesScript.PlayerCount == 1) { Refs.swapP2Text.text = ""; }
        else if (BetweenScenesScript.ControlTypeP2 == 0) { Refs.swapP2Text.text = "Swap P2 to keyboard controls"; }
        else { Refs.swapP2Text.text = "Swap P2 to gamepad controls"; }
    }

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
            CheckPlayerControls();
        }
        else if (intent == 1)
        { // Resume game
            Cursor.visible = false;
            if (!player1dead)
            {
                Refs.playerShip1.plrUiSound.CheckSounds(2);
                Refs.playerShip1.plrInput.InputChoice();
            }
            if (!player2dead)
            {
                Refs.playerShip1.plrUiSound.CheckSounds(2);
                Refs.playerShip2.plrInput.InputChoice();
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
