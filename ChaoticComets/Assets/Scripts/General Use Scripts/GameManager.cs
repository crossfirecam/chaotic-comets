using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/*
 * This class handles all in-game meta logic, such as level transitions, telling objects when and how to spawn, player management, etc.
 */

public partial class GameManager : MonoBehaviour {

    // General purpose variables
    public int asteroidCount;
    public int levelNo = 0;

    // UI related objects
    public GameObject fadeBlack, player2GUI;
    public GameObject gameOverPanel, gamePausePanel, gameLevelPanel;
    public GameObject gameLevelShieldRechargeText;
    public Text swapP1Text, swapP2Text;
    public Button buttonWhenPaused, buttonWhenGameOver, buttonWhenLeavingPauseBugFix;

    // Other variables
    [HideInInspector] public float screenTop = 8.5f, screenBottom = 8.5f, screenLeft = 11f, screenRight = 11f;
    public bool helpMenuMode = false; // Not in control help screen by default
    public AudioSource musicLoop;
    private float fadingAlpha = 0f;

    void Start() {
        Cursor.visible = false;
        // If in normal gameplay, set player ships to become active and start gameplay
        if (!helpMenuMode) {
            if (BetweenScenesScript.MusicVolume > 0f) {
                musicLoop.Play();
            }
            if (BetweenScenesScript.PlayerCount == 2) {
                player2GUI.SetActive(true);
                playerShip2.gameObject.SetActive(true);
                player2dead = false;
            }
            if (BetweenScenesScript.ResumingFromSave) { // If resuming from save file, read from save file first
                Saving_PlayerManager data = Saving_SaveManager.LoadData();
                levelNo = data.level;
                // Tell ships to 'play dead' (disable model & colliders) if previous shop says they're dead
                if (BetweenScenesScript.player1TempLives == 0)
                {
                    playerShip1.plrSpawnDeath.PretendShipDoesntExist();
                }
                if (BetweenScenesScript.player2TempLives == 0 && BetweenScenesScript.PlayerCount == 2)
                {
                    playerShip2.plrSpawnDeath.PretendShipDoesntExist();
                
                }
            }
            StartCoroutine(FadeBlack("from"));
            Invoke("StartNewLevel", 0f);
        }
    }

    void Update() {
        // Rotate the UFO for help screen purposes
        if (helpMenuMode) {
            alienShipProp.transform.rotation = Quaternion.Euler(-50, 0, 0);
        }

        // Each frame, check if pause menu is open, and what button is highlighted.
        // If the mouse is used to click auto highlight away, then drag a highlight back onto a certain button.
        if (gamePausePanel.activeInHierarchy || gameOverPanel.activeInHierarchy) {
            if (EventSystem.current.currentSelectedGameObject == null || EventSystem.current.currentSelectedGameObject.Equals(null))
            {
                // If on pause panel, then select the resume button. If on game over panel, select play again button.
                if (gamePausePanel.activeInHierarchy) { buttonWhenPaused.Select(); }
                else { buttonWhenGameOver.Select(); }
            }
        }

        // Check if pause button is pressed, and resume/pause game.
        if (Input.GetButtonDown("Pause") && !gameOverPanel.activeInHierarchy) {
            if (gamePausePanel.activeInHierarchy) {
                PauseGame(1);
            }
            else {
                PauseGame(0);
            }
        }
    }

    // Screen Wrapping
    public void CheckScreenWrap(Transform current, float offset)
    {
        Vector2 newPosition = current.position;
        if (current.position.y > screenTop) { newPosition.y = screenBottom + offset; }
        if (current.position.y < screenBottom) { newPosition.y = screenTop - offset; }
        if (current.position.x > screenRight) { newPosition.x = screenLeft + offset; }
        if (current.position.x < screenLeft) { newPosition.x = screenRight - offset; }
        current.position = newPosition;
    }
}
