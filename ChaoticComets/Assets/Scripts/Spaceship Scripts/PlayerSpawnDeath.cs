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
        if (p.plrPowerups.ifInsuranceActive)
        {
            p.plrPowerups.RemovePowerup(PlayerPowerups.Powerups.Insurance);
            p.plrUiSound.insurancePowerup.gameObject.SetActive(false); p.plrPowerups.ifInsuranceActive = false;
        }
        else
        {
            // If difficulty is easy, do not remove Auto-Brake
            if (BetweenScenesScript.Difficulty != 0)
            {
                p.plrPowerups.RemovePowerup(PlayerPowerups.Powerups.AutoBrake);
            }
            // If in tutorial mode, do not remove rapid shot (only section where player can die with a powerup)
            if (!p.gM.tutorialMode)
            {
                p.plrPowerups.RemovePowerup(PlayerPowerups.Powerups.RapidShot);
            }
            p.plrPowerups.RemovePowerup(PlayerPowerups.Powerups.TripleShot);
            p.plrPowerups.RemovePowerup(PlayerPowerups.Powerups.FarShot);
        }
        p.shields = 0;
        p.lives--;
        p.plrUiSound.UpdatePointDisplays();
        p.plrUiSound.powerBar.sprite = p.plrUiSound.powerWhenCharging;

        GameObject newExplosion = Instantiate(p.deathExplosion, transform.position, transform.rotation);
        Destroy(newExplosion, 2f);

        PretendShipDoesntExist();
        p.gM.PlayerLostLife(p.playerNumber);

        // If player out of lives, then tell GM the player is dead. Else, respawn them in 3 seconds.
        if (p.lives < 1)
        {
            p.gM.PlayerDied(p.playerNumber);
        }
        else { p.plrUiSound.prevshields = 80; Invoke("RespawnShip", 3f); }
    }

    public void ShipIsRecovering()
    {
        if (p.shields < 80 && p.shields > 0)
        {
            p.gM.ShowRechargeText();
            StartCoroutine("RecoveringTimer");
        }
    }

    public void RespawnShip()
    {
        if (p.lives > 0 && (p.gM.asteroidCount != 0 || p.gM.tutorialMode))
        {
            // If difficulty is Easy, equip Auto-Brake every respawn
            if (BetweenScenesScript.Difficulty == 0)
            {
                p.plrPowerups.ApplyPowerup(PlayerPowerups.Powerups.AutoBrake, false);
            }
            p.modelPlayer.SetActive(true);
            p.collisionsCanDamage = false;
            p.capsCollider.enabled = true;

            p.rbPlayer.velocity = Vector2.zero;

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
            ShipIsNowTransparent(true);
            StartCoroutine("InvulnTimer");
        }
    }

    // When ship is killed, take 4 seconds total to recharge shields. Ship becomes hittable after those 4s.
    private IEnumerator InvulnTimer()
    {
        // Or, if ship is respawning at start of a level, set the previousShields level to current shield level instead
        if (p.plrUiSound.prevshields != 80) { p.plrUiSound.prevshields = p.shields; }
        p.shields = 0;
        p.plrUiSound.powerBar.sprite = p.plrUiSound.powerWhenCharging; // Set power bar to have no text
        for (int shieldsTick = 0; shieldsTick <= p.plrUiSound.prevshields; shieldsTick++)
        {
            p.shields = shieldsTick;
            yield return new WaitForSeconds(2f / p.plrUiSound.prevshields);
        }
        p.plrUiSound.powerBar.sprite = p.plrUiSound.powerWhenReady; // Set power bar to have text informing power can be used
        ShipIsNowTransparent(false);
        p.collisionsCanDamage = true;
        p.plrUiSound.prevshields = 0;
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
        p.modelPlayer.SetActive(false);
        p.collisionsCanDamage = false;
        p.capsCollider.enabled = false;
    }

    private void ShipIsNowTransparent(bool transpOrSolid)
    {
        Renderer[] listOfShipParts = GetComponentsInChildren<Renderer>();
        float alpha;
        if (transpOrSolid) { alpha = .5f; }
        else { alpha = 1f; }

        foreach (Renderer shipPart in listOfShipParts)
        {
            Material partMaterial = shipPart.material;
            Color origColor = partMaterial.color;
            partMaterial.color = new Color(origColor.r, origColor.g, origColor.b, alpha);
        }
    }
}
