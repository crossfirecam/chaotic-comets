using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public partial class MainMenu : MonoBehaviour
{

    public GameObject saveOptionDialog, difficultyDialog, mainMenuPanel, optionsDialog;

    // Options screen
    public Button btnFullscreenToggle;
    public Slider optionMusicSlider, optionSFXSlider;
    public GameObject cheatDisclaimer, cheatDisclaimerResumeSave;

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
    public Button returnToMenuButton, saveFirstButton, optionsFirstButton;
    public Transform playerDecorDiffDialog;

    /* ------------------------------------------------------------------------------------------------------------------
     * Save Dialog - Checking for save file, and showing dialog
     * ------------------------------------------------------------------------------------------------------------------ */
    // Check for a save file, and set correct player icon/text
    public void CheckForSaveFile(int plrAmountRequested)
    {
        BetweenScenes.PlayerCount = plrAmountRequested;

        // If save found, and cheat mode flag is not found in the save, show save prompt
        // If none found, show difficulty prompt
        if (Saving_SaveManager.LoadData() != null && !Saving_SaveManager.LoadData().isCheatModeOn)
        {
            ShowSavePrompt();
        }
        else
        {
            ShowDifficulties();
        }
    }

    // Display a prompt to make a new game or resume saved game
    private Text saveDialogHeader, saveDialogInfo, saveDialogNewGame;
    public void ShowSavePrompt()
    {
        SetBackButton();
        Saving_PlayerManager data = Saving_SaveManager.LoadData();

        if (saveDialogHeader == null)
        {
            saveDialogHeader = saveOptionDialog.transform.Find("SaveDialog").Find("SaveHeader").GetComponent<Text>();
            saveDialogInfo = saveOptionDialog.transform.Find("SaveDialog").Find("SaveInfo").GetComponent<Text>();
            saveDialogNewGame = saveOptionDialog.transform.Find("SaveDialog").Find("SaveNewBtn").GetComponentInChildren<Text>();
        }

        string savePlayerCount = $"{(data.playerCount == 1 ? "One" : "Two")}";
        saveDialogHeader.text = $"{savePlayerCount} Player Mode\nAuto-Save Found";
        saveDialogNewGame.text = $"New {savePlayerCount} Player Game";

        string saveDifficulty;
        #pragma warning disable IDE0066 // Cannot use switch statement in Unity, unfortunately
        switch (data.difficulty)
        #pragma warning restore IDE0066 // Cannot use switch statement in Unity, unfortunately
        {
            case 0: saveDifficulty = "Easy"; break;
            case 1: saveDifficulty = "Normal"; break;
            case 2: saveDifficulty = "Hard"; break;
            default: saveDifficulty = "Error"; break;
        }
        string saveLevel = $"Level {data.level + 1}";
        saveDialogInfo.text = $"{saveDifficulty}, {saveLevel}";

        saveOptionDialog.SetActive(true);
        saveFirstButton.Select();
    }

    public void SaidNoToResuming()
    {
        BetweenScenes.ResumingFromSave = false;
        ShowDifficulties();
    }

    /* ------------------------------------------------------------------------------------------------------------------
     * Difficulty Select
     * ------------------------------------------------------------------------------------------------------------------ */
    // Display a prompt to select difficulty
    public void ShowDifficulties()
    {
        SetBackButton();
        // Dismiss save dialog if open
        saveOptionDialog.SetActive(false);

        // Open difficulty dialog, select Normal button by default
        difficultyDialog.SetActive(true);
        diffNormalButton.Select();

        // Depending on player count, change aesthetics
        if (BetweenScenes.PlayerCount == 1) {
            difficultyTitleText.text = diffTitle0;
            playerDecorDiffDialog.Find("1P").gameObject.SetActive(true);
            playerDecorDiffDialog.Find("2P").gameObject.SetActive(false);
        }
        else if (BetweenScenes.PlayerCount == 2) {
            difficultyTitleText.text = diffTitle1;
            playerDecorDiffDialog.Find("1P").gameObject.SetActive(false);
            playerDecorDiffDialog.Find("2P").gameObject.SetActive(true);
        }
    }

    public void SetDifficultyAndStart(int i)
    {
        // If difficulty is not already set by the save file, set difficulty from player choice
        if (i != -1)
            BetweenScenes.Difficulty = i;
        // If loaded from a save, difficulty is preset
        else
        {
            BetweenScenes.ResumingFromSave = true;
            BetweenScenes.CheaterMode = false;
            Saving_PlayerManager data = Saving_SaveManager.LoadData();
            BetweenScenes.Difficulty = data.difficulty;
            BetweenScenes.PlayerCount = data.playerCount;
        }
        StartGame();
    }

    /* ------------------------------------------------------------------------------------------------------------------
     * Options Screen
     * ------------------------------------------------------------------------------------------------------------------ */
    public void ShowOptions()
    {
        SetBackButton();
        optionsDialog.SetActive(true);
        optionsFirstButton.Select();
        SetBtnFullscreenText();
        optionMusicSlider.SetValueWithoutNotify(PlayerPrefs.GetFloat("Music"));
        optionSFXSlider.SetValueWithoutNotify(PlayerPrefs.GetFloat("SFX"));
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

    private void SetBtnFullscreenText()
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

    public void ToggleCheats(bool toggleValue)
    {
        BetweenScenes.CheaterMode = toggleValue;
        cheatDisclaimer.SetActive(toggleValue);
        cheatDisclaimerResumeSave.SetActive(toggleValue);
    }


    /* ------------------------------------------------------------------------------------------------------------------
     * Back To Main Menu - Returns to main menu and cancels all other dialogs, if the game is not fading to black.
     * ------------------------------------------------------------------------------------------------------------------ */
    public void BackToMenu()
    {
        if (!fadeBlack.activeInHierarchy)
        {
            audioMenuBack.Play();
            saveOptionDialog.SetActive(false);
            difficultyDialog.SetActive(false);
            optionsDialog.SetActive(false);
            resetScoresDialog.SetActive(false);

            mainMenuPanel.SetActive(true);
            returnToMenuButton.Select();
            returnToMenuButton = null;
        }
    }
    // When entering a panel from main menu, return to the button pressed later.
    private void SetBackButton()
    {
        if (returnToMenuButton == null)
        {
            returnToMenuButton = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
        }
    }

    /* ------------------------------------------------------------------------------------------------------------------
     * Other Nav functions
     * ------------------------------------------------------------------------------------------------------------------ */
    public void OnMenuGoBack()
    {
        if (difficultyDialog.activeInHierarchy) { BackToMenu(); }
        if (saveOptionDialog.activeInHierarchy) { BackToMenu(); }
        if (optionsDialog.activeInHierarchy) { BackToMenu(); }
        if (resetScoresDialog.activeInHierarchy) { BackToMenu(); }
    }

    private void CheckHighlightedButton()
    {
        // Each frame, check what button is highlighted. Change difficulty descriptions.
        if (EventSystem.current.currentSelectedGameObject != null && !EventSystem.current.currentSelectedGameObject.Equals(null))
        {
            if (EventSystem.current.currentSelectedGameObject.name == "Diff-BackButton") { difficultyText.text = diffText0; }
            else if (EventSystem.current.currentSelectedGameObject.name == "Diff-EasyButton") { difficultyText.text = diffText1; }
            else if (EventSystem.current.currentSelectedGameObject.name == "Diff-NormalButton") { difficultyText.text = diffText2; }
            else if (EventSystem.current.currentSelectedGameObject.name == "Diff-HardButton") { difficultyText.text = diffText3; }
        }
        else
        {
            returnToMenuButton.Select();
        }
    }
}
