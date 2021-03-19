using UnityEngine;

public partial class GameManager : MonoBehaviour
{
    public void PauseGame(int intent)
    {
        if (intent == 0) // PAUSE
        {

            if (musicManager != null)
            {
                musicManager.PauseMusic();
                musicManager.FindAllSfxAndPlayPause(0);
            }

            // If it's the first level, or tutorial mode, disable the auto-save warning text
            if (levelNo == 1 || tutorialMode)
            {
                Refs.gamePausePanel.transform.Find("PauseDialog").Find("WarningText").gameObject.SetActive(false);
            }

            // Delay player input, so thrusting and shooting can't happen on the same frame as the game is unpaused
            StartCoroutine(Refs.playerShip1.GetComponent<PlayerInput>().DelayNewInputs());
            if (BetweenScenes.PlayerCount == 2)
                StartCoroutine(Refs.playerShip2.GetComponent<PlayerInput>().DelayNewInputs());

            Refs.gamePausePanel.SetActive(true);
            Refs.buttonWhenPaused.Select();
            Time.timeScale = 0;
        }
        else if (intent == 1) // RESUME
        {

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
}
