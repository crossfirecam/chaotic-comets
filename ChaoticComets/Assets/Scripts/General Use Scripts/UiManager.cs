using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    private static UiManager _i;
    public static UiManager i { get { if (_i == null) _i = FindObjectOfType<UiManager>(); return _i; } }

    private void Awake()
    {
        plrUiPowerups = new GameObject[][] { plr1UiPowerups, plr2UiPowerups};

        // Find text objects that are children of player's Respawn Overlays
        plrRespawnOverlayTexts = new TextMeshProUGUI[]
        {
            plrRespawnOverlayObjects[0].GetComponentInChildren<TextMeshProUGUI>(),
            plrRespawnOverlayObjects[1].GetComponentInChildren<TextMeshProUGUI>()
        };
    }

    /* ------------------------------------------------------------------------------------------------------------------
     * Game State UI - Wave, Ships, Bonus counters in lower middle of screen.
     * ------------------------------------------------------------------------------------------------------------------ */
    [SerializeField] private TextMeshProUGUI gameUiWave, gameUiShips, gameUiBonus;

    public void SetWaveText(int wave)
    {
        gameUiWave.text = "Wave: " + wave;
        if (wave == -1) // Tutorial Mode
            gameUiWave.text = "(Training)";
    }

    public void SetShipsText(int ships)
    {
        gameUiShips.text = "Ships: " + ships;
        if (ships <= -1) // Tutorial Mode
            gameUiShips.text = "Ships: Inf.";
    }
    public void SetBonusText(int bonus)
    {
        gameUiBonus.text = "Bonus: " + bonus;
    }


    /* ------------------------------------------------------------------------------------------------------------------
     * Player UI - Current Credits, Total Credits
     * ------------------------------------------------------------------------------------------------------------------ */
    [SerializeField] private TextMeshProUGUI[] plrUiCurrentCredits, plrUiTotalCredits;
    public void SetPlayerCredits(int playerNum, int current, int total)
    {
        plrUiCurrentCredits[playerNum].text = current + "¢";
        plrUiTotalCredits[playerNum].text = "T: " + total;
    }


    /* ------------------------------------------------------------------------------------------------------------------
     * Player UI - Powerup Icons
     * ------------------------------------------------------------------------------------------------------------------ */
    [SerializeField] private GameObject[] plr1UiPowerups, plr2UiPowerups;
    [SerializeField] private GameObject[][] plrUiPowerups = new GameObject[2][];

    public void SetPlayerPowerup(int playerNum, int powerupId, bool activeState)
    {
        // Set a [Certain Player's] [Chosen Powerup Icon] to (This State)
        plrUiPowerups[playerNum][powerupId].SetActive(activeState);
    }


    /* ------------------------------------------------------------------------------------------------------------------
     * Player UI - Shield & Ability Bars
     * ------------------------------------------------------------------------------------------------------------------ */
    [SerializeField] private Image[] plrShieldBars, plrAbilityBars;

    public void SetPlayerStatusBars(int playerNum, float shieldAmount, float abilityAmount)
    {
        plrShieldBars[playerNum].fillAmount = shieldAmount / 80;
        plrAbilityBars[playerNum].fillAmount = abilityAmount / 80;
    }


    /* ------------------------------------------------------------------------------------------------------------------
     * Player UI - Respawn Notifications
     * ------------------------------------------------------------------------------------------------------------------ */
    [SerializeField] private GameObject[] plrRespawnOverlayObjects;
    private TextMeshProUGUI[] plrRespawnOverlayTexts;
    public void ShowPlayerRespawnOverlay(int playerNum, bool status)
    {
        plrRespawnOverlayObjects[playerNum].SetActive(status);
    }

    public void SetPlayerRespawnStatus(int playerNum, int playerLives)
    {
        if (playerLives == 0)
            plrRespawnOverlayTexts[playerNum].text = "No lives left.";
        else
            plrRespawnOverlayTexts[playerNum].text = "Press 'Fire' to respawn!";

        Debug.Log($"Plr {playerNum + 1} {(playerLives == 0 ? "cannot" : "can")} respawn");
    }
}
