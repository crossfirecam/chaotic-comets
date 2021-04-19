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
    public float upgradeSpeed, upgradeBrake, upgradeTeleportRate, upgradeAutoRate, upgradeShotSpeed, upgradeShotRange, upgradeShield;
    public int upgradeShotLimit;

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

            if (BetweenScenes.Difficulty == 3)
            {
                p.damageFromImpact *= 1.5f;
                p.damageFromUfoBullet *= 1.5f;
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
        p.shields = BetweenScenes.PlayerShopShields[p.playerNumber];
        p.bonus = data.playerList[p.playerNumber].bonusThreshold;
        if (data.playerList[p.playerNumber].powerups[0] == 1) { p.plrPowerups.ApplyPowerup(PlayerPowerups.Powerups.Insurance, false); }
        if (data.playerList[p.playerNumber].powerups[1] == 1) { p.plrPowerups.ApplyPowerup(PlayerPowerups.Powerups.FarShot, false, false); } // Adding Far Shot at start doesn't set BulletTime
        if (data.playerList[p.playerNumber].powerups[2] == 1) { p.plrPowerups.ApplyPowerup(PlayerPowerups.Powerups.RapidShot, false); }
        if (data.playerList[p.playerNumber].powerups[3] == 1) { p.plrPowerups.ApplyPowerup(PlayerPowerups.Powerups.AutoBrake, false); }
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
    }

    private void SetUpgradeLevelsForPlayer()
    {
        // Most upgrades are in intervals of 20%
        upgradeShield = 1 + (BetweenScenes.PlayerShopUpgrades[p.playerNumber][0] / 2f); // Shield upgrade is in intervals of 50%
        upgradeTeleportRate = 1 + (BetweenScenes.PlayerShopUpgrades[p.playerNumber][1] / 5f);
        upgradeSpeed = 1 + (BetweenScenes.PlayerShopUpgrades[p.playerNumber][2] / 10f); // Weaken the speed upgrade
        upgradeBrake = 1 + (BetweenScenes.PlayerShopUpgrades[p.playerNumber][3] / 5f);
        upgradeAutoRate = 1 + (BetweenScenes.PlayerShopUpgrades[p.playerNumber][4] / 5f);
        upgradeShotLimit = BetweenScenes.PlayerShopUpgrades[p.playerNumber][5]; // Shot limit upgrade is in intervals of 1 shot
        upgradeShotSpeed = 1 + (BetweenScenes.PlayerShopUpgrades[p.playerNumber][6] / 5f);
        upgradeShotRange = 1 + (BetweenScenes.PlayerShopUpgrades[p.playerNumber][7] / 5f);

        p.damageFromImpact /= upgradeShield;
        p.damageFromUfoBullet /= upgradeShield;
        p.plrAbility.powerChargeTime /= upgradeTeleportRate;

        p.plrMovement.thrustPower *= upgradeSpeed;
        p.plrMovement.manualBrakePower *= upgradeBrake;

        p.plrWeapons.fireRateNormalHeld /= upgradeAutoRate;
        p.plrWeapons.fireRateTripleHeld /= upgradeAutoRate;

        p.plrWeapons.capOfActiveBullets += (int) upgradeShotLimit;
        p.plrWeapons.bulletForce *= upgradeShotSpeed;
        p.plrWeapons.bulletRangeMultipler *= upgradeShotRange;

        p.plrUiSound.UpdatePointDisplays();

        // Set the base bullet timing.
        p.plrWeapons.SetBulletTime();
    }

    internal IEnumerator FadeShip(string inOrOut, float fadingInEndTransparency = 1f)
    {
        Renderer[] listOfShipParts = p.modelPlayer.GetComponentsInChildren<Renderer>();
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
                yield return new WaitForSeconds(0.03f);
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
