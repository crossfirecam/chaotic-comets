using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class OptionsPanel : MonoBehaviour
{
    public Button btnFullscreenToggle, optionsFirstButton;
    public Slider optionMusicSlider, optionSFXSlider;
    public GameObject cheatDisclaimer, cheatDisclaimerResumeSave;

    public void Awake()
    {
        // Find the SFX slider in Options, and set default values (for some reason GetFloat's defaultValue wouldn't work...)
        MusicManager.i.sfxDemo = optionSFXSlider.GetComponent<AudioSource>();
    }
    public void ShowOptionsPanel()
    {
        optionsFirstButton.Select();
        SetBtnFullscreenText();
        optionMusicSlider.SetValueWithoutNotify(PlayerPrefs.GetFloat("Music"));
        optionSFXSlider.SetValueWithoutNotify(PlayerPrefs.GetFloat("SFX"));
    }

    public void SwapFullscreen()
    {
        if (Screen.fullScreen)
            Screen.SetResolution(Screen.currentResolution.width / 2, Screen.currentResolution.height / 2, false);
        else
            Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, true);
        Invoke(nameof(SetBtnFullscreenText), 0.1f);
    }

    private void SetBtnFullscreenText()
    {
        if (Screen.fullScreen)
            btnFullscreenToggle.GetComponentInChildren<TextMeshProUGUI>().text = "Fullscreen ON";
        else
            btnFullscreenToggle.GetComponentInChildren<TextMeshProUGUI>().text = "Fullscreen OFF";
    }

    public void ToggleCheats(bool toggleValue)
    {
        BetweenScenes.CheaterMode = toggleValue;
        cheatDisclaimer.SetActive(toggleValue);
        cheatDisclaimerResumeSave.SetActive(toggleValue);
    }

    public void ChangeMusicPassToManager(float musVolume)
    {
        MusicManager.i.ChangeMusic(musVolume);
    }
    public void ChangeSFXPassToManager(float sfxVolume)
    {
        MusicManager.i.ChangeSFX(sfxVolume);
    }
}
