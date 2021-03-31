using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public partial class MainMenu : MonoBehaviour
{
    [Header("Main Menu Misc")]
    public AudioMixer mixer;
    public GameObject fadeBlack;
    private AudioSource audioMenuBack;

    // ----------
    private void Start()
    {
        StartupSoundManagement();
        UsefulFunctions.ResetBetweenScenesScript();
        ChangeScoreTypeAndPopulate(PlayerPrefs.GetInt("ScorePreference", 0));

        // Check most recently used controller every .2 seconds
        StartCoroutine(UsefulFunctions.CheckController());

        // If returning to main menu from About or Help, play a sound and select the button. If not, then fade the screen in.
        if (BetweenScenes.BackToMainMenuButton != "")
        {
            Button buttonToSelect = mainMenuPanel.Find(BetweenScenes.BackToMainMenuButton + "Button").GetComponent<Button>();
            buttonToSelect.Select();
            audioMenuBack.Play();
            BetweenScenes.BackToMainMenuButton = "";
        }
        else
        {
            StartCoroutine(FadeBlack("from"));
        }
    }

    /* ------------------------------------------------------------------------------------------------------------------
     * Other functions
     * ------------------------------------------------------------------------------------------------------------------ */
    private bool fadingInAlready = false;
    private float fadingAlpha = 0f;
    private IEnumerator FadeBlack(string ToOrFrom) {
        // If 'fade to' is trigger during 'fade from', this variable stops the while loop that does 'fade from'
        if (!fadingInAlready)
        {
            fadingInAlready = true;
            fadingAlpha = 0f;
        }

        Image tempFade = fadeBlack.GetComponent<Image>();
        Color origColor = tempFade.color;
        float speedOfFade = 2f;
        fadeBlack.SetActive(true);
        if (ToOrFrom == "from")
        {
            fadingAlpha = 1f;
            while (fadingAlpha > 0f && fadingInAlready)
            {
                fadingAlpha -= speedOfFade * Time.deltaTime;
                tempFade.color = new Color(origColor.r, origColor.g, origColor.b, fadingAlpha);
                yield return null;
            }
            fadeBlack.SetActive(false);
            fadingInAlready = false;
        }
        else if (ToOrFrom == "to")
        {
            while (fadingAlpha < 1f)
            {
                fadingAlpha += speedOfFade * Time.deltaTime;
                tempFade.color = new Color(origColor.r, origColor.g, origColor.b, fadingAlpha);
                yield return null;
            }
        }
        yield return new WaitForSeconds(1);
    }

    private void StartupSoundManagement()
    {
        // Find Music Manager & audio source for moving back in Main Menu UI
        audioMenuBack = GetComponent<AudioSource>();

        // Find the SFX slider in Options, and set default values (for some reason GetFloat's defaultValue wouldn't work...)
        MusicManager.i.sfxDemo = optionsPanel.optionSFXSlider.GetComponent<AudioSource>();
        if (!PlayerPrefs.HasKey("Music"))
        {
            PlayerPrefs.SetFloat("Music", 0.8f);
            PlayerPrefs.SetFloat("SFX", 0.8f);
            PlayerPrefs.Save();
        }

        // Change music to main menu track, set volumes
        MusicManager.i.ChangeMusicTrack(0);
        float musicVol = PlayerPrefs.GetFloat("Music");
        float sfxVol = PlayerPrefs.GetFloat("SFX");
        mixer.SetFloat("MusicVolume", Mathf.Log10(musicVol) * 20);
        mixer.SetFloat("SFXVolume", Mathf.Log10(sfxVol) * 20);
    }
}
