using Rewired.Integration.UnityUI;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public partial class ShopScript : MonoBehaviour
{
    private void PlayMusicIfEnabled()
    {
        MusicManager.i.ChangeMusicTrack(2);
        MusicManager.i.currentMusicPlayer.Play();
    }



    /* ------------------------------------------------------------------------------------------------------------------
    * Pausing related code
    * ------------------------------------------------------------------------------------------------------------------ */

    /// <summary>
    /// When player pauses or resumes the game from Shop, toggle these actions:<br/>
    /// - Swapping both player's EventSystemShop's for pause panel EventSystem<br/>
    /// - Pause or resume music<br/>
    /// - Open or close pause panel, select relevent button<br/>
    /// - Pause or resume timescale<br/>
    /// <br/>
    /// ShopMenu cannot be paused if either a Save Warning or Mouse Warning panel is active.<br/>
    /// </summary>
    /// <param name="intent">0 = Open Pause Menu, 1 = Close Pause Menu</param>
    public void PauseGame(int intent)
    {
        if (intent == 0 && !ShopRefs.saveFailedPanel.activeInHierarchy && !ShopRefs.mouseWarningPanel.activeInHierarchy)
        {
            MusicManager.i.PauseMusic();

            ShopRefs.gamePausePanel.SetActive(true);
            TogglePlrEventsAndPauseEvents(true);

            Time.timeScale = 0;
        }
        else if (intent == 1)
        {
            MusicManager.i.ResumeMusic();

            ShopRefs.gamePausePanel.SetActive(false);
            TogglePlrEventsAndPauseEvents(false);

            Time.timeScale = 1;
        }
    }

    private void TogglePlrEventsAndPauseEvents(bool paused)
    {
        if (paused)
        {
            ShopRefs.plrEventSystems[0].gameObject.SetActive(false);
            ShopRefs.plrEventSystems[1].gameObject.SetActive(false);
            ShopRefs.pauseEventSystem.GetComponent<RewiredStandaloneInputModule>().enabled = true;
            ShopRefs.buttonWhenPaused.Select();
        }
        else
        {
            // Select an off-screen button with pauseEventSystem to prevent a UI bug
            ShopRefs.buttonWhenLeavingPauseBugFix.Select();
            ShopRefs.pauseEventSystem.GetComponent<RewiredStandaloneInputModule>().enabled = false;
            // Resume event systems for however many players are present
            for (int i = 0; i < BetweenScenes.PlayerCount; i++)
            {
                ShopRefs.plrEventSystems[i].gameObject.SetActive(true);
            }
        }
    }

    {
    }

    /* ------------------------------------------------------------------------------------------------------------------
     * Mouse Warning - Functions that show user a warning when clicking in shop
     * ------------------------------------------------------------------------------------------------------------------ */
    private bool mouseWarningActive = false;
    public void PlayMouseWarning()
    {
        if (!mouseWarningActive)
        {
            StartCoroutine(MouseWarning());
        }
    }
    private IEnumerator MouseWarning()
    {
        mouseWarningActive = true;
        ShopRefs.mouseWarningPanel.SetActive(true);
        yield return new WaitForSeconds(3f);
        mouseWarningActive = false;
        ShopRefs.mouseWarningPanel.SetActive(false);
    }
}
