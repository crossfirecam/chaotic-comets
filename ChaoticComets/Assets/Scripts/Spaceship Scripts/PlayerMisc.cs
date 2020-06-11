using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * This class concerns all actions of the Player that don't fit in other class categories.
 */

public class PlayerMisc : MonoBehaviour
{
    [SerializeField] PlayerMain p = default;

    // Upgrade Systems
    public float upgradeSpeed, upgradeBrake, upgradeFireRate, upgradeShotSpeed;

    internal void OtherStartFunctions()
    {
        // If tutorial mode, then ignore all start functions. Else, go ahead.
        if (!p.gM.tutorialMode)
        {
            // If spaceship object was resumed from savefile, ask savefile
            if (BetweenScenes.ResumingFromSave)
            {
                SetStatsForPlayer();
                HandlePlayerLifeStates();

                // Report back what was loaded from ships.
                print($"Loaded. Player {p.playerNumber}: {p.shields} shields, {p.credits} credits (shop save amount), " +
                    $", {p.bonus} bonus threshold, {p.lives} lives.");
            }

            SetUpgradeLevelsForPlayer();
        }
    }

    // Set the statistics depending on what player called the function.
    private void SetStatsForPlayer()
    {
        Saving_PlayerManager data = Saving_SaveManager.LoadData();

        int plrToSet = p.playerNumber - 1;
        p.credits = BetweenScenes.playerShopCredits[plrToSet];
        p.lives = BetweenScenes.playerShopLives[plrToSet];
        p.totalCredits = data.playerList[plrToSet].totalCredits;
        p.shields = data.playerList[plrToSet].health;
        p.bonus = data.playerList[plrToSet].bonusThreshold;
        if (data.playerList[plrToSet].powerups[0] == 1) { p.plrPowerups.ApplyPowerup(PlayerPowerups.Powerups.Insurance, false); }
        if (data.playerList[plrToSet].powerups[1] == 1) { p.plrPowerups.ApplyPowerup(PlayerPowerups.Powerups.FarShot, false); }
        if (data.playerList[plrToSet].powerups[2] == 1) { p.plrPowerups.ApplyPowerup(PlayerPowerups.Powerups.AutoBrake, false); }
        if (data.playerList[plrToSet].powerups[3] == 1) { p.plrPowerups.ApplyPowerup(PlayerPowerups.Powerups.RapidShot, false); }
        if (data.playerList[plrToSet].powerups[4] == 1) { p.plrPowerups.ApplyPowerup(PlayerPowerups.Powerups.TripleShot, false); }
    }

    private void HandlePlayerLifeStates()
    {
        // If one player is dead... disable their model/colliders, then tell GameManager the player is dead.
        if (p.lives == 0)
        {
            p.plrSpawnDeath.PretendShipDoesntExist();
            p.gM.GetComponent<GameManager>().PlayerDied(p.playerNumber);
        }
        // If a player has died, but brought to life by another player, they'll have >1 life and 0 shields. Give revived player 80 shields.
        else if (p.lives > 0 && p.shields == 0)
        {
            p.plrUiSound.prevshields = 80;
        }
    }

    private void SetUpgradeLevelsForPlayer()
    {
        // Set the current upgrade level for Player object depending on which player they are.
        var playerToUpgrade = new List<int[]> { BetweenScenes.UpgradesP1, BetweenScenes.UpgradesP2 };
        upgradeSpeed = playerToUpgrade[p.playerNumber - 1][0] / 10f;
        upgradeBrake = playerToUpgrade[p.playerNumber - 1][1] / 10f;
        upgradeFireRate = playerToUpgrade[p.playerNumber - 1][2] / 10f;
        upgradeShotSpeed = playerToUpgrade[p.playerNumber - 1][3] / 10f;

        // Bullet force and fire rate are affected by multipliers purchased from the shop
        p.plrWeapons.bulletForce *= upgradeShotSpeed;
        p.plrWeapons.fireRateNormal /= upgradeFireRate;
        p.plrWeapons.fireRateRapid /= upgradeFireRate;
        p.plrWeapons.fireRateTriple /= upgradeFireRate;

        // Thrust and brake efficiency are affected by multipliers purchased from the shop
        p.plrInput.thrust *= upgradeSpeed;
        p.plrInput.brakingPower /= upgradeBrake;

        p.plrUiSound.UpdatePointDisplays();
    }

#pragma warning disable IDE0051 // FadeShip isn't directly called, used by a StartCoroutine
    private IEnumerator FadeShip(string inOrOut)
#pragma warning restore IDE0051
    {
        Renderer[] listOfShipParts = GetComponentsInChildren<Renderer>();
        if (inOrOut == "Out")
        {
            p.collisionsCanDamage = false;
            p.plrInput.isNotTeleporting = false;

            for (float fadeTick = 1f; fadeTick >= -0.1f; fadeTick -= 0.1f)
            {
                foreach (Renderer shipPart in listOfShipParts)
                {
                    Material partMaterial = shipPart.material;
                    Color origColor = partMaterial.color;
                    partMaterial.color = new Color(origColor.r, origColor.g, origColor.b, fadeTick);
                }
                yield return new WaitForSeconds(0.1f);
            }
        }
        else
        {
            p.plrInput.isNotTeleporting = true;
            for (float fadeTick = 0f; fadeTick <= 1f; fadeTick += 0.1f)
            {
                foreach (Renderer shipPart in listOfShipParts)
                {
                    Material partMaterial = shipPart.material;
                    Color origColor = partMaterial.color;
                    partMaterial.color = new Color(origColor.r, origColor.g, origColor.b, fadeTick);
                }
                yield return new WaitForSeconds(0.1f);
            }
            p.collisionsCanDamage = true;
        }
    }
}
