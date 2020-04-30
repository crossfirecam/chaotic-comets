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
            swapP1Text.text = "Swap P1 to gamepad controls";
        }
        else
        {
            BetweenScenesScript.ControlTypeP1 = 0;
            swapP1Text.text = "Swap P1 to keyboard controls";
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
                swapP2Text.text = "Swap P2 to gamepad controls";
            }
            else
            {
                BetweenScenesScript.ControlTypeP2 = 0;
                swapP2Text.text = "Swap P2 to keyboard controls";
            }
        }
    }

    // Each time the pause menu is used, change the buttons for controller swapping depending on their state
    public void CheckPlayerControls()
    {
        if (BetweenScenesScript.ControlTypeP1 == 0) { swapP1Text.text = "Swap P1 to keyboard controls"; }
        else { swapP1Text.text = "Swap P1 to gamepad controls"; }
        if (BetweenScenesScript.PlayerCount == 1) { swapP2Text.text = ""; }
        else if (BetweenScenesScript.ControlTypeP2 == 0) { swapP2Text.text = "Swap P2 to keyboard controls"; }
        else { swapP2Text.text = "Swap P2 to gamepad controls"; }
    }

    public void PauseGame(int intent)
    {
        if (intent == 0)
        { // Pause game
            Cursor.visible = true;
            if (!player1dead) { playerShip1.CheckSounds(1); }
            if (!player2dead) { playerShip2.CheckSounds(1); }

            musicLoop.Pause();

            GameObject[] listOfUfos = GameObject.FindGameObjectsWithTag("ufo");
            foreach (GameObject ufo in listOfUfos)
            {
                ufo.GetComponent<UfoAllTypes>().CheckAlienSounds(1);
            }

            gamePausePanel.SetActive(true);
            buttonWhenPaused.Select();
            Time.timeScale = 0;
            CheckPlayerControls();
        }
        else if (intent == 1)
        { // Resume game
            Cursor.visible = false;
            if (!player1dead)
            {
                playerShip1.CheckSounds(2);
                playerShip1.playerInput.InputChoice();
            }
            if (!player2dead)
            {
                playerShip1.CheckSounds(2);
                playerShip2.playerInput.InputChoice();
            }

            if (BetweenScenesScript.MusicVolume > 0f && !helpMenuMode) { musicLoop.Play(); }

            GameObject[] listOfUfos = GameObject.FindGameObjectsWithTag("ufo");
            foreach (GameObject ufo in listOfUfos)
            {
                ufo.GetComponent<UfoAllTypes>().CheckAlienSounds(2);
            }

            gamePausePanel.SetActive(false);
            buttonWhenLeavingPauseBugFix.Select();
            Time.timeScale = 1;
        }
    }

    public void ExitGame()
    {
        SceneManager.LoadScene("StartMenu");
        Time.timeScale = 1;
    }
}
