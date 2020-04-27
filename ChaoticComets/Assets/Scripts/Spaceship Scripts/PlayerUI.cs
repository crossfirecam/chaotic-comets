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
        scoreText.text = "Credits:\n" + p.credits;
        if (p.credits > p.bonus)
        {
            p.bonus += bonusInterval;
            p.lives++; livesText.text = "Lives: " + p.lives;
            p.audioShipImpact.clip = p.lifeGained;
            p.audioShipImpact.Play();
        }
    }
}
