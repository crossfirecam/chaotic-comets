using Rewired.Integration.UnityUI;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public partial class ShopScript : MonoBehaviour
{
    private void PlayMusicIfEnabled()
    {
        // Change music track
        musicManager = FindObjectOfType<MusicManager>();
        if (!musicManager)
        {
            Instantiate(ShopRefs.musicManagerIfNotFoundInScene);
            musicManager = FindObjectOfType<MusicManager>();
        }

        musicManager.ChangeMusicTrack(2);

        if (BetweenScenesScript.MusicVolume > 0f)
        {
            musicManager.currentMusicPlayer.Play();
        }
    }

    /* ------------------------------------------------------------------------------------------------------------------
    * Pause & FadeBlack code
    * ------------------------------------------------------------------------------------------------------------------ */


    public void PauseGame(int intent)
    {
        if (intent == 0)
        { // Pause game

            ShopRefs.p1Events.gameObject.SetActive(false);
            ShopRefs.p2Events.gameObject.SetActive(false);
            ShopRefs.pauseEventSystem.GetComponent<RewiredStandaloneInputModule>().enabled = true;

            if (musicManager != null)
            {
                musicManager.PauseMusic();
            }
            ShopRefs.gamePausePanel.SetActive(true);
            ShopRefs.buttonWhenPaused.Select();
            Time.timeScale = 0;
        }
        else if (intent == 1)
        { // Resume game

            ShopRefs.buttonWhenLeavingPauseBugFix.Select(); // Select it with pause event system, not upcoming event systems
            ShopRefs.pauseEventSystem.GetComponent<RewiredStandaloneInputModule>().enabled = false;
            ShopRefs.p1Events.gameObject.SetActive(true);
            if (ShopRefs.player2GUI.activeInHierarchy)
                ShopRefs.p2Events.gameObject.SetActive(true);

            if (PlayerPrefs.GetFloat("Music") > 0f && musicManager != null)
            {
                musicManager.ResumeMusic();
            }
            ShopRefs.gamePausePanel.SetActive(false);
            Time.timeScale = 1;
        }
    }

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
}
