using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour {

    // General purpose variables
    public bool instantkillAsteroids = false;
    public int asteroidCount;
    public int levelNo = 0;
    public bool player1dead = false, player2dead = true; // Only one player by default
    public bool player1TEMPDEAD = false, player2TEMPDEAD = false; // Only used to alert UFO that player is temporarily inactive

    // Gameplay related objects
    public GameObject largeAsteroid, alienShip, canister;
    public GameObject playerShip1, playerShip2;
    private float ufoAmountSpawned, canisterAmountSpawned, propCap; // Variables used to track how many props have, and can spawn.

    // UI related objects
    public GameObject fadeBlack, player2GUI;
    public GameObject gameOverPanel, gamePausePanel, gameLevelPanel;
    public GameObject gameLevelShieldRechargeText;
    public Text swapP1text, swapP2text;
    public Button buttonWhenPaused, buttonWhenGameOver, buttonWhenLeavingPauseBugFix;

    // Other variables
    public float screenTop, screenBottom, screenLeft, screenRight;
    public bool helpMenuMode = false; // Not in control help screen by default
    public AudioSource musicLoop;
    private float fadingAlpha = 0f;
    
    /* ------------------------------------------------------------------------------------------------------------------
     * Start, Update, other regularly used functions
     * ------------------------------------------------------------------------------------------------------------------ */

    void Start() {
        Cursor.visible = false;
        // If in normal gameplay, set player ships to become active and start gameplay
        if (!helpMenuMode) {
            if (BetweenScenesScript.MusicVolume > 0f) {
                musicLoop.Play();
            }
            if (BetweenScenesScript.PlayerCount == 2) {
                player2GUI.SetActive(true);
                playerShip2.SetActive(true);
                player2dead = false;
            }
            if (BetweenScenesScript.ResumingFromSave) { // If resuming from save file, read from save file first
                Saving_PlayerManager data = Saving_SaveManager.LoadData();
                levelNo = data.level;
                // Tell ships to 'play dead' (disable sprite & colliders) if previous shop says they're dead
                if (BetweenScenesScript.player1TempLives == 0) {
                    playerShip1.SendMessage("PretendShipDoesntExist"); }
                if (BetweenScenesScript.player2TempLives == 0 && BetweenScenesScript.PlayerCount == 2) {
                    playerShip2.SendMessage("PretendShipDoesntExist"); }
            }
            StartCoroutine(FadeBlack("from"));
            Invoke("StartNewLevel", 0f);
        }
    }

    void Update() {
        // Rotate the UFO for help screen purposes
        if (helpMenuMode) {
            alienShip.transform.rotation = Quaternion.Euler(-50, 0, 0);
        }
        // Each frame, check if pause menu is open, and what button is highlighted.
        if (gamePausePanel.activeInHierarchy || gameOverPanel.activeInHierarchy) {
            // If the mouse is used to click auto highlight away, then drag a highlight back onto a certain button.
            // If on game quit panel, then select the resume button. If on game over panel, select play again button.
            if (EventSystem.current.currentSelectedGameObject == null || EventSystem.current.currentSelectedGameObject.Equals(null)) {
                if (gamePausePanel.activeInHierarchy) { buttonWhenPaused.Select(); }
                else { buttonWhenGameOver.Select(); }
            }
        }
        // If the pause button is pressed while the game pause panel is active, then return to gameplay.
        // Else, bring up the pause panel.
        if (Input.GetButtonDown("Pause") && !gameOverPanel.activeInHierarchy) {
            if (gamePausePanel.activeInHierarchy) {
                PauseGame(1);
            }
            else {
                PauseGame(0);
            }
        }
    }

    // If in normal gameplay, update asteroids for each one destroyed.
    public void UpdateNumberAsteroids (int change) {
        if (!helpMenuMode) {
            asteroidCount += change;
            if (asteroidCount == 0) {
                Invoke("EndLevelFanFare", 2f);
            }
        }
    }

    public void AlienAndPowerupLogic(string reason) {
        // Alien has 10-15sec to spawn the first time.
        // Alien has 15-30sec to spawn all following times.
        // Canister has 8-20sec to spawn the first time.
        // Canister has 20-40sec to spawn all following times.
        
        float[] alienFirstTimeArray = { 10f, 15f };
        float[] alienSubsequentArray = { 15f, 30f };
        float[] canisterFirstTimeArray = { 8f, 16f };
        float[] canisterSubsequentArray = { 20f, 40f };
        float[] chosenArray = { }; // Left blank to be filled by one of the above
        float minTime; float maxTime;

        if (reason == "initialAlienSetup") {
            chosenArray = alienFirstTimeArray; }
        else if (reason == "initialPowerupSetup") {
            chosenArray = canisterFirstTimeArray; }
        else if (reason == "alienRespawn") {
            chosenArray = alienSubsequentArray; }
        else { // (reason == "powerupRespawn")
            chosenArray = canisterSubsequentArray; }
        
        minTime = chosenArray[0];
        maxTime = chosenArray[1];

        float randomiser = Random.Range(0f, 1f);
        float randomTime = Random.Range(minTime, maxTime);

        if ((reason == "initialAlienSetup" || reason == "alienRespawn") && ufoAmountSpawned < propCap) {
            if (levelNo > 1) { // Alien will not appear on lvl 1
                ufoAmountSpawned += 1;
                Debug.Log("Next UFO will spawn in: " + randomTime + ". Only " + (propCap - ufoAmountSpawned) + " more can spawn.");
                Invoke("RespawnAlien", randomTime);
            }
        }
        else if ((reason == "initialPowerupSetup" || reason == "powerupRespawn") && canisterAmountSpawned < propCap) {
            canisterAmountSpawned += 1;
            Debug.Log("Next canister will spawn in: " + randomTime + ". Only " + (propCap - canisterAmountSpawned) + " more can spawn.");
            Invoke("RespawnCanister", randomTime);
        }
    }

    // When alien or powerup is required, call SpawnProp
    public void RespawnAlien() { SpawnProp("ufo"); }
    public void RespawnCanister() { SpawnProp("canister"); }

    // If a player dies, set the value for their death. If both are dead, game is over
    public void PlayerDied(int playerThatDied) {
        if (playerThatDied == 1) { player1dead = true; }
        else if (playerThatDied == 2) { player2dead = true; }
        if (player1dead && player2dead) {
            Invoke("GameOver", 2f);
        }
    }

    // When either ship is destroyed, alien will change target
    public void PlayerLostLife(int playerNumber) {
        if (playerNumber == 1) { player1TEMPDEAD = true; }
        else if (playerNumber == 2) { player2TEMPDEAD = true; }
        GameObject[] listOfUfos = GameObject.FindGameObjectsWithTag("ufo");
        foreach (GameObject ufo in listOfUfos) {
            ufo.SendMessage("PlayerDied");
        }
    }

    /* ------------------------------------------------------------------------------------------------------------------
     * Pause screen code
     * ------------------------------------------------------------------------------------------------------------------ */

    // Swap player 1's controls with this button
    public void SwapP1Controls() {
        if (BetweenScenesScript.ControlTypeP1 == 0) {
            BetweenScenesScript.ControlTypeP1 = 1;
            swapP1text.text = "Swap P1 to gamepad controls"; }
        else {
            BetweenScenesScript.ControlTypeP1 = 0;
            swapP1text.text = "Swap P1 to keyboard controls"; }
    }
    // If player 2 exists, then swap their controls with this button
    public void SwapP2Controls() {
        if (BetweenScenesScript.PlayerCount == 2) {
            if (BetweenScenesScript.ControlTypeP2 == 0) {
                BetweenScenesScript.ControlTypeP2 = 1;
                swapP2text.text = "Swap P2 to gamepad controls"; }
            else {
                BetweenScenesScript.ControlTypeP2 = 0;
                swapP2text.text = "Swap P2 to keyboard controls"; }
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

    public void PauseGame(int intent) {
        if (intent == 0) { // Pause game
            Cursor.visible = true;
            if (!player1dead) { playerShip1.SendMessage("CheckSounds", 1); }
            if (!player2dead) { playerShip2.SendMessage("CheckSounds", 1); }

            musicLoop.Pause();
            GameObject[] listOfObjects = GameObject.FindGameObjectsWithTag("ufo");
            foreach (GameObject gameObj in listOfObjects) { gameObj.SendMessage("CheckSounds", 1); }

            gamePausePanel.SetActive(true);
            buttonWhenPaused.Select();
            Time.timeScale = 0;
            CheckPlayerControls();
        }
        else if (intent == 1) { // Resume game
            Cursor.visible = false;
            if (!player1dead) { playerShip1.SendMessage("CheckSounds", 2); playerShip1.SendMessage("InputChoice"); }
            if (!player2dead) { playerShip2.SendMessage("CheckSounds", 2); playerShip2.SendMessage("InputChoice"); }

            if (BetweenScenesScript.MusicVolume > 0f && !helpMenuMode) { musicLoop.Play(); }
            GameObject[] listOfObjects = GameObject.FindGameObjectsWithTag("ufo");
            foreach (GameObject gameObj in listOfObjects) { gameObj.SendMessage("CheckSounds", 2); }

            gamePausePanel.SetActive(false);
            buttonWhenLeavingPauseBugFix.Select();
            Time.timeScale = 1;
        }
    }

    public void ExitGame() {
        SceneManager.LoadScene("StartMenu");
        Time.timeScale = 1;
    }

    /* ------------------------------------------------------------------------------------------------------------------
     * Level transition code
     * ------------------------------------------------------------------------------------------------------------------ */
     
    // When a level starts, perform starting operations
    void StartNewLevel() {
        levelNo++;
        // Asteroid number depends on level number. Iterated in SpawnProp()
        asteroidCount = 0;
        for (int i = 0; i < levelNo + 1; i++) { SpawnProp("asteroid"); }
        // Player Respawn
        if (!player1dead) { playerShip1.SendMessage("RespawnShip"); }
        if (!player2dead) { playerShip2.SendMessage("RespawnShip"); }

        // Set a cap on how many UFOs or canisters can spawn
        if (levelNo == 1) { propCap = 2; }
        else if (levelNo < 3) { propCap = 3; }
        else if (levelNo < 9) { propCap = 4; }
        else { propCap = 5; }
        // Set when the first UFO and canister will spawn
        AlienAndPowerupLogic("initialAlienSetup");
        AlienAndPowerupLogic("initialPowerupSetup");
    }

    // At the end of a level activate level transition dialog, set the UFO to disappear, and if a player is still alive, respawn them
    public void EndLevelFanFare() {
        if (!player1dead || !player2dead) {
            GameObject[] listOfObjects = GameObject.FindGameObjectsWithTag("ufo");
            foreach (GameObject gameObj in listOfObjects) { gameObj.SendMessage("TeleportStart"); }
            Invoke("EndLevelFanFare2", 2.5f);
        }
    }
    private void EndLevelFanFare2() {
        gameLevelPanel.SetActive(true);
        // Player Shield Recovery
        if (!player1dead) { playerShip1.SendMessage("ShipIsRecovering"); }
        if (!player2dead) { playerShip2.SendMessage("ShipIsRecovering"); }
        StartCoroutine(FadeBlack("to"));
        Invoke("BringUpShop", 3f);
    }

    // Leaves Main scene and brings up the shop
    private void BringUpShop() {
        BetweenScenesScript.ResumingFromSave = true;
        Saving_SaveManager.SaveData(this, playerShip1, playerShip2);
        SceneManager.LoadScene("ShopMenu");
    }

    // Alien UFO that spawns during the end-level transition will check this, and despawn if level transition is happening
    public bool CheckIfEndOfLevel() {
        if (asteroidCount <= 0) { return true; }
        return false;
    }

    // If a ship has less than full shields, show the text say shields are being recharged
    public void ShowRechargeText() { gameLevelShieldRechargeText.SetActive(true); }

    // Show game over panel and pause the game when the game is over
    public void GameOver() {
        Cursor.visible = true;
        BetweenScenesScript.ResumingFromSave = false;
        Saving_SaveManager.EraseData();
        gameOverPanel.SetActive(true);
        buttonWhenGameOver.Select();

        musicLoop.Pause();
        GameObject[] listOfObjects = GameObject.FindGameObjectsWithTag("ufo");
        foreach (GameObject gameObj in listOfObjects) { gameObj.SendMessage("CheckSounds", 1); }

        Time.timeScale = 0;
    }

    // Reload the scene and restart playback if user decides to play again
    public void PlayAgain() {
        SceneManager.LoadScene("MainScene");
        Time.timeScale = 1;
    }
    
    /* ------------------------------------------------------------------------------------------------------------------
     * Misc Functions
     * ------------------------------------------------------------------------------------------------------------------ */

    public void SpawnProp(string type) {
        float originChoice = Random.Range(0f, 4f);
        Vector2 spawnPosition = new Vector2();
        if (originChoice < 1f) { // Spawn on the left
            spawnPosition = new Vector2(screenLeft - 1.5f, Random.Range(-9f, 9f));
        }
        else if (originChoice < 2f) { // Spawn on the right
            spawnPosition = new Vector2(screenRight + 1.5f, Random.Range(-9f, 9f));
        }
        else if (originChoice < 3f) { // Spawn on the top
            spawnPosition = new Vector2(Random.Range(-12f, 12f), screenTop + 1.5f);
        }
        else if (originChoice < 4f) { // Spawn on the bottom
            spawnPosition = new Vector2(Random.Range(-12f, 12f), screenBottom - 1.5f);
        }
        if (type == "ufo") {
            GameObject newAlien = Instantiate(alienShip);
            newAlien.transform.position = spawnPosition;
        }
        else if (type == "canister") {
            GameObject newCanister = Instantiate(canister);
            newCanister.transform.position = spawnPosition;
        }
        else if (type == "asteroid") {
            GameObject newAsteroid = Instantiate(largeAsteroid, spawnPosition, Quaternion.identity);
            asteroidCount += 1;
            if (instantkillAsteroids) {
                GameObject childAsteroid = newAsteroid.transform.GetChild(0).gameObject;
                childAsteroid.SendMessage("DebugMode");
            }
        }
    }

    private IEnumerator FadeBlack(string ToOrFrom) {
        Image tempFade = fadeBlack.GetComponent<Image>();
        Color origColor = tempFade.color;
        float speedOfFade = 0.4f;
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
            yield return new WaitForSeconds(2f);
            while (fadingAlpha < 1f) {
                fadingAlpha += speedOfFade * Time.deltaTime;
                tempFade.color = new Color(origColor.r, origColor.g, origColor.b, fadingAlpha);
                yield return null;
            }
        }
    }
}
