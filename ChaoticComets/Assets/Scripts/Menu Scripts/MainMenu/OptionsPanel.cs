using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class OptionsPanel : MonoBehaviour
{
    [SerializeField] private Button btnFullscreenToggle, optionsFirstButton;
    [SerializeField] private Slider optionMusicSlider;
    public Slider optionSFXSlider;
    [SerializeField] private Toggle rainbowModeToggle;
    [SerializeField] private GameObject cheatDisclaimer, cheatDisclaimerResumeSave;


    public void ShowOptionsPanel()
    {
        optionsFirstButton.Select();
        SetBtnFullscreenText();
        optionMusicSlider.SetValueWithoutNotify(PlayerPrefs.GetFloat("Music"));
        optionSFXSlider.SetValueWithoutNotify(PlayerPrefs.GetFloat("SFX"));
        SetRainbowModeToggleOnLoad();
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

    private void SetRainbowModeToggleOnLoad()
    {
        rainbowModeToggle.SetIsOnWithoutNotify(PlayerPrefs.GetInt("RainbowMode") == 1);
    }

    public void ToggleRainbowAsteroidMode(bool toggleValue)
    {
        PlayerPrefs.SetInt("RainbowMode", toggleValue ? 1 : 0);
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
