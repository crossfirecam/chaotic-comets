using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using Rewired;

public partial class MainMenu : MonoBehaviour
{
    public AudioMixer mixer;
    public GameObject fadeBlack;
    private float fadingAlpha = 0f;
    private MusicManager musicManager;
    public GameObject musicManagerIfNotFoundInScene;

    // ----------

    private void Start() {
        Cursor.visible = true;
        ResetBetweenScenesScript();
        ChangeScoreTypeAndPopulate(PlayerPrefs.GetInt("ScorePreference", 0));

        musicManager = FindObjectOfType<MusicManager>();
        if (!musicManager)
        {
            Instantiate(musicManagerIfNotFoundInScene);
            musicManager = FindObjectOfType<MusicManager>();
        }
        musicManager.sfxDemo = optionSFXSlider.GetComponent<AudioSource>();
        musicManager.ChangeMusicTrack(0);

        mixer.SetFloat("MusicVolume", Mathf.Log10(BetweenScenesScript.MusicVolume) * 20);
        mixer.SetFloat("SFXVolume", Mathf.Log10(BetweenScenesScript.SFXVolume) * 20);
        StartCoroutine(FadeBlack("from"));
    }

    private void Update() {
        CheckHighlightedButton();
    }

    /* ------------------------------------------------------------------------------------------------------------------
     * Other functions
     * ------------------------------------------------------------------------------------------------------------------ */

    private IEnumerator FadeBlack(string ToOrFrom) {
        Image tempFade = fadeBlack.GetComponent<Image>();
        Color origColor = tempFade.color;
        float speedOfFade = 1.6f;
        fadeBlack.SetActive(true);
        if (ToOrFrom == "from") {
            fadingAlpha = 1f;
            while (fadingAlpha > 0f) {
                fadingAlpha -= speedOfFade * Time.deltaTime;
                tempFade.color = new Color(origColor.r, origColor.g, origColor.b, fadingAlpha);
                yield return null;
            }
            fadeBlack.SetActive(false);
        }
        else if (ToOrFrom == "to") {
            fadingAlpha = 0f;
            while (fadingAlpha < 1f) {
                fadingAlpha += speedOfFade * Time.deltaTime;
                tempFade.color = new Color(origColor.r, origColor.g, origColor.b, fadingAlpha);
                yield return null;
            }
        }
    }
}
