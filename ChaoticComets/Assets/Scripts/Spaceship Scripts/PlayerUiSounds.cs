using Rewired;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUiSounds : MonoBehaviour
{
    [SerializeField] PlayerMain p = default;

    // UI Systems
    public Transform playerUi;
    private Transform plrUiPowerup, plrUiBars;
    internal Image insurancePowerup, farShotPowerup, tripleShotPowerup, rapidShotPowerup, autoBrakePowerup;
    const int bonusInterval = 5000;
    internal Image shieldBar, abilityBar;
    public Sprite abilityWhenCharging, abilityWhenReady;
    private Text titleText, totalScoreText, scoreText, livesText;
    internal float prevshields;

    // Sound Systems
    public AudioSource audioShipThrust, audioShipAutoBrake, audioShipSFX; // Thrust: passive thruster noise, SFX: powerup, extra life, impact noises
    public AudioClip audClipPlrSfxImpactSoft, audClipPlrSfxImpactHard, audClipPlrSfxDeath;

    private void Awake()
    {
        titleText = playerUi.Find("Title").GetComponent<Text>();
        titleText.text = "Player " + p.playerNumber;
        totalScoreText = playerUi.Find("TotalScore").GetComponent<Text>();
        scoreText = playerUi.Find("Credits").GetComponent<Text>();
        livesText = playerUi.Find("Lives").GetComponent<Text>();

        plrUiPowerup = playerUi.Find("Powerups");
        insurancePowerup = plrUiPowerup.Find("PowerupInsurance").GetComponent<Image>();
        farShotPowerup = plrUiPowerup.Find("PowerupFarShot").GetComponent<Image>();
        tripleShotPowerup = plrUiPowerup.Find("PowerupTripleShot").GetComponent<Image>();
        rapidShotPowerup = plrUiPowerup.Find("PowerupRapidShot").GetComponent<Image>();
        autoBrakePowerup = plrUiPowerup.Find("PowerupAutoBrake").GetComponent<Image>();

        plrUiBars = playerUi.Find("Bars");
        shieldBar = plrUiBars.Find("ShieldBarPartial").GetComponent<Image>();
        abilityBar = plrUiBars.Find("AbilityBarPartial").GetComponent<Image>();
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

        scoreText.text = p.credits + "c";
        livesText.text = "Lives: " + p.lives;
        totalScoreText.text = "Total Score:\n" + p.totalCredits;
        if (p.gM.tutorialMode)
        {
            livesText.text = "Lives: ∞";
            p.lives = 2; // Ensure player never game-overs in tutorial
        }
    }

    public GameObject[] ReturnPlayerSounds()
    {
        GameObject[] playerSfx = { audioShipThrust.gameObject, audioShipSFX.gameObject, audioShipAutoBrake.gameObject,
            p.plrAbility.teleportIn, p.plrAbility.teleportOut };
        return playerSfx;
    }
}
