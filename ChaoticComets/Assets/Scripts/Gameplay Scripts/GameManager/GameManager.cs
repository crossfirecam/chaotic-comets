using UnityEngine;
using UnityEngine.UI;

/*
 * This class handles all in-game meta logic, such as level transitions, telling objects when and how to spawn, player management, etc.
 */

public partial class GameManager : MonoBehaviour
{
    [Header("Debug Settings (set false in public builds)")]
    public bool instantkillAsteroids = false;
    public bool cheatMode = false;

    [Header("Tutorial Mode")]
    public bool tutorialMode = true; // Not in tutorial by default

    [Header("General purpose variables")]
    public int asteroidCount;
    public int levelNo = 0;
    internal float screenTop = 8.5f, screenBottom = -7.5f, screenLeft = -15f, screenRight = 15f;
    private MusicManager musicManager;

    [Header("Inspector References")]
    public GameManagerHiddenVars Refs;

    private static GameManager _i;
    public static GameManager i { get { if (_i == null) _i = FindObjectOfType<GameManager>(); return _i; } }

    private void Awake()
    {
        if (cheatMode)
        {
            BetweenScenes.CheaterMode = true;
        }
    }
    void Start()
    {
        // If in cheater mode and tutorial mode isn't selected, enable the cheat panel in Pause Menu
        if (BetweenScenes.CheaterMode && !BetweenScenes.TutorialMode)
            UiManager.i.EnablePauseCheatPanel();

        // If in tutorial mode, activate TutorialManager & tutorial music
        // If in normal gameplay, set player ships to become active and start gameplay
        if (BetweenScenes.TutorialMode)
            StartTutorialMode();
        else
            StartNormalMode();

        PlayMusicIfEnabled();
        StartCoroutine(UsefulFunctions.CheckController());
    }

    private void StartTutorialMode()
    {
        StartCoroutine(Refs.playerShip1.GetComponent<PlayerInput>().DelayNewInputs());
        Refs.tutorialManager.SetActive(true);
        tutorialMode = true;
        playerLives = -1; // Player lives don't matter in tutorial mode.

        // TODO tutorial should set all UI values to 0
    }

    private void StartNormalMode()
    {
        if (BetweenScenes.PlayerCount == 2)
        {
            Refs.player2GUI.SetActive(true);
            Refs.playerShip2.gameObject.SetActive(true);
            player2dead = false;
        }
        UiManager.i.StartCoroutine(UiManager.i.FadeScreenBlack("from"));

        // Level -1 is used by the Testing script. In Level -1, nothing else spawns except ships.
        if (levelNo != -1)
        {
            CheckIfResumingFromSave();
            StartCoroutine(StartNewLevel());
        }
    }

    // Screen Wrapping
    public void CheckScreenWrap(Transform current, float xOldOffset = 0f, float yOldOffset = 0f, float xNewOffset = 0f, float yNewOffset = 0f)
    {
        Vector2 newPosition = current.position;
        if (current.position.y > screenTop - yOldOffset) { newPosition.y = screenBottom + yNewOffset; }
        if (current.position.y < screenBottom + yOldOffset) { newPosition.y = screenTop - yNewOffset; }
        if (current.position.x > screenRight - xOldOffset) { newPosition.x = screenLeft + xNewOffset; }
        if (current.position.x < screenLeft + xOldOffset) { newPosition.x = screenRight - xNewOffset; }
        current.position = newPosition;
    }
}

[System.Serializable]
public class GameManagerHiddenVars
{
    [Header("Tutorial References")]
    public AudioClip musicTutorial;
    public GameObject tutorialManager;
    public GameObject largeAsteroidSafeProp;

    [Header("UI References")]
    public GameObject gameLevelShieldRechargeText;
    public GameObject player2GUI;

    [Header("Prop References")]
    public Transform propParent;
    public PlayerMain playerShip1, playerShip2;
    public GameObject largeAsteroidProp, ufoFollowerProp, ufoPasserProp, canisterProp;

    [Header("Other References")]
    public GameObject musicManagerIfNotFoundInScene;
}
