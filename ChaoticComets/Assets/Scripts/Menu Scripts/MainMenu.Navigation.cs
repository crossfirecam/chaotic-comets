using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using Rewired;

public partial class MainMenu : MonoBehaviour
{

    public GameObject saveOptionDialog, difficultyDialog, mainMenuPanel, optionsDialog;

    // Options screen
    public Button btnFullscreenToggle;
    public Slider optionMusicSlider, optionSFXSlider;

    // Save confirmation screen
    public Text saveDescriptionText;

    // Difficulty selection screen
    public Text difficultyTitleText, difficultyText;
    public Button diffBackButton, diffEasyButton, diffNormalButton, diffHardButton;
    private readonly string diffTitle0 = "Select difficulty", diffTitle1 = "Select difficulty\n(both players)";
    private readonly string diffText0 = "Go back to main menu.";
    private readonly string diffText1 = "Ship is always equipped with Auto-Brake and comes to a complete stop. No manual braking required.";
    private readonly string diffText2 = "Standard gameplay. Try other difficulties to change ship's handling.";
    private readonly string diffText3 = "Ship's manual brake and Auto-Brake are less effective. A real test of maneuverability.";


    // Misc UI objects
    public Button returnToMenuButton, controlFirstButton, saveFirstButton;
    public GameObject buttonCon3, player1pIcon, player2pIcon;


    private void CheckForBackButton()
    {
        // TODO
    }

    // Check for a save file, and set correct player icon/text
    public void CheckForSaveFile(int i)
    {
        BetweenScenesScript.PlayerCount = i;
        player1pIcon.SetActive(false);
        player2pIcon.SetActive(false);

        // If save found, show save prompt
        // If none found, show difficulty prompt
        if (Saving_SaveManager.LoadData() != null)
        {
            ShowSavePrompt();
        }
        else
        {
            ShowDifficulties();
        }
    }

    // Display a prompt to make a new game or resume saved game
    public void ShowSavePrompt()
    {
        Saving_PlayerManager data = Saving_SaveManager.LoadData();
        string tempSavePlayerCount = data.playerCount.ToString() + " Player";
        if (data.playerCount == 2) { tempSavePlayerCount += "s"; }
        string tempSaveDifficulty = data.difficulty.ToString();
        if (tempSaveDifficulty == "0") { tempSaveDifficulty = "Easy"; }
        if (tempSaveDifficulty == "1") { tempSaveDifficulty = "Normal"; }
        if (tempSaveDifficulty == "2") { tempSaveDifficulty = "Hard"; }
        string tempSaveLevel = $"Level {data.level + 1}";
        saveDescriptionText.text = $"{tempSavePlayerCount},\n{tempSaveDifficulty}, {tempSaveLevel}";
        mainMenuPanel.SetActive(false);
        saveOptionDialog.SetActive(true);
        saveFirstButton.Select();
    }

    public void SaidNoToResuming(int i)
    {
        BetweenScenesScript.PlayerCount = i;
        BetweenScenesScript.ResumingFromSave = false;
        ShowDifficulties();
    }

    // Display a prompt to select difficulty
    public void ShowDifficulties()
    {
        saveOptionDialog.SetActive(false);
        difficultyDialog.SetActive(true);
        diffNormalButton.Select();
        mainMenuPanel.SetActive(false);
        if (BetweenScenesScript.PlayerCount == 1) { difficultyTitleText.text = diffTitle0; player1pIcon.SetActive(true); }
        else if (BetweenScenesScript.PlayerCount == 2) { difficultyTitleText.text = diffTitle1; player2pIcon.SetActive(true); }
    }

    public void SetDifficultyAndStart(int i)
    {
        if (i != -1)
        { // If difficulty is not already set by the save file, set difficulty from player choice
            BetweenScenesScript.Difficulty = i;
        }
        else
        { // If loaded from a save, difficulty is preset
            BetweenScenesScript.ResumingFromSave = true;
            Saving_PlayerManager data = Saving_SaveManager.LoadData();
            BetweenScenesScript.Difficulty = data.difficulty;
            BetweenScenesScript.PlayerCount = data.playerCount;
        }
        StartGame();
    }

    /* ------------------------------------------------------------------------------------------------------------------
     * Options Screen functions
     * ------------------------------------------------------------------------------------------------------------------ */


    public void ShowOptions()
    {
        mainMenuPanel.SetActive(false);
        optionsDialog.SetActive(true);
        btnFullscreenToggle.Select();
        SetBtnFullscreenText();
        optionMusicSlider.SetValueWithoutNotify(BetweenScenesScript.MusicVolume);
        optionSFXSlider.SetValueWithoutNotify(BetweenScenesScript.SFXVolume);
    }

    public void SwapFullscreen()
    {
        if (Screen.fullScreen)
        {
            Screen.SetResolution(Screen.currentResolution.width / 2, Screen.currentResolution.height / 2, false);
        }
        else
        {
            Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, true);
        }
        Invoke("SetBtnFullscreenText", 0.1f);
    }

    public void SetBtnFullscreenText()
    {
        if (Screen.fullScreen)
        {
            btnFullscreenToggle.GetComponentInChildren<Text>().text = "Fullscreen ON";
        }
        else
        {
            btnFullscreenToggle.GetComponentInChildren<Text>().text = "Fullscreen OFF";
        }
    }


    /* ------------------------------------------------------------------------------------------------------------------
     * BackTo functions
     * Changes UI on main menu as player progresses backward
     * ------------------------------------------------------------------------------------------------------------------ */
    public void BackToMenu()
    {
        saveOptionDialog.SetActive(false);
        difficultyDialog.SetActive(false);
        optionsDialog.SetActive(false);
        mainMenuPanel.SetActive(true);
        returnToMenuButton.Select();
        player1pIcon.SetActive(true);
        player2pIcon.SetActive(true);
    }

    /* ------------------------------------------------------------------------------------------------------------------
     * Update per-frame functions
     * ------------------------------------------------------------------------------------------------------------------ */
    private void OnMenuGoBack()
    {
        if (difficultyDialog.activeInHierarchy) { BackToMenu(); }
        if (saveOptionDialog.activeInHierarchy) { BackToMenu(); }
        if (optionsDialog.activeInHierarchy) { BackToMenu(); }
    }

    private void CheckHighlightedButton()
    {
        // Each frame, check what button is highlighted. Change difficulty descriptions, or pull a button back into focus if mouse clicks away from a button.
        if (EventSystem.current.currentSelectedGameObject != null && !EventSystem.current.currentSelectedGameObject.Equals(null))
        {
            if (EventSystem.current.currentSelectedGameObject.name == "Diff-BackButton") { difficultyText.text = diffText0; }
            else if (EventSystem.current.currentSelectedGameObject.name == "Diff-EasyButton") { difficultyText.text = diffText1; }
            else if (EventSystem.current.currentSelectedGameObject.name == "Diff-NormalButton") { difficultyText.text = diffText2; }
            else if (EventSystem.current.currentSelectedGameObject.name == "Diff-HardButton") { difficultyText.text = diffText3; }
        }
        // If the mouse is used to click auto highlight away, then drag a highlight back onto a certain button.
        else
        {
            if (difficultyDialog.activeInHierarchy) { diffNormalButton.Select(); }
            else if (saveOptionDialog.activeInHierarchy) { saveFirstButton.Select(); }
            else if (optionsDialog.activeInHierarchy) { btnFullscreenToggle.Select(); }
            else { returnToMenuButton.Select(); }
        }
    }
}
