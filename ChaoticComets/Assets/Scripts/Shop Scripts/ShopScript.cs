using Rewired;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public partial class ShopScript : MonoBehaviour {

    [Header("Shop Variables")]
    private readonly int baseUpgradePrice = 1000, priceIncreasePerLevel = 1000, upgradeCap = 15;
    public bool[] plrIsReady = { false, false };
    private float fadingAlpha = 0f;

    [Header("References")]
    private Saving_PlayerManager data;
    private MusicManager musicManager;
    private Player player1, player2;
    public ShopManagerHiddenVars ShopRefs;
    public TextMeshProUGUI saveDisclaimer;


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
        //BetweenScenes.PlayerCount = 2;

        data = Saving_SaveManager.LoadData();
        if (data == null)
        {
            ShopRefs.saveFailedPanel.SetActive(true);
            DisablePlrEventsEnablePauseEvents();
        }

        BetweenScenes.UpgradesP1 = data.playerList[0].upgrades;
        PrepareUI(1);

        if (data.playerCount == 1) {
            plrIsReady[1] = true;
            Player1OnlyGUI();
        }
        else if (data.playerCount == 2)
        {
            BetweenScenes.UpgradesP2 = data.playerList[1].upgrades;
            PrepareUI(2);
        }

        if (BetweenScenes.CheaterMode)
        {
            ShopRefs.gamePausePanel.transform.Find("PauseDialog").Find("AutoSaveText").GetComponent<Text>().text = "In Cheat Mode, progress is not saved.\nCheat buttons aren't available in the Depot.";
            ShopRefs.gamePausePanel.transform.Find("PauseDialog").Find("AutoSaveText").GetComponent<Text>().color = Color.red;
            saveDisclaimer.text = "<u>Progress not saved</u>\nGame is in Cheat Mode.";
        }

        // DelayedReady() prevents player that is holding 'Shoot' button from instantly readying up.
        StartCoroutine(DelayedReady());

        PlayMusicIfEnabled();
        UpdateButtonText();
        StartCoroutine(FadeBlack("from"));
    }

    private IEnumerator DelayedReady() {
        ShopRefs.plrEventSystems[0].SetSelectedGameObject(ShopRefs.buttonWhenLeavingPauseBugFix.gameObject);
        if (BetweenScenes.PlayerCount == 2) {
            ShopRefs.plrEventSystems[1].SetSelectedGameObject(ShopRefs.buttonWhenLeavingPauseBugFix.gameObject);
        }
        yield return new WaitForSeconds(0.2f);
        ShopRefs.plrEventSystems[0].SetSelectedGameObject(ShopRefs.plrReadyBtns[0].gameObject);
        if (BetweenScenes.PlayerCount == 2) {
            ShopRefs.plrEventSystems[1].SetSelectedGameObject(ShopRefs.plrReadyBtns[1].gameObject);
        }
    }

    void Update() {
        if (player1.GetButtonDown("Pause") || player2.GetButtonDown("Pause")) {
            if (ShopRefs.gamePausePanel.activeInHierarchy) {
                PauseGame(1);
            }
            else {
                PauseGame(0);
            }
        }
        // Each frame, check what button is highlighted. Pull a button back into focus if mouse clicks away from a button.
        // If the mouse is used to click auto highlight away, then drag a highlight back onto a certain button.
        if (ShopRefs.pauseEventSystem.gameObject.activeInHierarchy && ShopRefs.gamePausePanel.activeInHierarchy) {
            if (ShopRefs.pauseEventSystem.currentSelectedGameObject == null || ShopRefs.pauseEventSystem.currentSelectedGameObject.Equals(null)) {
                ShopRefs.pauseEventSystem.SetSelectedGameObject(ShopRefs.buttonWhenPaused.gameObject);
            }
        }
    }

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
    public Text[] listOfPlrScoreText, listOfPlrLivesText, listOfPlrTotalScoreText;

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
