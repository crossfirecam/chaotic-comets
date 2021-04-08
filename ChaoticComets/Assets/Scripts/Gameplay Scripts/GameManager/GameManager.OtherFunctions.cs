using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class GameManager : MonoBehaviour
{
    /// <summary>
    /// If the game is resuming from a save, perform loading of certain statistics from data.
    /// </summary>
    private void CheckIfResumingFromSave()
    {
        if (BetweenScenes.ResumingFromSave)
        { // If resuming from save file, read from save file first
            Saving_PlayerManager data = Saving_SaveManager.LoadData();
            levelNo = data.level;
            playerLives = BetweenScenes.PlayerShopLives;
            player1dead = data.player1dead;
            player2dead = data.player2dead;
        }
    }

    /// <summary>
    /// Change music track depending on if Tutorial or normal gameplay is chosen.
    /// </summary>
    private void PlayMusicIfEnabled()
    {
        if (!tutorialMode)
            MusicManager.i.ChangeMusicTrack(1);
        else
            MusicManager.i.ChangeMusicTrack(3);

        if (PlayerPrefs.GetFloat("Music") > 0f)
            MusicManager.i.currentMusicPlayer.Play();
    }

    /// <summary>
    /// Auto-save warning for Pause Menu. Don't show if game just starting, it's level 1, or it's tutorial mode.
    /// </summary>
    public bool AutoSaveWarnShouldHide()
    {
        return levelNo == 0 || levelNo == 1 || tutorialMode;
    }

    /// <summary>
    /// Determine how much Time Bonus is given to the player(s) at the start of a level, then count down by 10p per second.
    /// </summary>
    private IEnumerator BonusCounter()
    {
        bonusValue = 300 + (bonusPerAsteroid * asteroidCount);
        for (int i = bonusValue; i >= 0; i -= 10)
        {
            if (asteroidCount == 0)
            {
                break;
            }
            bonusValue = i;
            UiManager.i.SetBonusText(bonusValue);
            yield return new WaitForSeconds(1f);
        }
    }
}
