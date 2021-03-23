using UnityEngine;
using TMPro;
using UnityEngine.UI;

public partial class UiManager : MonoBehaviour
{
    /* ------------------------------------------------------------------------------------------------------------------
     * Game State UI - Wave, Ships, Bonus counters in lower middle of screen.
     * ------------------------------------------------------------------------------------------------------------------ */
    [SerializeField] private AudioSource gameUiBonusAudSrc;
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

    private bool firstBonusSound = true;
    public void SetBonusText(int bonus)
    {
        // Change sound effect pitch. Alternate between two of them.
        if ((bonus / 10) % 2 == 0)
            gameUiBonusAudSrc.pitch = 0.4f;
        else
            gameUiBonusAudSrc.pitch = 0.6f;

        // Don't play sound effect when text first appears
        if (!firstBonusSound)
            gameUiBonusAudSrc.Play();
        else
            firstBonusSound = false;

        gameUiBonus.text = "Time Bonus: " + bonus;

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


    /* ------------------------------------------------------------------------------------------------------------------
     * Player UI - Show 2nd Player UI
     * ------------------------------------------------------------------------------------------------------------------ */
    [SerializeField] private GameObject player2GUI;

    public void ShowP2UI()
    {
        player2GUI.SetActive(true);
    }
}
