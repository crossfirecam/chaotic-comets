using Rewired.Integration.UnityUI;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public partial class ShopScript : MonoBehaviour
{
    private void PlayMusicIfEnabled()
    {

        // Change music track
        MusicManager.i.ChangeMusicTrack(2);

        if (PlayerPrefs.GetFloat("Music") > 0f)
        {
            MusicManager.i.currentMusicPlayer.Play();
        }
    }

    /* ------------------------------------------------------------------------------------------------------------------
    * Pausing
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
            if (PlayerPrefs.GetFloat("Music") > 0f)
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

    /* ------------------------------------------------------------------------------------------------------------------
    * FadeBlack
    * ------------------------------------------------------------------------------------------------------------------ */
    private IEnumerator FadeBlack(string ToOrFrom)
    {
        Image tempFade = ShopRefs.fadeBlack.GetComponent<Image>();
        Color origColor = tempFade.color;
        float speedOfFade = 0.8f;
        ShopRefs.fadeBlack.SetActive(true);
        if (ToOrFrom == "from")
        {
            fadingAlpha = 1f;
            while (fadingAlpha > 0f)
            {
                fadingAlpha -= speedOfFade * Time.deltaTime;
                tempFade.color = new Color(origColor.r, origColor.g, origColor.b, fadingAlpha);
                yield return null;
            }
            ShopRefs.fadeBlack.SetActive(false);
        }
        else if (ToOrFrom == "to")
        {
            fadingAlpha = 0f;
            speedOfFade = 1.2f;
            while (fadingAlpha < 1f)
            {
                fadingAlpha += speedOfFade * Time.deltaTime;
                tempFade.color = new Color(origColor.r, origColor.g, origColor.b, fadingAlpha);
                yield return null;
            }
        }
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
