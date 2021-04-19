using UnityEngine;
using TMPro;
using UnityEngine.UI;

public partial class UiManager : MonoBehaviour
{
    /* ------------------------------------------------------------------------------------------------------------------
     * Pause Dialog - Methods to do with players pausing the game
     * ------------------------------------------------------------------------------------------------------------------ */

    [Header("Pause Dialog UI")]
    [SerializeField] private GameObject panelPauseMenu;
    [SerializeField] private Button buttonWhenPaused, buttonWhenLeavingPauseBugFix;
    [SerializeField] private GameObject pauseAutoSaveWarningText, pauseCheatMenu;
    [SerializeField] private RectTransform pauseDialogRt;

    public void PauseGame()
    {
        if (MusicManager.i != null)
        {
            MusicManager.i.PauseMusic();
            MusicManager.i.FindAllSfxAndPlayPause(0);
        }

        // If it's the first level, or tutorial mode, disable the auto-save warning text
        if (GameManager.i.AutoSaveWarnShouldHide())
        {
            pauseAutoSaveWarningText.SetActive(false);
        }

        // If it's Cheat Mode, move the pause menu up to make room.
        if (BetweenScenes.CheaterMode)
            pauseDialogRt.localPosition = new Vector2(0f, 100f);

        // Delay player input, so thrusting and shooting can't happen on the same frame as the game is unpaused
        StartCoroutine(GameManager.i.Refs.playerShip1.GetComponent<PlayerInput>().DelayNewInputs());
        if (BetweenScenes.PlayerCount == 2)
            StartCoroutine(GameManager.i.Refs.playerShip2.GetComponent<PlayerInput>().DelayNewInputs());

        panelPauseMenu.SetActive(true);
        buttonWhenPaused.Select();
        Time.timeScale = 0;
    }
    public void ResumeGame()
    {
        if (PlayerPrefs.GetFloat("Music") > 0f && MusicManager.i != null)
        {
            MusicManager.i.ResumeMusic();
            MusicManager.i.FindAllSfxAndPlayPause(1);
        }

        panelPauseMenu.SetActive(false);
        buttonWhenLeavingPauseBugFix.Select();
        Time.timeScale = 1;
    }


    // If game over panel, or tutorial control choice panel are up, do not pause, otherwise handle pausing
    public void OnPressingPauseButton()
    {
        if (!panelGameOver.activeInHierarchy && !panelTutorialChoice.activeInHierarchy)
        {
            if (panelPauseMenu.activeInHierarchy)
                ResumeGame();
            else
                PauseGame();
        }
    }

    public bool GameIsPaused()
    {
        return panelPauseMenu.activeInHierarchy;
    }

    public void EnablePauseCheatPanel()
    {
        pauseCheatMenu.SetActive(true);
    }
}
