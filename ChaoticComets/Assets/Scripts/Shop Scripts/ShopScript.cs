using Rewired;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public partial class ShopScript : MonoBehaviour {

    [Header("Shop Variables")]
    private readonly int baseUpgradePrice = 500, priceIncreasePerLevel = 750, upgradeCap = 15;
    public bool[] plrIsReady = { false, false };
    private float fadingAlpha = 0f;

    [Header("References")]
    private Saving_PlayerManager data;
    private MusicManager musicManager;
    private Player player1, player2;
    public ShopManagerHiddenVars ShopRefs;
    public TextMeshProUGUI saveDisclaimer;


    /* ------------------------------------------------------------------------------------------------------------------
     * Awake & Start - Startup functions
     * ------------------------------------------------------------------------------------------------------------------ */
    private void Awake()
    {
        player1 = ReInput.players.GetPlayer(0);
        player2 = ReInput.players.GetPlayer(BetweenScenes.PlayerCount == 2 ? 1 : 0);

        ShopRefs.listOfPlrPowerups[0] = ShopRefs.listOfPlr1Powerups;
        ShopRefs.listOfPlrPowerups[1] = ShopRefs.listOfPlr2Powerups;

        StartCoroutine(UsefulFunctions.CheckController());
    }

    void Start()
    {
        // Uncomment this if testing a two-player save file
        //BetweenScenes.PlayerCount = 2; Debug.LogWarning("PlayerCount set manually in ShopScript");

        // Load data. If failed, then send user to main menu with error message
        data = Saving_SaveManager.LoadData();
        if (data == null)
        {
            ShopRefs.saveFailedPanel.SetActive(true);
            DisablePlrEventsEnablePauseEvents();
        }

        // Prepare UI for each player
        for (int i = 0; i < BetweenScenes.PlayerCount; i++)
        {
            PrepareUI(i);
            BetweenScenes.PlayerShopUpgrades[i] = data.playerList[i].upgrades;
        }
        // In single player mode, say that P2 is ready and move P1's buttons to the center
        if (data.playerCount == 1) {
            plrIsReady[1] = true;
            Player1OnlyGUI();
        }

        // If save is in cheater mode, change some details
        if (BetweenScenes.CheaterMode)
        {
            ShopRefs.gamePausePanel.transform.Find("PauseDialog").Find("AutoSaveText").GetComponent<Text>().text = "In Cheat Mode, progress is not saved.\nCheat buttons aren't available in the Depot.";
            ShopRefs.gamePausePanel.transform.Find("PauseDialog").Find("AutoSaveText").GetComponent<Text>().color = Color.red;
            saveDisclaimer.text = "<u>Progress not saved</u>\nGame is in Cheat Mode.";
        }

        // DelayedReady() prevents player that is holding 'Shoot' button from instantly readying up.
        StartCoroutine(DelayedReady());

        PlayMusicIfEnabled();
        InitialiseButtonText();
        StartCoroutine(FadeBlack("from"));
    }

    /* ------------------------------------------------------------------------------------------------------------------
     * DelayedReady - When shop opens, find all player event systems and select an offscreen button for 0.2s.
     * Then select ready button. This is to prevent players that are holding 'Fire' from instantly being flagged as ready.
     * ------------------------------------------------------------------------------------------------------------------ */
    private IEnumerator DelayedReady() {
        for (int i = 0; i < BetweenScenes.PlayerCount; i++)
        {
            ShopRefs.plrEventSystems[i].gameObject.SetActive(true);
            ShopRefs.plrEventSystems[i].SetSelectedGameObject(ShopRefs.buttonWhenLeavingPauseBugFix.gameObject);
        }
        yield return new WaitForSeconds(0.2f);
        for (int i = 0; i < BetweenScenes.PlayerCount; i++)
        {
            ShopRefs.plrEventSystems[i].SetSelectedGameObject(ShopRefs.plrReadyBtns[i].gameObject);
        }
    }

    /* ------------------------------------------------------------------------------------------------------------------
     * Update - Only used in shop to check for pausing
     * ------------------------------------------------------------------------------------------------------------------ */
    void Update() {
        if (player1.GetButtonDown("Pause") || player2.GetButtonDown("Pause")) {
            if (ShopRefs.gamePausePanel.activeInHierarchy) {
                PauseGame(1);
            }
            else {
                PauseGame(0);
            }
        }
    }

    /* ------------------------------------------------------------------------------------------------------------------
     * Scene Change Functions
     * ------------------------------------------------------------------------------------------------------------------ */
    public void GoBackToGame() {
        ShopRefs.readyPromptText.text = $"Prepare for Level {data.level + 1}!";
        StartCoroutine(FadeBlack("to"));
        Invoke("LoadMainGame", 1f);
    }
    private void LoadMainGame() {
        SceneManager.LoadScene("MainScene");
    }
    public void BackToMainMenu() {
        Time.timeScale = 1;
        if (BetweenScenes.CheaterMode)
        {
            Saving_SaveManager.EraseData();
        }
        SceneManager.LoadScene("StartMenu");
    }
}

/* ------------------------------------------------------------------------------------------------------------------
 * ShopManagerHiddenVars - To keep Unity inspector less cluttered, this is a class containing drag-in references
 * ------------------------------------------------------------------------------------------------------------------ */
[System.Serializable]
public class ShopManagerHiddenVars
{
    [Header("Pause UI References")]
    public GameObject gamePausePanel;
    public Button buttonWhenPaused, buttonWhenLeavingPauseBugFix;

    [Header("Player UI References")]
    public GameObject[] listOfPlr1Powerups;
    public GameObject[] listOfPlr2Powerups;
    public GameObject[][] listOfPlrPowerups = new GameObject[2][];
    public Image[] listOfPlrShieldBars, listOfPlrAbilityBars;
    public TextMeshProUGUI[] listOfPlrScoreText, listOfPlrTotalScoreText;

    [Header("Shop UI References")]
    public TextMeshProUGUI readyPromptText;
    public Button[] plrAboveReadyBtns;
    public Button[] plrReadyBtns;

    [Header("Event System References")]
    public EventSystem pauseEventSystem;
    public EventSystemShop[] plrEventSystems;

    [Header("Other References")]
    public GameObject musicManagerIfNotFoundInScene;
    public GameObject fadeBlack, player2GUI, saveFailedPanel, mouseWarningPanel;
}
