using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/*
 * This class handles all in-game meta logic, such as level transitions, telling objects when and how to spawn, player management, etc.
 */

public partial class GameManager : MonoBehaviour
{

    [Header("Tutorial Mode")]
    public bool tutorialMode = true; // Not in tutorial by default

    [Header("General purpose variables")]
    public int asteroidCount;
    public int levelNo = 0;
    [HideInInspector] public float screenTop = 8f, screenBottom = 8f, screenLeft = 11f, screenRight = 11f;
    private MusicManager musicManager;

    [Header("Other Variables")]
    private float fadingAlpha = 0f;

    [Header("Inspector References")]
    public GameManagerHiddenVars Refs;

    void Start()
    {
        // If in cheater mode, enable the cheat panel in Pause
        if (BetweenScenes.CheaterMode)
        {
            Refs.gamePausePanel.transform.Find("PauseDialog").Find("CheatPanel").gameObject.SetActive(true);
        }

        // If in tutorial mode, activate TutorialManager & tutorial music
        if (BetweenScenes.TutorialMode || tutorialMode)
        {
            StartCoroutine(Refs.playerShip1.GetComponent<PlayerInput>().DelayNewInputs());
            Refs.tutorialManager.SetActive(true);
            Refs.titleP1.text = "Tutorial";
            tutorialMode = true;
        }
        // If in normal gameplay, set player ships to become active and start gameplay
        else
        {
            if (BetweenScenes.PlayerCount == 2) {
                Refs.player2GUI.SetActive(true);
                Refs.playerShip2.gameObject.SetActive(true);
                player2dead = false;
            }
            CheckIfResumingFromSave();
            StartCoroutine(FadeBlack("from"));
            StartCoroutine(StartNewLevel());
        }
        PlayMusicIfEnabled();
        StartCoroutine(UsefulFunctions.CheckController());
    }

    private void Update()
    {
        print($"Player 1 Speed: {Refs.playerShip1.rbPlayer.velocity.magnitude} Player 2 Speed: {Refs.playerShip2.rbPlayer.velocity.magnitude}");
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
    public GameObject tutorialManager, tutorialChoicePanel;
    public Text titleP1;
    public Button buttonWhenTutorialChoice;
    public GameObject largeAsteroidSafeProp;

    [Header("UI References")]
    public GameObject gameLevelShieldRechargeText;
    public GameObject fadeBlack, player2GUI;
    public GameObject gameOverPanel, gameOverPanelAlt, gamePausePanel, gameLevelPanel;
    public Button buttonWhenPaused, buttonWhenGameOver, buttonWhenGameOverAlt, buttonWhenLeavingPauseBugFix;

    [Header("Prop References")]
    public PlayerMain playerShip1;
    public PlayerMain playerShip2;
    public GameObject largeAsteroidProp, ufoFollowerProp, ufoPasserProp, canisterProp;

    [Header("Other References")]
    public GameObject musicManagerIfNotFoundInScene;
}
