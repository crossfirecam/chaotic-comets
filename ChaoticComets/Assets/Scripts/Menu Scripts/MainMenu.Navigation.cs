using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public partial class MainMenu : MonoBehaviour
{

    public GameObject saveOptionDialog, difficultyDialog, mainMenuPanel, controlOptionDialog, optionsDialog;

    // Options screen
    public Button btnFullscreenToggle;
    public Slider optionMusicSlider, optionSFXSlider;

    // Save confirmation screen
    public Text saveDescriptionText;

    // Difficulty selection screen
    public Text difficultyTitleText, difficultyText;
    public Button diffBackButton, diffEasyButton, diffNormalButton, diffHardButton;
    public TextMeshProUGUI diffSelectedText;
    public GameObject diffSelectedObject;
    private readonly string diffTitle0 = "Select difficulty", diffTitle1 = "Select difficulty\n(both players)";
    private readonly string diffText0 = "Go back to main menu.";
    private readonly string diffText1 = "Ship is always equipped with Auto-Brake and comes to a near-complete stop. No manual braking required.";
    private readonly string diffText2 = "Standard gameplay. Try other difficulties to change ship's handling.";
    private readonly string diffText3 = "Ship's manual brake and Auto-Brake are less effective. A real test of maneuverability.";

    // Controller selection screen
    private bool controllerP1Found = false, controllerP2Found = false, keyboardFound = true; // Default to both players on keyboard
    public Text buttonCon1Text, buttonCon2Text, buttonCon3Text;
    private readonly string conText1a = "P1 Keyboard", conText2a = "P1 Gamepad"; // One Player selected
    private readonly string conText1b = "P1 Keyboard, P2 Keyboard", conText2b = "P1 Gamepad, P2 Keyboard", conText3b = "P1 Gamepad, P2 Gamepad"; // Two Player selected
    private readonly string diffSelect0 = "Easy Mode", diffSelect1 = "Normal Mode", diffSelect2 = "Hard Mode";

    // Misc UI objects
    public Button returnToMenuButton, controlFirstButton, saveFirstButton;
    public GameObject buttonCon3, player1pIcon, player2pIcon;


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

    public void SetDifficultyAndGoToControls(int i)
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
            if (BetweenScenesScript.PlayerCount == 1) { player1pIcon.SetActive(true); }
            else if (BetweenScenesScript.PlayerCount == 2) { player2pIcon.SetActive(true); }
        }
        ShowControlOptions();
    }

    // When difficulty is selected, save to memory. Bring up control selection screen
    public void ShowControlOptions()
    {
        saveOptionDialog.SetActive(false);
        diffSelectedObject.SetActive(true);

        if (BetweenScenesScript.Difficulty == 0) { diffSelectedText.text = diffSelect0; }
        if (BetweenScenesScript.Difficulty == 1) { diffSelectedText.text = diffSelect1; }
        if (BetweenScenesScript.Difficulty == 2) { diffSelectedText.text = diffSelect2; }
        
        controlOptionDialog.SetActive(true);
        difficultyDialog.SetActive(false);
        mainMenuPanel.SetActive(false);
        controlFirstButton.Select();


        if (BetweenScenesScript.PlayerCount == 1)
        {
            diffSelectedObject.gameObject.transform.localPosition = new Vector3(-131f, 14f);
            buttonCon1Text.text = conText1a;
            buttonCon2Text.text = conText2a;
            buttonCon3.SetActive(false);
        }
        else if (BetweenScenesScript.PlayerCount == 2)
        {
            diffSelectedObject.gameObject.transform.localPosition = new Vector3(-131f, 72f);
            buttonCon1Text.text = conText1b;
            buttonCon2Text.text = conText2b;
            buttonCon3.SetActive(true);
            buttonCon3Text.text = conText3b;
        }
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
        diffSelectedObject.SetActive(false);
        difficultyDialog.SetActive(false);
        optionsDialog.SetActive(false);
        controlOptionDialog.SetActive(false);
        mainMenuPanel.SetActive(true);
        returnToMenuButton.Select();
        player1pIcon.SetActive(true);
        player2pIcon.SetActive(true);
    }
    public void BackToDifficulty()
    {
        if (BetweenScenesScript.ResumingFromSave == true)
        {
            BackToMenu();
            BetweenScenesScript.ResumingFromSave = false;
        }
        else
        {
            diffSelectedObject.SetActive(false);
            controlOptionDialog.SetActive(false);
            difficultyDialog.SetActive(true);
            diffNormalButton.Select();
        }
    }

    /* ------------------------------------------------------------------------------------------------------------------
     * Update per-frame functions
     * ------------------------------------------------------------------------------------------------------------------ */
    private void CheckForMenuNavCancel()
    {
        if (Input.GetButtonDown("MenuNavCancel") && difficultyDialog.activeInHierarchy) { BackToMenu(); }
        if (Input.GetButtonDown("MenuNavCancel") && saveOptionDialog.activeInHierarchy) { BackToMenu(); }
        if (Input.GetButtonDown("MenuNavCancel") && optionsDialog.activeInHierarchy) { BackToMenu(); }
        else if (Input.GetButtonDown("MenuNavCancel") && controlOptionDialog.activeInHierarchy) { BackToDifficulty(); }
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
            else if (controlOptionDialog.activeInHierarchy) { controlFirstButton.Select(); }
            else if (optionsDialog.activeInHierarchy) { btnFullscreenToggle.Select(); }
            else { returnToMenuButton.Select(); }
        }
    }
    public void CheckForControllerOrKeyboard()
    {
        if (!controllerP1Found)
        {
            if (Input.GetKeyDown(KeyCode.Joystick1Button0) || Input.GetKeyDown(KeyCode.Joystick1Button1) || Input.GetKeyDown(KeyCode.Joystick1Button2) ||
                Input.GetKeyDown(KeyCode.Joystick1Button3) || Input.GetKeyDown(KeyCode.Joystick1Button4) || Input.GetKeyDown(KeyCode.Joystick1Button5) ||
                Input.GetKeyDown(KeyCode.Joystick1Button6) || Input.GetKeyDown(KeyCode.Joystick1Button7) || Input.GetKeyDown(KeyCode.Joystick1Button8) ||
                Input.GetKeyDown(KeyCode.Joystick1Button9) || Input.GetAxisRaw("Rotate Ship (P1joy)") != 0 || Input.GetAxisRaw("Thrust (P1joy)") != 0)
            {

                Debug.Log("Controller main input detected");
                controllerP1Found = true;
            }
        }
        if (!controllerP2Found)
        {
            if (Input.GetKeyDown(KeyCode.Joystick2Button0) || Input.GetKeyDown(KeyCode.Joystick2Button1) || Input.GetKeyDown(KeyCode.Joystick2Button2) ||
                Input.GetKeyDown(KeyCode.Joystick2Button3) || Input.GetKeyDown(KeyCode.Joystick2Button4) || Input.GetKeyDown(KeyCode.Joystick2Button5) ||
                Input.GetKeyDown(KeyCode.Joystick2Button6) || Input.GetKeyDown(KeyCode.Joystick2Button7) || Input.GetKeyDown(KeyCode.Joystick2Button8) ||
                Input.GetKeyDown(KeyCode.Joystick2Button9) || Input.GetAxisRaw("Rotate Ship (P2joy)") != 0 || Input.GetAxisRaw("Thrust (P2joy)") != 0)
            {

                Debug.Log("Controller main input detected");
                controllerP2Found = true;
            }
        }
        if (!keyboardFound)
        {
            if (Event.current.isKey || Event.current.isMouse)
            {
                Debug.Log("Keyboard main input detected");
                keyboardFound = true;
            }
        }
    }
}
