using Rewired;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUiSounds : MonoBehaviour
{
    [SerializeField] PlayerMain p = default;

    // UI Systems
    public Transform playerUi, gameUi;
    private Transform plrUiPowerup, plrUiBars;
    internal Image insurancePowerup, farShotPowerup, tripleShotPowerup, rapidShotPowerup, autoBrakePowerup;
    const int bonusInterval = 5000;
    internal Image shieldBar, abilityBar;
    private TextMeshProUGUI totalScoreText, currentCreditsText, livesText;
    internal float prevshields;

    // Sound Systems
    public AudioSource audioShipThrust, audioShipAutoBrake, audioShipSFX; // Thrust: passive thruster noise, SFX: powerup, extra life, impact noises
    public AudioClip audClipPlrSfxImpactSoft, audClipPlrSfxImpactHard, audClipPlrSfxDeath;

    private void Awake()
    {
        livesText = gameUi.Find("Lives").GetComponent<TextMeshProUGUI>();

        currentCreditsText = playerUi.Find("Credits").GetComponent<TextMeshProUGUI>();
        totalScoreText = playerUi.Find("TotalScore").GetComponent<TextMeshProUGUI>();

        plrUiPowerup = playerUi.Find("Powerups");
        insurancePowerup = plrUiPowerup.Find("PowerupInsurance").GetComponent<Image>();
        farShotPowerup = plrUiPowerup.Find("PowerupFarShot").GetComponent<Image>();
        tripleShotPowerup = plrUiPowerup.Find("PowerupTripleShot").GetComponent<Image>();
        rapidShotPowerup = plrUiPowerup.Find("PowerupRapidShot").GetComponent<Image>();
        autoBrakePowerup = plrUiPowerup.Find("PowerupAutoBrake").GetComponent<Image>();

        plrUiBars = playerUi.Find("Bars");
        shieldBar = plrUiBars.Find("ShieldBarActive").GetComponent<Image>();
        abilityBar = plrUiBars.Find("AbilityBarActive").GetComponent<Image>();
    }
    public void UpdateBars()
    {
        shieldBar.fillAmount = p.shields / 80;
        abilityBar.fillAmount = p.power / 80;
    }

    public void UpdatePointDisplays()
    {
        // If total credits are higher than bonus threshold, then grant a life
        if (p.totalCredits > p.bonus && !p.gM.tutorialMode)
        {
            p.bonus += bonusInterval;
            p.plrPowerups.GrantExtraLife();
        }

        currentCreditsText.text = p.credits + "¢";
        livesText.text = "Lives: " + p.gM.playerLives;
        totalScoreText.text = "T: " + p.totalCredits;
        if (p.gM.tutorialMode)
        {
            livesText.text = "Lives: Inf.";
            p.gM.playerLives = 2; // Ensure player never game-overs in tutorial
        }
        if (p.gM.playerLives <= -1) // TODO Ensure this edge case works. Player dies with 0 lives remaining should not display -1.
        {
            livesText.text = "Lives: 0";
        }
    }

    public GameObject[] ReturnPlayerSounds()
    {
        GameObject[] playerSfx = { audioShipThrust.gameObject, audioShipSFX.gameObject, audioShipAutoBrake.gameObject,
            p.plrAbility.teleportIn, p.plrAbility.teleportOut };
        return playerSfx;
    }
}
