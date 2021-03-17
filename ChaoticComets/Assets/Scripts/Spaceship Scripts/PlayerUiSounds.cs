using Rewired;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUiSounds : MonoBehaviour
{
    [SerializeField] PlayerMain p = default;

    // UI Systems
    public Transform playerUi, respawnOverlay;
    private Transform plrUiBars;
    const int bonusInterval = 5000;
    internal Image shieldBar, abilityBar;
    private TextMeshProUGUI totalScoreText, currentCreditsText;
    internal float prevshields;

    // Sound Systems
    public AudioSource audioShipThrust, audioShipAutoBrake, audioShipSFX; // Thrust: passive thruster noise, SFX: powerup, extra life, impact noises
    public AudioClip audClipPlrSfxImpactSoft, audClipPlrSfxImpactHard, audClipPlrSfxDeath;

    private void Awake()
    {
        currentCreditsText = playerUi.Find("Credits").GetComponent<TextMeshProUGUI>();
        totalScoreText = playerUi.Find("TotalScore").GetComponent<TextMeshProUGUI>();

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

        UiManager.i.SetPlayerCredits(p.playerNumber, p.credits, p.totalCredits);
        UiManager.i.SetShipsText(p.gM.playerLives);
    }

    public void ShowRespawnOverlay(bool status)
    {
        respawnOverlay.gameObject.SetActive(status);
    }

    public void UpdateRespawnStatus()
    {
        if (p.gM.playerLives == 0)
        {
            respawnOverlay.Find("RespawnOverlayText").GetComponent<TextMeshProUGUI>().text = "No lives left.";
        }
        else
        {
            respawnOverlay.Find("RespawnOverlayText").GetComponent<TextMeshProUGUI>().text = "Press 'Fire' to respawn!";
        }
    }

    public GameObject[] ReturnPlayerSounds()
    {
        GameObject[] playerSfx = { audioShipThrust.gameObject, audioShipSFX.gameObject, audioShipAutoBrake.gameObject,
            p.plrAbility.teleportIn, p.plrAbility.teleportOut };
        return playerSfx;
    }
}
