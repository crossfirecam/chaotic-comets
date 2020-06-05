using Rewired;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUiSounds : MonoBehaviour
{
    [SerializeField] PlayerMain p = default;

    // UI Systems
    public Image insurancePowerup, farShotPowerup, tripleShotPowerup, rapidShotPowerup, autoBrakePowerup;
    const int bonusInterval = 10000;
    public Image shieldBar, powerBar;
    public Sprite powerWhenCharging, powerWhenReady;
    public Text scoreText, livesText;
    internal float prevshields;

    // Sound Systems
    public AudioSource audioShipThrust, audioShipAutoBrake, audioShipSFX; // Thrust: passive thruster noise, SFX: powerup, extra life, impact noises
    public AudioClip audClipPlrSfxImpactSoft, audClipPlrSfxImpactHard, audClipPlrSfxDeath;

    public void UpdateBars()
    {
        shieldBar.fillAmount = p.shields / 80;
        powerBar.fillAmount = p.power / 80;
    }

    public void UpdatePointDisplays()
    {
        // If credits are higher than bonus threshold, then grant a life
        if (p.credits > p.bonus)
        {
            p.bonus += bonusInterval;
            p.plrPowerups.GrantExtraLife();
        }

        scoreText.text = "Credits:\n" + p.credits;
        livesText.text = "Lives: " + p.lives;
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
