using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class ShopScript : MonoBehaviour {

    private Saving_PlayerManager data;

    // Pause UI
    public GameObject gamePausePanel, shopShip1, shopShip2;
    public Button buttonWhenPaused, buttonWhenLeavingPauseBugFix;
    public Text swapP1text, swapP2text;

    // Fading & music
    public GameObject fadeBlack, player2GUI;
    public AudioSource musicLoop;
    private float fadingAlpha = 0f;

    // Player UI
    public Image p1InsurancePowerup, p1FarShotPowerup, p1RetroThrusterPowerup, p1RapidShotPowerup, p1TripleShotPowerup;
    public Image p2InsurancePowerup, p2FarShotPowerup, p2RetroThrusterPowerup, p2RapidShotPowerup, p2TripleShotPowerup;
    const int bonusInterval = 10000;
    public Image p1ShieldBar, p1PowerBar, p2ShieldBar, p2PowerBar;
    public Text p1ScoreText, p1LivesText, p2ScoreText, p2LivesText;
    public TextMeshProUGUI readyPromptText, player2AbsentText;

    // Event System handling
    public EventSystem pauseEventSystem;
    public EventSystemShop p1JoyEvents, p1KeyEvents, p2JoyEvents, p2KeyEvents;

    // Readying before level transition
    public Button p1ReadyButton, p2ReadyButton;
    public bool p1IsReady = false, p2IsReady = false;

    // Shop UI
    public Button p1UpgButton1, p1UpgButton2, p1UpgButton3, p1UpgButton4, p2UpgButton1, p2UpgButton2, p2UpgButton3, p2UpgButton4;

    void Start() {
        if (BetweenScenesScript.MusicVolume > 0f) { musicLoop.Play(); }

        data = Saving_SaveManager.LoadData();
        p1ShieldBar.fillAmount = data.player1health / 80;
        p1ScoreText.text = "Credits:\n" + data.player1credits;
        p1LivesText.text = "Lives: " + data.player1lives;
        if (data.player1powerups[0] == 1) { p1InsurancePowerup.gameObject.SetActive(true); }
        if (data.player1powerups[1] == 1) { p1FarShotPowerup.gameObject.SetActive(true); }
        if (data.player1powerups[2] == 1) { p1RetroThrusterPowerup.gameObject.SetActive(true); }
        if (data.player1powerups[3] == 1) { p1RapidShotPowerup.gameObject.SetActive(true); }
        if (data.player1powerups[4] == 1) { p1TripleShotPowerup.gameObject.SetActive(true); }
        if (data.playerCount == 1) {
            readyPromptText.text = "Press 'Ready' to\nContinue to Level " + (data.level + 1).ToString() + "...";
            p2IsReady = true;
        }
        else if (data.playerCount == 2) {
            player2GUI.SetActive(true); player2AbsentText.gameObject.SetActive(false);
            p2ShieldBar.fillAmount = data.player2health / 80;
            p2ScoreText.text = "Credits:\n" + data.player2credits;
            p2LivesText.text = "Lives: " + data.player2lives;
            if (data.player2powerups[0] == 1) { p2InsurancePowerup.gameObject.SetActive(true); }
            if (data.player2powerups[1] == 1) { p2FarShotPowerup.gameObject.SetActive(true); }
            if (data.player2powerups[2] == 1) { p2RetroThrusterPowerup.gameObject.SetActive(true); }
            if (data.player2powerups[3] == 1) { p2RapidShotPowerup.gameObject.SetActive(true); }
            if (data.player2powerups[4] == 1) { p2TripleShotPowerup.gameObject.SetActive(true); }
            readyPromptText.text = "Both players 'Ready' to\nContinue to Level " + (data.level + 1).ToString() + "...";
        }
        UpdateButtonText();
        UpdateEventSystemChoices();
        StartCoroutine(FadeBlack("from"));
    }

    void Update() {
        // If the pause button is pressed while the game pause panel is active, then return to gameplay.
        // Else, bring up the pause panel.
        if (Input.GetButtonDown("Pause")) {
            if (gamePausePanel.activeInHierarchy) {
                PauseGame(1);
            }
            else {
                PauseGame(0);
            }
        }
    }

    public void ReadyUp(int i) {
        Navigation customNav = new Navigation {
            mode = Navigation.Mode.Explicit };

        Button[] listOfButtons = GameObject.FindObjectsOfType<Button>();
        if (!p1IsReady && i == 1) {
            p1IsReady = true; p1ReadyButton.GetComponentInChildren<Text>().text = "Unready";
            //customNav.selectOnUp = null; p1ReadyButton.GetComponent<Button>().navigation = customNav;

            foreach (Button gameObj in listOfButtons) { // Find all P1 buttons that aren't 'Ready' button, and disable them
                if (gameObj.transform.name.StartsWith("P1") && !gameObj.transform.name.EndsWith("Ready")) {
                    gameObj.GetComponent<Button>().interactable = false;
                }
            }
        }
        else if (p1IsReady && i == 1) {
            p1IsReady = false; p1ReadyButton.GetComponentInChildren<Text>().text = "Ready";
            //customNav.selectOnUp = p1UpgButton4; p1ReadyButton.GetComponent<Button>().navigation = customNav;

            foreach (Button gameObj in listOfButtons) { // Find all P1 buttons that aren't 'Ready' button, and disable them
                if (gameObj.transform.name.StartsWith("P1") && !gameObj.transform.name.EndsWith("Ready")) {
                    gameObj.GetComponent<Button>().interactable = true;
                }
            }
        }
        if (!p2IsReady && i == 2) {
            p2IsReady = true; p2ReadyButton.GetComponentInChildren<Text>().text = "Unready";
            //customNav.selectOnUp = null; p1ReadyButton.GetComponent<Button>().navigation = customNav;

            foreach (Button gameObj in listOfButtons) { // Find all P1 buttons that aren't 'Ready' button, and disable them
                if (gameObj.transform.name.StartsWith("P2") && !gameObj.transform.name.EndsWith("Ready")) {
                    gameObj.GetComponent<Button>().interactable = false;
                }
            }
        }
        else if (p2IsReady && i == 2) {
            p2IsReady = false; p2ReadyButton.GetComponentInChildren<Text>().text = "Ready";
            //customNav.selectOnUp = p2UpgButton4; p1ReadyButton.GetComponent<Button>().navigation = customNav;  

            foreach (Button gameObj in listOfButtons) { // Find all P1 buttons that aren't 'Ready' button, and disable them
                if (gameObj.transform.name.StartsWith("P2") && !gameObj.transform.name.EndsWith("Ready")) {
                    gameObj.GetComponent<Button>().interactable = true;
                }
            }
        }
        if (p1IsReady && p2IsReady) {
            p1ReadyButton.GetComponentInChildren<Text>().text = "";
            p2ReadyButton.GetComponentInChildren<Text>().text = "";
            p1ReadyButton.GetComponent<Button>().interactable = false;
            p2ReadyButton.GetComponent<Button>().interactable = false;
            GoBackToGame();
        }
    }

    public void GoBackToGame() {
        readyPromptText.text = "Prepare for Level " + (data.level + 1).ToString() + "!";
        StartCoroutine(FadeBlack("to"));
        Invoke("LoadMainGame", 1f);
    }
    private void LoadMainGame() {
        SceneManager.LoadScene("MainScene");
    }
    public void BackToMainMenu() {
        Time.timeScale = 1;
        SceneManager.LoadScene("StartMenu");
    }

    /* ------------------------------------------------------------------------------------------------------------------
    * Upgrades code
    * ------------------------------------------------------------------------------------------------------------------ */

    public void PerformUpgrade(int whichUpgrade) {
        if (p1JoyEvents.currentSelectedGameObject.name.StartsWith("P1") || p1KeyEvents.currentSelectedGameObject.name.StartsWith("P1")) {
            BetweenScenesScript.UpgradesP1[whichUpgrade] += 0.1f;
            Debug.Log(string.Join(",", BetweenScenesScript.UpgradesP1));
        }
        else if (p2JoyEvents.currentSelectedGameObject.name.StartsWith("P2") || p2KeyEvents.currentSelectedGameObject.name.StartsWith("P2")) {
            BetweenScenesScript.UpgradesP2[whichUpgrade] += 0.1f;
            Debug.Log(string.Join(",", BetweenScenesScript.UpgradesP2));
        }
        UpdateButtonText();
    }

    /* ------------------------------------------------------------------------------------------------------------------
    * Pause screen code
    * ------------------------------------------------------------------------------------------------------------------ */

    // Swap player 1's controls with this button
    public void SwapP1Controls() {
        if (BetweenScenesScript.ControlTypeP1 == 0) {
            BetweenScenesScript.ControlTypeP1 = 1;
            swapP1text.text = "Swap P1 to gamepad controls";
        }
        else {
            BetweenScenesScript.ControlTypeP1 = 0;
            swapP1text.text = "Swap P1 to keyboard controls";
        }
    }
    // If player 2 exists, then swap their controls with this button
    public void SwapP2Controls() {
        if (BetweenScenesScript.PlayerCount == 2) {
            if (BetweenScenesScript.ControlTypeP2 == 0) {
                BetweenScenesScript.ControlTypeP2 = 1;
                swapP2text.text = "Swap P2 to gamepad controls";
            }
            else {
                BetweenScenesScript.ControlTypeP2 = 0;
                swapP2text.text = "Swap P2 to keyboard controls";
            }
        }
    }

    public void PauseGame(int intent) {
        if (intent == 0) { // Pause game
            
            // This section is Shop-specific. It deactivates all shop-related Event Systems, and enables pause Event System
            pauseEventSystem.GetComponent<StandaloneInputModule>().enabled = true;
            p1JoyEvents.gameObject.SetActive(false);
            p2JoyEvents.gameObject.SetActive(false);
            p1KeyEvents.gameObject.SetActive(false);
            p2KeyEvents.gameObject.SetActive(false);

            musicLoop.Pause();
            gamePausePanel.SetActive(true);
            buttonWhenPaused.Select();
            Time.timeScale = 0;
            CheckPlayerControls();
        }
        else if (intent == 1) { // Resume game

            buttonWhenLeavingPauseBugFix.Select(); // Select it with pause event system, not upcoming event systems

            // This section is Shop-specific. It deactivates pause Event System, and activates only the Event System that matches the control scheme chosen
            UpdateEventSystemChoices();
            pauseEventSystem.GetComponent<StandaloneInputModule>().enabled = false;
            if (BetweenScenesScript.ControlTypeP1 == 0) { p1JoyEvents.gameObject.SetActive(true); }
            else { p1KeyEvents.gameObject.SetActive(true); }
            if (BetweenScenesScript.PlayerCount == 2) {
                if (BetweenScenesScript.ControlTypeP2 == 0) { p2JoyEvents.gameObject.SetActive(true); }
                else { p2KeyEvents.gameObject.SetActive(true); }
            }

            if (BetweenScenesScript.MusicVolume > 0f) { musicLoop.Play(); }
            gamePausePanel.SetActive(false);
            Time.timeScale = 1;
        }
    }
    // Each time the pause menu is used, change the buttons for controller swapping depending on their state
    public void CheckPlayerControls() {
        if (BetweenScenesScript.ControlTypeP1 == 0) { swapP1text.text = "Swap P1 to keyboard controls"; }
        else { swapP1text.text = "Swap P1 to gamepad controls"; }
        if (BetweenScenesScript.PlayerCount == 1) { swapP2text.text = ""; }
        else if (BetweenScenesScript.ControlTypeP2 == 0) { swapP2text.text = "Swap P2 to keyboard controls"; }
        else { swapP2text.text = "Swap P2 to gamepad controls"; }
    }

    /* ------------------------------------------------------------------------------------------------------------------
     * Misc code
     * ------------------------------------------------------------------------------------------------------------------ */
    
    private void UpdateButtonText() {
        Button[] listOfButtons = GameObject.FindObjectsOfType<Button>();
        //int i = 0; int j = 0;
        /*foreach (Button gameObj in listOfButtons) {
            if (gameObj.transform.name.StartsWith("P1") && !gameObj.transform.name.EndsWith("Ready")) {
                gameObj.GetComponentInChildren<Text>().text = "x" + BetweenScenesScript.UpgradesP1[i].ToString() +
                    " (Upgrade: 1000c)";
                i += 1;
            }
            if (gameObj.transform.name.StartsWith("P2") && !gameObj.transform.name.EndsWith("Ready")) {
                gameObj.GetComponentInChildren<Text>().text = "x" + BetweenScenesScript.UpgradesP2[j].ToString() +
                    " (Upgrade: 1000c)";
                j += 1;
            }
        }*/
        p1UpgButton1.GetComponentInChildren<Text>().text = "x" + BetweenScenesScript.UpgradesP1[0].ToString() + " (Upgrade: 1000c)";
        p1UpgButton2.GetComponentInChildren<Text>().text = "x" + BetweenScenesScript.UpgradesP1[1].ToString() + " (Upgrade: 1000c)";
        p1UpgButton3.GetComponentInChildren<Text>().text = "x" + BetweenScenesScript.UpgradesP1[2].ToString() + " (Upgrade: 1000c)";
        p1UpgButton4.GetComponentInChildren<Text>().text = "x" + BetweenScenesScript.UpgradesP1[3].ToString() + " (Upgrade: 1000c)";
        p2UpgButton1.GetComponentInChildren<Text>().text = "x" + BetweenScenesScript.UpgradesP2[0].ToString() + " (Upgrade: 1000c)";
        p2UpgButton2.GetComponentInChildren<Text>().text = "x" + BetweenScenesScript.UpgradesP2[1].ToString() + " (Upgrade: 1000c)";
        p2UpgButton3.GetComponentInChildren<Text>().text = "x" + BetweenScenesScript.UpgradesP2[2].ToString() + " (Upgrade: 1000c)";
        p2UpgButton4.GetComponentInChildren<Text>().text = "x" + BetweenScenesScript.UpgradesP2[3].ToString() + " (Upgrade: 1000c)";
    }


    // Find every button that matches player number, and tell it to update expected event systems
    private void UpdateEventSystemChoices() {
        Button[] listOfButtons = GameObject.FindObjectsOfType<Button>();
        foreach (Button gameObj in listOfButtons) {
            if (gameObj.transform.name.StartsWith("P1")) {
                gameObj.GetComponent<MyEventSystemProvider>().DetermineControls();
            }
        }

        if (player2GUI.activeInHierarchy) {
            listOfButtons = GameObject.FindObjectsOfType<Button>();
            foreach (Button gameObj in listOfButtons) {
                if (gameObj.transform.name.StartsWith("P2")) {
                    gameObj.GetComponent<MyEventSystemProvider>().DetermineControls();
                }
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
            speedOfFade = 1.2f;
            while (fadingAlpha < 1f) {
                fadingAlpha += speedOfFade * Time.deltaTime;
                tempFade.color = new Color(origColor.r, origColor.g, origColor.b, fadingAlpha);
                yield return null;
            }
        }
    }
}
