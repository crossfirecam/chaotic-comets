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
    public bool p1IsReady = false, p2IsReady = false;
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
    }

    void Start() {
        // Nullexception is possible here, but only if shop is loaded without a save file. In typical gameplay it isn't possible
        data = Saving_SaveManager.LoadData();

        BetweenScenes.UpgradesP1 = data.playerList[0].upgrades;
        PrepareUI(1);

        if (data.playerCount == 1) {
            p2IsReady = true;
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
        StartCoroutine(UsefulFunctions.CheckController());
    }

    private IEnumerator DelayedReady() {
        ShopRefs.p1Events.SetSelectedGameObject(ShopRefs.buttonWhenLeavingPauseBugFix.gameObject);
        if (BetweenScenes.PlayerCount == 2) {
            ShopRefs.p2Events.SetSelectedGameObject(ShopRefs.buttonWhenLeavingPauseBugFix.gameObject);
        }
        yield return new WaitForSeconds(0.2f);
        ShopRefs.p1Events.SetSelectedGameObject(ShopRefs.p1ReadyButton.gameObject);
        if (BetweenScenes.PlayerCount == 2) {
            ShopRefs.p2Events.SetSelectedGameObject(ShopRefs.p2ReadyButton.gameObject);
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
        if (ShopRefs.p1Events.gameObject.activeInHierarchy) {
            if (ShopRefs.p1Events.currentSelectedGameObject == null || ShopRefs.p1Events.currentSelectedGameObject.Equals(null)) {
                ShopRefs.p1Events.SetSelectedGameObject(ShopRefs.p1ReadyButton.gameObject);
            }
        }
        if (BetweenScenes.PlayerCount == 2 && ShopRefs.p2Events.gameObject.activeInHierarchy) {
            if (ShopRefs.p2Events.currentSelectedGameObject == null || ShopRefs.p2Events.currentSelectedGameObject.Equals(null)) {
                ShopRefs.p2Events.SetSelectedGameObject(ShopRefs.p2ReadyButton.gameObject);
            }
        }
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
    public GameObject[] listOfP1Powerups;
    public GameObject[] listOfP2Powerups;
    public Image p1ShieldBar, p1PowerBar, p2ShieldBar, p2PowerBar;
    public Text p1ScoreText, p1LivesText, p1TotalScoreText, p2ScoreText, p2LivesText, p2TotalScoreText;

    [Header("Shop UI References")]
    public TextMeshProUGUI readyPromptText;
    public Button p1UpgBtnAboveReady, p2UpgBtnAboveReady;
    public Button p1ReadyButton, p2ReadyButton;

    [Header("Event System References")]
    public EventSystem pauseEventSystem;
    public EventSystemShop p1Events, p2Events;

    [Header("Other References")]
    public GameObject fadeBlack, player2GUI;
    public GameObject musicManagerIfNotFoundInScene;
}
