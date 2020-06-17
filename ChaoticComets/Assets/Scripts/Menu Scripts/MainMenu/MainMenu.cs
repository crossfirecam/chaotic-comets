using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public partial class MainMenu : MonoBehaviour
{
    public AudioMixer mixer;
    public GameObject fadeBlack;
    private float fadingAlpha = 0f;
    private MusicManager musicManager;
    public GameObject musicManagerIfNotFoundInScene;
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
            Button buttonToSelect = mainMenuPanel.transform.Find(BetweenScenes.BackToMainMenuButton + "Button").GetComponent<Button>();
            buttonToSelect.Select();
            audioMenuBack.Play();
            BetweenScenes.BackToMainMenuButton = "";
        }
        else
        {
            StartCoroutine(FadeBlack("from"));
        }
    }

    private void Update() {
        CheckHighlightedButton();
    }

    /* ------------------------------------------------------------------------------------------------------------------
     * Other functions
     * ------------------------------------------------------------------------------------------------------------------ */
    private bool alreadyFading = false;
    private IEnumerator FadeBlack(string ToOrFrom) {
        if (!alreadyFading)
        {
            alreadyFading = true;
            Image tempFade = fadeBlack.GetComponent<Image>();
            Color origColor = tempFade.color;
            float speedOfFade = 2f;
            fadeBlack.SetActive(true);
            if (ToOrFrom == "from")
            {
                fadingAlpha = 1f;
                while (fadingAlpha > 0f)
                {
                    fadingAlpha -= speedOfFade * Time.deltaTime;
                    tempFade.color = new Color(origColor.r, origColor.g, origColor.b, fadingAlpha);
                    yield return null;
                }
                fadeBlack.SetActive(false);
            }
            else if (ToOrFrom == "to")
            {
                fadingAlpha = 0f;
                while (fadingAlpha < 1f)
                {
                    fadingAlpha += speedOfFade * Time.deltaTime;
                    tempFade.color = new Color(origColor.r, origColor.g, origColor.b, fadingAlpha);
                    yield return null;
                }
            }
            yield return new WaitForSeconds(1);
            alreadyFading = false;
        }
    }

    private void StartupSoundManagement()
    {
        // Find Music Manager & audio source for moving back in Main Menu UI
        audioMenuBack = GetComponent<AudioSource>();
        musicManager = FindObjectOfType<MusicManager>();
        if (!musicManager)
        {
            Instantiate(musicManagerIfNotFoundInScene);
            musicManager = FindObjectOfType<MusicManager>();
        }

        // Find the SFX slider in Options, and set default values (for some reason GetFloat's defaultValue wouldn't work...)
        musicManager.sfxDemo = optionSFXSlider.GetComponent<AudioSource>();
        if (!PlayerPrefs.HasKey("Music"))
        {
            PlayerPrefs.SetFloat("Music", 0.8f);
            PlayerPrefs.SetFloat("SFX", 0.8f);
            PlayerPrefs.Save();
        }

        // Change music to main menu track, set volumes
        musicManager.ChangeMusicTrack(0);
        float musicVol = PlayerPrefs.GetFloat("Music");
        float sfxVol = PlayerPrefs.GetFloat("SFX");
        mixer.SetFloat("MusicVolume", Mathf.Log10(musicVol) * 20);
        mixer.SetFloat("SFXVolume", Mathf.Log10(sfxVol) * 20);
    }
}
