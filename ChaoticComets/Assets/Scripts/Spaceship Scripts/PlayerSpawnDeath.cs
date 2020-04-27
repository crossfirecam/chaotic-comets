using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * This class concerns all actions of the Player to do with spawning, death, and related code.
 */

public class PlayerSpawnDeath : MonoBehaviour
{
    [SerializeField] PlayerMain p = default;

    internal void ShipIsDead()
    {
        if (p.playerPowerups.ifInsuranceActive)
        {
            p.playerUI.insurancePowerup.gameObject.SetActive(false); p.playerPowerups.ifInsuranceActive = false;
        }
        else
        {
            // If difficulty is easy, do not remove retro thruster
            if (BetweenScenesScript.Difficulty != 0)
            {
                p.playerUI.retroThrusterPowerup.gameObject.SetActive(false); p.playerPowerups.ifRetroThruster = false;
            }
            p.playerUI.rapidShotPowerup.gameObject.SetActive(false); p.playerPowerups.ifRapidShot = false;
            p.playerUI.tripleShotPowerup.gameObject.SetActive(false); p.playerPowerups.ifTripleShot = false;
            p.playerUI.farShotPowerup.gameObject.SetActive(false); p.playerPowerups.ifFarShot = false;
            p.playerWeapons.bulletDestroyTime = 0.8f; // Reset bullet destroy time to not be 'far shot'
        }
        p.shields = 0;
        p.lives--; p.playerUI.livesText.text = "Lives: " + p.lives;
        GameObject newExplosion = Instantiate(p.deathExplosion, transform.position, transform.rotation);
        Destroy(newExplosion, 2f);
        p.audioShipImpact.clip = p.deathSound;
        p.sprite.enabled = false;
        p.colliderEnabled = false;
        p.gM.SendMessage("PlayerLostLife", p.playerNumber);
        if (p.lives < 1)
        {
            p.gM.SendMessage("PlayerDied", p.playerNumber);
        }
        else { p.playerUI.prevshields = 80; Invoke("RespawnShip", 3f); }
    }

    void ShipIsRecovering()
    {
        if (p.shields < 80 && p.shields > 0)
        {
            p.gM.SendMessage("ShowRechargeText");
            StartCoroutine("RecoveringTimer");
        }
    }

    void RespawnShip()
    {
        if (p.lives > 0 && p.gM.asteroidCount != 0)
        {
            // If difficulty is Easy, equip Retro Thruster every respawn
            if (BetweenScenesScript.Difficulty == 0)
            {
                p.playerPowerups.ifRetroThruster = true; p.playerUI.retroThrusterPowerup.gameObject.SetActive(true);
            }
            p.sprite.enabled = true;
            p.colliderEnabled = false;
            p.rb.velocity = Vector2.zero;

            // If at least one player is dead, place the other in the center of the screen
            if (p.gM.player1dead || p.gM.player2dead)
            {
                transform.position = new Vector2(0f, 0f);
            }
            // If both players exist, place them apart
            else
            {
                if (p.playerNumber == 1) { transform.position = new Vector2(-3f, 0f); }
                else { transform.position = new Vector2(3f, 0f); }
            }
            // Alert GameManger that their temporary death is over for UFO tracking
            if (p.playerNumber == 1)
            {
                p.gM.player1TEMPDEAD = false;
            }
            else
            {
                p.gM.player2TEMPDEAD = false;
            }

            p.sprite.color = p.invulnColor;
            StartCoroutine("InvulnTimer");
        }
    }

    // When ship is killed, take 4 seconds total to recharge shields. Ship becomes hittable after those 4s.
    private IEnumerator InvulnTimer()
    {
        // Or, if ship is respawning at start of a level, set the previousShields level to current shield level instead
        if (p.playerUI.prevshields != 80) { p.playerUI.prevshields = p.shields; }
        p.shields = 0;
        p.playerUI.powerBar.sprite = p.playerUI.powerWhenCharging; // Set power bar to have no text
        for (int shieldsTick = 0; shieldsTick <= p.playerUI.prevshields; shieldsTick++)
        {
            p.shields = shieldsTick;
            yield return new WaitForSeconds(3f / p.playerUI.prevshields);
        }
        p.playerUI.powerBar.sprite = p.playerUI.powerWhenReady; // Set power bar to have text informing power can be used
        p.sprite.color = p.normalColor;
        p.colliderEnabled = true;
        p.playerUI.prevshields = 0;
    }

    // When level transition happens, take a moment to recharge shields by 20, or if above 60 heal up to 80.
    private IEnumerator RecoveringTimer()
    {
        // By default, recover to full shields, and determine how much shields need to be healed for UI timing
        float shieldToRecoverTo = 80;
        float shieldTimer = 80 - p.shields;
        // If shields are less than 60, determine where to recover to, and set shield timer to 20s for UI timing
        if (p.shields < 60)
        {
            shieldToRecoverTo = p.shields + 20;
            shieldTimer = 20;
        }
        for (float shieldsTick = p.shields; shieldsTick <= shieldToRecoverTo; shieldsTick++)
        {
            if (p.shields == 80) { break; } // If a canister is picked up during regen, break the loop
            p.shields = shieldsTick;
            yield return new WaitForSeconds(1f / shieldTimer);
        }
    }

    public void PretendShipDoesntExist()
    {
        p.sprite.enabled = false;
        p.colliderEnabled = false;
    }
}
