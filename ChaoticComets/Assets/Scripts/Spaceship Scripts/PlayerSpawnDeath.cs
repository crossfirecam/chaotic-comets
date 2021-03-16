using System.Collections;
using UnityEngine;

/*
 * This class concerns all actions of the Player to do with spawning, death, and related code.
 */

public class PlayerSpawnDeath : MonoBehaviour
{
    [SerializeField] PlayerMain p = default;
    private bool shipIsRespawning = false;

    internal void ShipIsDead()
    {
        // If insurance is active, do not remove any powerup except for Insurance
        // Else, remove all powerups
        if (p.plrPowerups.ifInsurance)
        {
            p.plrPowerups.RemovePowerup(PlayerPowerups.Powerups.Insurance);
            print($"Insurance powerup removed from player {p.playerNumber}");
        }
        else
        {
            // If difficulty is easy, do not remove Auto-Brake
            if (BetweenScenes.Difficulty != 0)
                p.plrPowerups.RemovePowerup(PlayerPowerups.Powerups.AutoBrake);

            // If in tutorial mode, do not remove rapid shot (only section where player can die with a powerup)
            if (!p.gM.tutorialMode)
                p.plrPowerups.RemovePowerup(PlayerPowerups.Powerups.RapidShot);

            p.plrPowerups.RemovePowerup(PlayerPowerups.Powerups.TripleShot);
            p.plrPowerups.RemovePowerup(PlayerPowerups.Powerups.FarShot);
            print($"Non-insurance powerups removed from player {p.playerNumber}");
        }
        p.shields = 0;
        if (p.gM.playerLives != 0) // Only remove a life from the counter if the players have at least one to spare.
        {
            p.plrUiSound.prevshields = 80; Invoke(nameof(RespawnShip), 3f);
            p.gM.playerLives--;
        }
        else // If life counter is 0 when a player dies, they've depleted the life counter and need to stay dead
        {
            p.gM.PlayerDied(p.playerNumber);
            p.plrUiSound.ShowRespawnOverlay(true);
            p.plrUiSound.UpdateRespawnStatus();
        }
        p.plrUiSound.UpdatePointDisplays();

        GameObject newExplosion = Instantiate(p.deathExplosion, transform.position, transform.rotation);
        Destroy(newExplosion, 2f);

        PretendShipDoesntExist();
        p.gM.PlayerLostLife(p.playerNumber);
    }

    public void ShipIsRecovering()
    {
        if (p.shields < 80 && p.shields > 0)
        {
            p.gM.ShowRechargeText();
            StartCoroutine(nameof(RecoveringTimer));
        }
    }

    public void RespawnShip()
    {
        if (p.gM.playerLives >= 0 || p.gM.tutorialMode)
        {
            // If difficulty is Easy, equip Auto-Brake every respawn
            if (BetweenScenes.Difficulty == 0)
                p.plrPowerups.ApplyPowerup(PlayerPowerups.Powerups.AutoBrake, false);

            // Player becomes visible, collision damage is disabled until shields recharge, but collider itself is enabled to rebound objects
            p.modelPlayer.SetActive(true);
            p.collisionsCanDamage = false;
            p.capsCollider.enabled = true;

            p.rbPlayer.velocity = Vector2.zero;

            // If at least one player is dead, place the other in the center of the screen
            // If both players exist, place them apart
            if (p.gM.player1dead || p.gM.player2dead)
                transform.position = new Vector2(0f, 0f);
            else
            {
                if (p.playerNumber == 1) { transform.position = new Vector2(-3f, 0f); }
                else { transform.position = new Vector2(3f, 0f); }
            }

            // Alert GameManger that their temporary death is over for UFO tracking
            if (p.playerNumber == 1)
                p.gM.player1TEMPDEAD = false;
            else
                p.gM.player2TEMPDEAD = false;

            ShipIsNowTransparent(true);
            StartCoroutine("InvulnTimer");
        }
    }

    // When ship is killed, take 4 seconds total to recharge shields. Ship becomes hittable after those 4s.
    private IEnumerator InvulnTimer()
    {
        shipIsRespawning = true;
        // If ship is respawning at start of a level, set the previousShields level to current shield level
        if (p.plrUiSound.prevshields != 80) { p.plrUiSound.prevshields = p.shields; }
        p.shields = 0;
        for (int shieldsTick = 0; shieldsTick <= p.plrUiSound.prevshields; shieldsTick++)
        {
            p.shields = shieldsTick;
            yield return new WaitForSeconds(2f / p.plrUiSound.prevshields);
        }
        shipIsRespawning = false;
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
            if (p.shields == 80 || shipIsRespawning) { break; } // If a canister is picked up during regen, or if ship is respawning after a death, break the loop
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

    // When the team of 2 players loses all reserve lives, respawning is impossible.
    // Alive teammate gains a life: Remind the dead player that they can respawn.
    // Alive teammate loses a life while this player still hasn't respawned: Remind dead player that they cannot respawn.
    public void RefreshRespawnStatus()
    {
        p.plrUiSound.UpdateRespawnStatus();
        if (p.gM.playerLives == 0)
        {
            Debug.Log("Plr " + p.playerNumber + " cannot respawn");
        }
        else
        {
            Debug.Log("Plr " + p.playerNumber + " can respawn");
        }
    }

    public void ShipChoseToRespawn()
    {
        p.gM.PlayerChoseToRespawn();
        RespawnShip();
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
