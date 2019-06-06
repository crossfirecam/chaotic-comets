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
    private EventSystem p1SelectedSystem, p2SelectedSystem;

    // Readying before level transition
    public Button p1ReadyButton, p2ReadyButton;
    public bool p1IsReady = false, p2IsReady = false;

    // Shop UI
    public Button p1UpgButton0, p1UpgButton1, p1UpgButton2, p1UpgButton3, p2UpgButton0, p2UpgButton1, p2UpgButton2, p2UpgButton3;
    private int baseUpgradePrice = 500, priceIncreasePerLevel = 500, upgradeCap = 15;

    void Start() {
        if (BetweenScenesScript.MusicVolume > 0f) { musicLoop.Play(); }

        // Nullexception is possible here, but only if shop is loaded without a save file. In typical gameplay it isn't possible
        data = Saving_SaveManager.LoadData();
        p1ShieldBar.fillAmount = data.player1health / 80;
        BetweenScenesScript.player1TempCredits = data.player1credits;
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
            BetweenScenesScript.player2TempCredits = data.player2credits;
            p2LivesText.text = "Lives: " + data.player2lives;
            if (data.player2powerups[0] == 1) { p2InsurancePowerup.gameObject.SetActive(true); }
            if (data.player2powerups[1] == 1) { p2FarShotPowerup.gameObject.SetActive(true); }
            if (data.player2powerups[2] == 1) { p2RetroThrusterPowerup.gameObject.SetActive(true); }
            if (data.player2powerups[3] == 1) { p2RapidShotPowerup.gameObject.SetActive(true); }
            if (data.player2powerups[4] == 1) { p2TripleShotPowerup.gameObject.SetActive(true); }
            readyPromptText.text = "Both players 'Ready' to\nContinue to Level " + (data.level + 1).ToString() + "...";
        }

        SetActiveEventSystems(1);
        // If space or K is held during level transition, the player will instantly ready up. This small section of code prevents
        // that by calling the Ready button to activate one frame later.
        StartCoroutine(DelayedReady());

        UpdateButtonText();
        UpdateEventSystemChoices();
        StartCoroutine(FadeBlack("from"));
        Cursor.visible = false;
    }

    private IEnumerator DelayedReady() {
        p1SelectedSystem.SetSelectedGameObject(buttonWhenLeavingPauseBugFix.gameObject);
        if (BetweenScenesScript.PlayerCount == 2) {
            p2SelectedSystem.SetSelectedGameObject(buttonWhenLeavingPauseBugFix.gameObject);
        }
        yield return new WaitForSeconds(0.2f);
        p1SelectedSystem.SetSelectedGameObject(p1ReadyButton.gameObject);
        if (BetweenScenesScript.PlayerCount == 2) {
            p2SelectedSystem.SetSelectedGameObject(p2ReadyButton.gameObject);
        }
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
        // Each frame, check what button is highlighted. Pull a button back into focus if mouse clicks away from a button.
        // If the mouse is used to click auto highlight away, then drag a highlight back onto a certain button.
        if (p1SelectedSystem.gameObject.activeInHierarchy) {
            if (p1SelectedSystem.currentSelectedGameObject == null || p1SelectedSystem.currentSelectedGameObject.Equals(null)) {
                p1SelectedSystem.SetSelectedGameObject(p1ReadyButton.gameObject);
            }
        }
        if (BetweenScenesScript.PlayerCount == 2 && p2SelectedSystem.gameObject.activeInHierarchy) {
            if (p2SelectedSystem.currentSelectedGameObject == null || p2SelectedSystem.currentSelectedGameObject.Equals(null)) {
                p2SelectedSystem.SetSelectedGameObject(p2ReadyButton.gameObject);
            }
        }
    }

    // Code for shop's Ready system. If both players are ready, shop closes.
    public void ReadyUp(int i) {
        // 
        Navigation customNav = new Navigation {
            mode = Navigation.Mode.Explicit };
        Button[] listOfButtons = GameObject.FindObjectsOfType<Button>();


        if (!p1IsReady && i == 1) {
            p1IsReady = true; p1ReadyButton.GetComponentInChildren<Text>().text = "Unready";
            customNav.selectOnUp = null; p1ReadyButton.GetComponent<Button>().navigation = customNav;

            foreach (Button gameObj in listOfButtons) { // Find all P1 buttons that aren't 'Ready' button, and disable them
                if (gameObj.transform.name.StartsWith("P1") && !gameObj.transform.name.EndsWith("Ready")) {
                    gameObj.GetComponent<Button>().interactable = false;
                }
            }
        }
        else if (p1IsReady && i == 1) {
            p1IsReady = false; p1ReadyButton.GetComponentInChildren<Text>().text = "Ready";
            customNav.selectOnUp = p1UpgButton3; p1ReadyButton.GetComponent<Button>().navigation = customNav;

            foreach (Button gameObj in listOfButtons) { // Find all P1 buttons that aren't 'Ready' button, and disable them
                if (gameObj.transform.name.StartsWith("P1") && !gameObj.transform.name.EndsWith("Ready")) {
                    gameObj.GetComponent<Button>().interactable = true;
                }
            }
        }

        if (!p2IsReady && i == 2) {
            p2IsReady = true; p2ReadyButton.GetComponentInChildren<Text>().text = "Unready";
            customNav.selectOnUp = null; p1ReadyButton.GetComponent<Button>().navigation = customNav;

            foreach (Button gameObj in listOfButtons) { // Find all P1 buttons that aren't 'Ready' button, and disable them
                if (gameObj.transform.name.StartsWith("P2") && !gameObj.transform.name.EndsWith("Ready")) {
                    gameObj.GetComponent<Button>().interactable = false;
                }
            }
        }
        else if (p2IsReady && i == 2) {
            p2IsReady = false; p2ReadyButton.GetComponentInChildren<Text>().text = "Ready";
            customNav.selectOnUp = p2UpgButton3; p1ReadyButton.GetComponent<Button>().navigation = customNav;  

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
    * Upgrades code.
    * They must be separate functions due to how Unity handles button press scripting. Only one variable can be passed per button.
    * ------------------------------------------------------------------------------------------------------------------ */

    public void PerformUpgradeP1(int whichUpgrade) {
        if (p1SelectedSystem.currentSelectedGameObject.name.StartsWith("P1")) {
            if (BetweenScenesScript.UpgradesP1[whichUpgrade] < upgradeCap) {
                int price = baseUpgradePrice + priceIncreasePerLevel * (BetweenScenesScript.UpgradesP1[whichUpgrade] - 10);
                if (BetweenScenesScript.player1TempCredits > price) {
                    BetweenScenesScript.UpgradesP1[whichUpgrade] += 1;
                    BetweenScenesScript.player1TempCredits -= price;
                    Debug.Log("Upgrades: " + string.Join(",", BetweenScenesScript.UpgradesP1) + " Credits left: " + BetweenScenesScript.player1TempCredits);
                }
                else {
                    Debug.Log("Upgrade failed, not enough credits");
                }
            }
        }
        UpdateButtonText();
    }

    public void PerformUpgradeP2(int whichUpgrade) {
        if (p2SelectedSystem.currentSelectedGameObject.name.StartsWith("P2")) {
            if (BetweenScenesScript.UpgradesP2[whichUpgrade] < upgradeCap) {
                int price = baseUpgradePrice + priceIncreasePerLevel * (BetweenScenesScript.UpgradesP2[whichUpgrade] - 10);
                if (BetweenScenesScript.player2TempCredits > price) {
                    BetweenScenesScript.UpgradesP2[whichUpgrade] += 1;
                    BetweenScenesScript.player2TempCredits -= price;
                    Debug.Log("Upgrades: " + string.Join(",", BetweenScenesScript.UpgradesP2) + " Credits left: " + BetweenScenesScript.player2TempCredits);
                }
                else {
                    Debug.Log("Upgrade failed, not enough credits");
                }
            }
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
            SetActiveEventSystems(0);

            musicLoop.Pause();
            gamePausePanel.SetActive(true);
            buttonWhenPaused.Select();
            Time.timeScale = 0;
            CheckPlayerControls();
            Cursor.visible = true;
        }
        else if (intent == 1) { // Resume game

            buttonWhenLeavingPauseBugFix.Select(); // Select it with pause event system, not upcoming event systems

            // This section is Shop-specific. It deactivates pause Event System, and activates only the Event System that matches the control scheme chosen
            UpdateEventSystemChoices();
            SetActiveEventSystems(1);

            if (BetweenScenesScript.MusicVolume > 0f) { musicLoop.Play(); }
            gamePausePanel.SetActive(false);
            Time.timeScale = 1;
            Cursor.visible = false;
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
    
    private void SetActiveEventSystems(int intent) {
        if (intent == 0) {
            pauseEventSystem.GetComponent<StandaloneInputModule>().enabled = true;
            p1JoyEvents.gameObject.SetActive(false);
            p2JoyEvents.gameObject.SetActive(false);
            p1KeyEvents.gameObject.SetActive(false);
            p2KeyEvents.gameObject.SetActive(false);
        }
        else if (intent == 1) {
            pauseEventSystem.GetComponent<StandaloneInputModule>().enabled = false;
            if (BetweenScenesScript.ControlTypeP1 == 0) {
                p1JoyEvents.gameObject.SetActive(true);
                p1SelectedSystem = p1JoyEvents;
            }
            else {
                p1KeyEvents.gameObject.SetActive(true);
                p1SelectedSystem = p1KeyEvents;
            }
            if (BetweenScenesScript.PlayerCount == 2) {
                if (BetweenScenesScript.ControlTypeP2 == 0) {
                    p2JoyEvents.gameObject.SetActive(true);
                    p2SelectedSystem = p2JoyEvents;
}
                else {
                    p2KeyEvents.gameObject.SetActive(true);
                    p2SelectedSystem = p2KeyEvents;
                }
            }
        }
    }

    // If a button pressed begins with 'P1' or 'P2', and does not end with 'Ready', then update the button's text based on BetweenScenesScript variables
    private void UpdateButtonText() {
        Button[] listOfButtons = GameObject.FindObjectsOfType<Button>();
        foreach (Button gameObj in listOfButtons) {
            string tempName = gameObj.transform.name;
            tempName = tempName.Substring(tempName.Length - 1, 1);
            if (int.TryParse(tempName, out int i)) {
                i = int.Parse(tempName);
            }
            int priceP1 = baseUpgradePrice + priceIncreasePerLevel * (BetweenScenesScript.UpgradesP1[i] - 10);
            int priceP2 = baseUpgradePrice + priceIncreasePerLevel * (BetweenScenesScript.UpgradesP2[i] - 10);
            string upgradeTier; int tempUpgradeNumLength;
            if (gameObj.transform.name.StartsWith("P1") && !gameObj.transform.name.EndsWith("Ready")) {
                tempUpgradeNumLength = BetweenScenesScript.UpgradesP1[i].ToString().Length - 1;
                upgradeTier = BetweenScenesScript.UpgradesP1[i].ToString().Insert(tempUpgradeNumLength, ".");
                gameObj.GetComponentInChildren<Text>().text = "Current: x" + upgradeTier + "\n(Upgrade: " + priceP1.ToString() + "c)";
                if (BetweenScenesScript.UpgradesP1[i] == upgradeCap) {
                    gameObj.GetComponentInChildren<Text>().text = "Current: x" + upgradeTier + "\n(Maximum upgrade)";
                }
            }
            else if (gameObj.transform.name.StartsWith("P2") && !gameObj.transform.name.EndsWith("Ready")) {
                tempUpgradeNumLength = BetweenScenesScript.UpgradesP2[i].ToString().Length - 1;
                upgradeTier = BetweenScenesScript.UpgradesP2[i].ToString().Insert(tempUpgradeNumLength, ".");
                gameObj.GetComponentInChildren<Text>().text = "Current: x" + upgradeTier + "\n(Upgrade: " + priceP2.ToString() + "c)";
                if (BetweenScenesScript.UpgradesP2[i] == upgradeCap) {
                    gameObj.GetComponentInChildren<Text>().text = "Current: x" + upgradeTier + "\n(Maximum upgrade)";
                }
            }
            p1ScoreText.text = "Credits:\n" + BetweenScenesScript.player1TempCredits;
            if (data.playerCount == 2) { p2ScoreText.text = "Credits:\n" + BetweenScenesScript.player2TempCredits; }
        }
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
