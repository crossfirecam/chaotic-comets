using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUiSounds : MonoBehaviour
{
    [SerializeField] PlayerMain p = default;

    // UI Systems
    public Image insurancePowerup, farShotPowerup, tripleShotPowerup, rapidShotPowerup, retroThrusterPowerup;
    const int bonusInterval = 10000;
    public Image shieldBar, powerBar;
    public Sprite powerWhenCharging, powerWhenReady;
    public Text scoreText, livesText;
    internal float prevshields;

    // Sound Systems
    public AudioSource audioShipThrust, audioShipSFX; // Thrust: passive thruster noise, SFX: powerup, extra life, impact noises
    public AudioClip audClipPlrSfxImpactSoft, audClipPlrSfxImpactHard, audClipPlrSfxDeath;
    private bool audioThrusterPlaying = false, audioTeleportInPlaying = false, audioTeleportOutPlaying = false;

    public void UpdateBars()
    {
        shieldBar.fillAmount = p.shields / 80;
        powerBar.fillAmount = p.power / 80;
    }

    public void UpdatePointDisplays()
    {
        // If credits are higher than bonus threshold, and bonus is not set to 0 (credits cheated in), then grant a life
        if (p.credits > p.bonus && p.bonus != 0)
        {
            p.bonus += bonusInterval;
            p.plrPowerups.GrantExtraLife();
        }

        scoreText.text = "Credits:\n" + p.credits;
        livesText.text = "Lives: " + p.lives;
    }

    public void CheckSounds(int intent)
    {
        if (intent == 1)
        {
            if (audioShipThrust.isPlaying)
            {
                audioThrusterPlaying = true;
                audioShipThrust.Pause();
            }
            if (p.plrAbility.teleportIn.GetComponent<AudioSource>().isPlaying)
            {
                audioTeleportInPlaying = true;
                p.plrAbility.teleportIn.GetComponent<AudioSource>().Pause();
            }
            if (p.plrAbility.teleportOut.GetComponent<AudioSource>().isPlaying)
            {
                audioTeleportOutPlaying = true;
                p.plrAbility.teleportOut.GetComponent<AudioSource>().Pause();
            }
        }
        else if (intent == 2)
        {
            if (audioThrusterPlaying)
            {
                audioShipThrust.UnPause();
            }
            if (audioTeleportInPlaying)
            {
                p.plrAbility.teleportIn.GetComponent<AudioSource>().UnPause();
            }
            if (audioTeleportOutPlaying)
            {
                p.plrAbility.teleportOut.GetComponent<AudioSource>().UnPause();
            }
        }
    }
}
