using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class GameManager : MonoBehaviour
{
    private void CheckIfResumingFromSave()
    {
        if (BetweenScenesScript.ResumingFromSave)
        { // If resuming from save file, read from save file first
            Saving_PlayerManager data = Saving_SaveManager.LoadData();
            levelNo = data.level;
            // Tell ships to disable model & colliders, if previous shop says they're dead
            if (BetweenScenesScript.player1TempLives == 0)
            {
                Refs.playerShip1.plrSpawnDeath.PretendShipDoesntExist();
            }
            if (BetweenScenesScript.player2TempLives == 0 && BetweenScenesScript.PlayerCount == 2)
            {
                Refs.playerShip2.plrSpawnDeath.PretendShipDoesntExist();

            }
        }
    }
    private void PlayMusicIfEnabled()
    {
        if (BetweenScenesScript.MusicVolume > 0f)
        {
            musicLoop.Play();
        }
    }

    private void OnPause()
    {
        if (!Refs.gameOverPanel.activeInHierarchy)
        {
            if (Refs.gamePausePanel.activeInHierarchy)
            {
                PauseGame(1);
            }
            else
            {
                PauseGame(0);
            }
        }
    }
}
