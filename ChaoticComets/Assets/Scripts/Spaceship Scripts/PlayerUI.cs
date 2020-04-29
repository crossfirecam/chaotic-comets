using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] PlayerMain p = default;

    public Image insurancePowerup, farShotPowerup, tripleShotPowerup, rapidShotPowerup, retroThrusterPowerup;
    const int bonusInterval = 10000;
    public Image shieldBar, powerBar;
    public Sprite powerWhenCharging, powerWhenReady;
    public Text scoreText, livesText;
    internal float prevshields;

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
            p.playerPowerups.GrantExtraLife();
        }

        scoreText.text = "Credits:\n" + p.credits;
        livesText.text = "Lives: " + p.lives;
    }
}
