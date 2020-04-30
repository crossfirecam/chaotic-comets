using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.Audio;
using TMPro;

public class MainMenuScript : MonoBehaviour {

    // UI objects
    private readonly float verticalMenuMovement;
    public Text difficultyTitleText, difficultyText, buttonCon1Text, buttonCon2Text, buttonCon3Text;
    public GameObject saveOptionDialog, difficultyDialog, mainMenuPanel, controlOptionDialog, optionsDialog;
    public Button diffBackButton, diffEasyButton, diffNormalButton, diffHardButton;
    public Button returnToMenuButton, returnToDifficultyButton, controlFirstButton, saveFirstButton;
    public GameObject buttonCon3, player1pIcon, player2pIcon;
    public GameObject mainMenuCanvas, mainMenuDecor;

    // Save confirmation screen
    public Text saveDescriptionText;

    // Difficulty selection screen
    public TextMeshProUGUI diffSelectedText;
    public GameObject diffSelectedObject;
    private readonly string diffTitle0 = "Select difficulty", diffTitle1 = "Select difficulty\n(both players)";
    private readonly string diffText0 = "Go back to main menu.";
    private readonly string diffText1 = "Ship is always equipped with Auto-Brake and comes to a near-complete stop. No manual braking required.";
    private readonly string diffText2 = "Standard gameplay. Try other difficulties to change ship's handling.";
    private readonly string diffText3 = "Ship's manual brake and Auto-Brake are less effective. A real test of maneuverability.";

    // Controller selection screen
    private readonly string conText1a = "P1 Keyboard", conText2a = "P1 Gamepad"; // One Player selected
    private readonly string conText1b = "P1 Keyboard, P2 Keyboard", conText2b = "P1 Gamepad, P2 Keyboard", conText3b = "P1 Gamepad, P2 Gamepad"; // Two Player selected
    private readonly string diffSelect0 = "Easy Mode", diffSelect1 = "Normal Mode", diffSelect2 = "Hard Mode", diffSelect9 = "Control Test";

    // Options screen
    public Slider optionMusicSlider, optionSFXSlider;

    // Other
    public AudioMixer mixer;
    public GameObject fadeBlack;
    private float fadingAlpha = 0f;
    private bool controllerFound = false, keyboardFound = false;

    // ----------

    private void Start() {
        Cursor.visible = true;
        BetweenScenesScript.ResumingFromSave = false; // Set to false first, in case game is closed while save is being loaded
        BetweenScenesScript.MusicVolume = PlayerPrefs.GetFloat("Music");
        BetweenScenesScript.SFXVolume = PlayerPrefs.GetFloat("SFX");
        BetweenScenesScript.player1TempCredits = 0; // Reset temporary credit & lives count to 0. These will be set if a store is loaded and progressed past
        BetweenScenesScript.player2TempCredits = 0;
        BetweenScenesScript.player1TempLives = 0;
        BetweenScenesScript.player2TempLives = 0;
        mixer.SetFloat("MusicVolume", Mathf.Log10(BetweenScenesScript.MusicVolume) * 20);
        mixer.SetFloat("SFXVolume", Mathf.Log10(BetweenScenesScript.SFXVolume) * 20);
        StartCoroutine(FadeBlack("from"));
    }

    private void Update() {
        if (Input.GetButtonDown("MenuNavCancel") && difficultyDialog.activeInHierarchy) { BackToMenu(); }
        if (Input.GetButtonDown("MenuNavCancel") && saveOptionDialog.activeInHierarchy) { BackToMenu(); }
        if (Input.GetButtonDown("MenuNavCancel") && optionsDialog.activeInHierarchy) { BackToMenu(); }
        else if (Input.GetButtonDown("MenuNavCancel") && controlOptionDialog.activeInHierarchy && BetweenScenesScript.Difficulty == 9) { BackToMenu(); }
        else if (Input.GetButtonDown("MenuNavCancel") && controlOptionDialog.activeInHierarchy) { BackToDifficulty(); }
        // Each frame, check what button is highlighted. Change difficulty descriptions, or pull a button back into focus if mouse clicks away from a button.
        if (EventSystem.current.currentSelectedGameObject != null && !EventSystem.current.currentSelectedGameObject.Equals(null)) {
            if (EventSystem.current.currentSelectedGameObject.name == "Diff-BackButton") { difficultyText.text = diffText0; }
            else if (EventSystem.current.currentSelectedGameObject.name == "Diff-EasyButton") { difficultyText.text = diffText1; }
            else if (EventSystem.current.currentSelectedGameObject.name == "Diff-NormalButton") { difficultyText.text = diffText2; }
            else if (EventSystem.current.currentSelectedGameObject.name == "Diff-HardButton") { difficultyText.text = diffText3; }
        }
        // If the mouse is used to click auto highlight away, then drag a highlight back onto a certain button.
        else {
            if (difficultyDialog.activeInHierarchy) { diffNormalButton.Select(); }
            else if (saveOptionDialog.activeInHierarchy) { saveFirstButton.Select(); }
            else if (controlOptionDialog.activeInHierarchy) { controlFirstButton.Select(); }
            else if (optionsDialog.activeInHierarchy) { optionMusicSlider.Select(); }
            else { returnToMenuButton.Select(); }
        }
        CheckForControllerOrKeyboard();
    }

    /* ------------------------------------------------------------------------------------------------------------------
     * Menu navigation functions
     * ------------------------------------------------------------------------------------------------------------------ */

    // Check for a save file, and set correct player icon/text
    public void CheckForSaveFile(int i) {
        BetweenScenesScript.PlayerCount = i;
        player1pIcon.SetActive(false);
        player2pIcon.SetActive(false);

        // If save found, show save prompt
        // If none found, show difficulty prompt
        if (Saving_SaveManager.LoadData() != null) {
            ShowSavePrompt(); }
        else {
            ShowDifficulties(); }
    }

    // Display a prompt to make a new game or resume saved game
    public void ShowSavePrompt() {
        Saving_PlayerManager data = Saving_SaveManager.LoadData();
        string tempSavePlayerCount = data.playerCount.ToString() + " Player";
        if (data.playerCount == 2) { tempSavePlayerCount += "s"; }
        string tempSaveDifficulty = data.difficulty.ToString();
        if (tempSaveDifficulty == "0") { tempSaveDifficulty = "Easy"; }
        if (tempSaveDifficulty == "1") { tempSaveDifficulty = "Normal"; }
        if (tempSaveDifficulty == "2") { tempSaveDifficulty = "Hard"; }
        string tempSaveLevel = $"Level {(data.level + 1).ToString()}";
        saveDescriptionText.text = $"{tempSavePlayerCount},\n{tempSaveDifficulty}, {tempSaveLevel}";
        mainMenuPanel.SetActive(false);
        saveOptionDialog.SetActive(true);
        saveFirstButton.Select();
    }

    public void SaidNoToResuming(int i) {
        if (i == 1) { BetweenScenesScript.PlayerCount = 1; }
        else if (i == 2) { BetweenScenesScript.PlayerCount = 2; }
        BetweenScenesScript.ResumingFromSave = false;
        ShowDifficulties();
    }

    // Display a prompt to select difficulty
    public void ShowDifficulties() {
        saveOptionDialog.SetActive(false);
        difficultyDialog.SetActive(true);
        diffNormalButton.Select();
        mainMenuPanel.SetActive(false);
        if (BetweenScenesScript.PlayerCount == 1) { difficultyTitleText.text = diffTitle0; player1pIcon.SetActive(true); }
        else if (BetweenScenesScript.PlayerCount == 2) { difficultyTitleText.text = diffTitle1; player2pIcon.SetActive(true); }
    }

    public void SetDifficultyAndGoToControls(int i) {
        if (i != -1) { // If difficulty is not already set by the save file, set difficulty from player choice
            if (i == 9) { BetweenScenesScript.Difficulty = 9; } // ... Or if control test selected, set to 9 for interpretation in ShowControlOptions()
            else { BetweenScenesScript.Difficulty = i; }
        }
        else { // If loaded from a save, difficulty is preset
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
    // If i = 9, then this was called from the Control Test button. This value isn't used in gameplay,
    // instead just to tell the Update loop here if a B press needs to send the user back to main menu instead of difficulty panel.
    public void ShowControlOptions() {
        saveOptionDialog.SetActive(false);
        diffSelectedObject.SetActive(true);
        if (BetweenScenesScript.Difficulty == 0) { diffSelectedText.text = diffSelect0; }
        if (BetweenScenesScript.Difficulty == 1) { diffSelectedText.text = diffSelect1; }
        if (BetweenScenesScript.Difficulty == 2) { diffSelectedText.text = diffSelect2; }
        if (BetweenScenesScript.Difficulty == 9) { diffSelectedText.text = diffSelect9; player1pIcon.SetActive(false); BetweenScenesScript.PlayerCount = 2; }
        controlOptionDialog.SetActive(true);
        difficultyDialog.SetActive(false);
        mainMenuPanel.SetActive(false);
        controlFirstButton.Select();
        if (BetweenScenesScript.PlayerCount == 1) {
            diffSelectedObject.gameObject.transform.localPosition = new Vector3(-131f, 14f);
            buttonCon1Text.text = conText1a; buttonCon2Text.text = conText2a; buttonCon3.SetActive(false);
        }
        else if (BetweenScenesScript.PlayerCount == 2) {
            diffSelectedObject.gameObject.transform.localPosition = new Vector3(-131f, 72f);
            buttonCon1Text.text = conText1b; buttonCon2Text.text = conText2b; buttonCon3.SetActive(true); buttonCon3Text.text = conText3b;
        }
    }

    public void ShowOptions() {
        mainMenuPanel.SetActive(false);
        optionsDialog.SetActive(true);
        optionMusicSlider.Select();
        optionMusicSlider.SetValueWithoutNotify(BetweenScenesScript.MusicVolume);
        optionSFXSlider.SetValueWithoutNotify(BetweenScenesScript.SFXVolume);
    }

    // Both BackTo... functions change UI on main menu as player progresses backward
    public void BackToMenu() {
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
    public void BackToDifficulty() {
        if (BetweenScenesScript.Difficulty == 9 || BetweenScenesScript.ResumingFromSave == true) {
            BackToMenu();
            BetweenScenesScript.ResumingFromSave = false;
        }
        else {
            diffSelectedObject.SetActive(false);
            controlOptionDialog.SetActive(false);
            difficultyDialog.SetActive(true);
            diffNormalButton.Select();
        }
    }

    /* ------------------------------------------------------------------------------------------------------------------
     * Scene loading functions
     * ------------------------------------------------------------------------------------------------------------------ */

    // Start game, depending on control selections
    public void StartGame(int i) {
        if (BetweenScenesScript.PlayerCount == 1) {
            if (i == 0) { BetweenScenesScript.ControlTypeP1 = 1; }
            else { BetweenScenesScript.ControlTypeP1 = 0; }
        }
        if (BetweenScenesScript.PlayerCount == 2) {
            if (i == 0) {
                BetweenScenesScript.ControlTypeP1 = 1;
                BetweenScenesScript.ControlTypeP2 = 1;
            }
            else if (i == 1) {
                BetweenScenesScript.ControlTypeP1 = 0;
                BetweenScenesScript.ControlTypeP2 = 1;
            }
            else {
                BetweenScenesScript.ControlTypeP1 = 0;
                BetweenScenesScript.ControlTypeP2 = 0;
            }
        }
        StartCoroutine(FadeBlack("to"));
        Invoke("LoadScene", 1f);
    }

    private void LoadScene() {
        // If control panel is called from main menu, then load control test.
        // If control panel is called while resuming a save, then load store before next level
        // Else, load as if a new game was started
        if (BetweenScenesScript.Difficulty == 9) {
            SceneManager.LoadScene("ControlsMenu");
        }
        else if (BetweenScenesScript.ResumingFromSave == true) {
            SceneManager.LoadScene("ShopMenu");
        }
        else { // If a new game is started, then erase old data
            Saving_SaveManager.EraseData();
            SceneManager.LoadScene("MainScene");
        }
    }

    public void VisitHelp() {
        SceneManager.LoadScene("HelpMenu");
    }

    public void VisitAbout() {
        SceneManager.LoadScene("AboutMenu");
    }

    public void EndGame() {
        Application.Quit();
    }

    /* ------------------------------------------------------------------------------------------------------------------
     * Other functions
     * ------------------------------------------------------------------------------------------------------------------ */

    public void CheckForControllerOrKeyboard() {
        if (!controllerFound) {
            if (Input.GetKeyDown(KeyCode.Joystick1Button0) || Input.GetKeyDown(KeyCode.Joystick1Button1) || Input.GetKeyDown(KeyCode.Joystick1Button2) ||
                Input.GetKeyDown(KeyCode.Joystick1Button3) || Input.GetKeyDown(KeyCode.Joystick1Button4) || Input.GetKeyDown(KeyCode.Joystick1Button5) ||
                Input.GetKeyDown(KeyCode.Joystick1Button6) || Input.GetKeyDown(KeyCode.Joystick1Button7) || Input.GetKeyDown(KeyCode.Joystick1Button8) ||
                Input.GetKeyDown(KeyCode.Joystick1Button9) || Input.GetAxisRaw("Rotate Ship (P1joy)") != 0 || Input.GetAxisRaw("Thrust (P1joy)") != 0) {

                Debug.Log("Controller main input detected");
                controllerFound = true;
                keyboardFound = false;
            }
        }
        if (!keyboardFound) {
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.S) ||
                Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.Space) ||
                Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow) ||
                Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.K) || Input.GetKeyDown(KeyCode.L)) {

                Debug.Log("Keyboard main input detected");
                controllerFound = false;
                keyboardFound = true;
            }
        }
    }

    private IEnumerator FadeBlack(string ToOrFrom) {
        Image tempFade = fadeBlack.GetComponent<Image>();
        Color origColor = tempFade.color;
        float speedOfFade = 0.8f;
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
            speedOfFade = 1.6f;
            while (fadingAlpha < 1f) {
                fadingAlpha += speedOfFade * Time.deltaTime;
                tempFade.color = new Color(origColor.r, origColor.g, origColor.b, fadingAlpha);
                yield return null;
            }
        }
    }
}
