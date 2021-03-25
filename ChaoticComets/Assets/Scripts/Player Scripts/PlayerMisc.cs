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
        if (!GameManager.i.tutorialMode)
        {
            // If spaceship object was resumed from savefile, ask savefile
            if (BetweenScenes.ResumingFromSave)
            {
                SetStatsForPlayer();
                HandlePlayerLifeStates();

                // Report back what was loaded from ships.
                print($"Loaded. Player {p.playerNumber + 1}: {p.shields} shields, {p.credits} credits (shop save amount), " +
                    $"{p.bonus} bonus threshold.");
            }

            SetUpgradeLevelsForPlayer();
        }
    }

    // Set the statistics depending on what player called the function.
    private void SetStatsForPlayer()
    {
        Saving_PlayerManager data = Saving_SaveManager.LoadData();

        p.credits = BetweenScenes.PlayerShopCredits[p.playerNumber];
        p.totalCredits = data.playerList[p.playerNumber].totalCredits;
        p.shields = data.playerList[p.playerNumber].health;
        p.bonus = data.playerList[p.playerNumber].bonusThreshold;
        if (data.playerList[p.playerNumber].powerups[0] == 1) { p.plrPowerups.ApplyPowerup(PlayerPowerups.Powerups.Insurance, false); }
        if (data.playerList[p.playerNumber].powerups[1] == 1) { p.plrPowerups.ApplyPowerup(PlayerPowerups.Powerups.FarShot, false); }
        if (data.playerList[p.playerNumber].powerups[2] == 1) { p.plrPowerups.ApplyPowerup(PlayerPowerups.Powerups.AutoBrake, false); }
        if (data.playerList[p.playerNumber].powerups[3] == 1) { p.plrPowerups.ApplyPowerup(PlayerPowerups.Powerups.RapidShot, false); }
        if (data.playerList[p.playerNumber].powerups[4] == 1) { p.plrPowerups.ApplyPowerup(PlayerPowerups.Powerups.TripleShot, false); }
    }

    private void HandlePlayerLifeStates()
    {
        bool[] tempGMLifeStates = { GameManager.i.player1dead, GameManager.i.player2dead };

        // If one player is dead... disable their model/colliders, then tell GameManager the player is dead.
        if (tempGMLifeStates[p.playerNumber] == true)
        {
            p.plrSpawnDeath.PretendShipDoesntExist();
            GameManager.i.PlayerDied(p.playerNumber);
        }
        //// If a player has died, but brought to life by another player, they'll have >1 life and 0 shields. Give revived player 80 shields.
        //else if (tempGMLifeStates[plrToSet] == true && p.shields == 0)
        //{
        //    p.plrUiSound.prevshields = 80;
        //}
    }

    private void SetUpgradeLevelsForPlayer()
    {
        // Set the current upgrade level for Player
        upgradeSpeed = BetweenScenes.PlayerShopUpgrades[p.playerNumber][0] / 10f;
        upgradeBrake = BetweenScenes.PlayerShopUpgrades[p.playerNumber][1] / 10f;
        upgradeFireRate = BetweenScenes.PlayerShopUpgrades[p.playerNumber][2] / 10f;
        upgradeShotSpeed = BetweenScenes.PlayerShopUpgrades[p.playerNumber][3] / 10f;

        // Bullet force and fire rate are affected by multipliers purchased from the shop
        p.plrWeapons.bulletForce *= upgradeShotSpeed;
        p.plrWeapons.fireRateNormalHeld /= upgradeFireRate;
        p.plrWeapons.fireRateRapid /= upgradeFireRate;
        p.plrWeapons.fireRateTripleHeld /= upgradeFireRate;

        // Thrust and brake efficiency are affected by multipliers purchased from the shop
        p.plrMovement.thrustPower *= upgradeSpeed;
        p.plrMovement.manualBrakePower *= upgradeBrake;

        p.plrUiSound.UpdatePointDisplays();
    }

    internal IEnumerator FadeShip(string inOrOut, float fadingInEndTransparency = 1f)
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
            for (float fadeTick = 0f; fadeTick <= fadingInEndTransparency; fadeTick += 0.1f)
            {
                foreach (Renderer shipPart in listOfShipParts)
                {
                    Material partMaterial = shipPart.material;
                    Color origColor = partMaterial.color;
                    partMaterial.color = new Color(origColor.r, origColor.g, origColor.b, fadeTick);
                }
                yield return new WaitForSeconds(0.1f);
            }
            if (fadingInEndTransparency == 1f)
                p.collisionsCanDamage = true;
        }
    }
}
