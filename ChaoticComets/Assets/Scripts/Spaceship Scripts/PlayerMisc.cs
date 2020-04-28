using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * This class concerns all actions of the Player that don't fit in other class categories.
 */

public class PlayerMisc : MonoBehaviour
{
    [SerializeField] PlayerMain p = default;

    internal void OtherStartFunctions()
    {
        // If the difficulty is 9 (indicating player is in help menu) then ignore all start functions. Else, go ahead.
        if (BetweenScenesScript.Difficulty != 9)
        {
            // If spaceship object created on first load, set default stats
            // Else, spaceship object was resumed from savefile, ask savefile
            if (!BetweenScenesScript.ResumingFromSave)
            {
                p.credits = 0; p.bonus = 9999; p.lives = 3; p.shields = 80;
            }
            else
            {
                SetStatsForPlayer();
                HandlePlayerLifeStates();

                // Report back what was loaded from ships.
                Debug.Log($"Loaded. Player {p.playerNumber}: {p.shields} shields, {p.credits} credits (shop save amount), " +
                    $"{BetweenScenesScript.player1TempCredits} credits (right now, after shop), {p.bonus} bonus threshold, {p.lives} lives.");
            }

            SetUpgradeLevelsForPlayer();
        }
    }

    // A function desperately in need of compressing. TODO.
    // What it does is set the statistics depending on what player called the function.
    private void SetStatsForPlayer()
    {
        Saving_PlayerManager data = Saving_SaveManager.LoadData();
        if (p.playerNumber == 1)
        {
            p.credits = BetweenScenesScript.player1TempCredits;
            p.lives = BetweenScenesScript.player1TempLives;
            p.shields = data.player1health;
            p.bonus = data.player1bonus;
            if (data.player1powerups[0] == 1) { p.playerPowerups.ifInsuranceActive = true; p.playerUI.insurancePowerup.gameObject.SetActive(true); }
            if (data.player1powerups[1] == 1)
            {
                p.playerPowerups.ifFarShot = true; p.playerUI.farShotPowerup.gameObject.SetActive(true);
                p.playerWeapons.bulletDestroyTime = 1.4f;
            }
            if (data.player1powerups[2] == 1) { p.playerPowerups.ifRetroThruster = true; p.playerUI.retroThrusterPowerup.gameObject.SetActive(true); }
            if (data.player1powerups[3] == 1) { p.playerPowerups.ifRapidShot = true; p.playerUI.rapidShotPowerup.gameObject.SetActive(true); }
            if (data.player1powerups[4] == 1) { p.playerPowerups.ifTripleShot = true; p.playerUI.tripleShotPowerup.gameObject.SetActive(true); }
        }
        else
        { // (playerNumber == 2)
            p.credits = BetweenScenesScript.player2TempCredits;
            p.lives = BetweenScenesScript.player2TempLives;
            p.shields = data.player2health;
            p.bonus = data.player2bonus;
            if (data.player2powerups[0] == 1) { p.playerPowerups.ifInsuranceActive = true; p.playerUI.insurancePowerup.gameObject.SetActive(true); }
            if (data.player2powerups[1] == 1)
            {
                p.playerPowerups.ifFarShot = true; p.playerUI.farShotPowerup.gameObject.SetActive(true);
                p.playerWeapons.bulletDestroyTime = 1.4f;
            }
            if (data.player2powerups[2] == 1) { p.playerPowerups.ifRetroThruster = true; p.playerUI.retroThrusterPowerup.gameObject.SetActive(true); }
            if (data.player2powerups[3] == 1) { p.playerPowerups.ifRapidShot = true; p.playerUI.rapidShotPowerup.gameObject.SetActive(true); }
            if (data.player2powerups[4] == 1) { p.playerPowerups.ifTripleShot = true; p.playerUI.tripleShotPowerup.gameObject.SetActive(true); }
        }
    }

    private void HandlePlayerLifeStates()
    {
        // If one player is dead... disable their sprite/colliders, then tell GameManager the player is dead.
        if (p.lives == 0)
        {
            p.playerSpawnDeath.PretendShipDoesntExist();
            p.gM.GetComponent<GameManager>().PlayerDied(p.playerNumber);
        }
        // If a player has died, but brought to life by another player, they'll have >1 life and 0 shields. Give revived player 80 shields.
        else if (p.lives > 0 && p.shields == 0)
        {
            p.playerUI.prevshields = 80;
        }
    }

    private void SetUpgradeLevelsForPlayer()
    {
        // Set the current upgrade level for Player object depending on which player they are.
        var playerToUpgrade = new List<int[]> { BetweenScenesScript.UpgradesP1, BetweenScenesScript.UpgradesP2 };
        p.upgradeSpeed = playerToUpgrade[p.playerNumber - 1][0] / 10f;
        p.upgradeBrake = playerToUpgrade[p.playerNumber - 1][1] / 10f;
        p.upgradeFireRate = playerToUpgrade[p.playerNumber - 1][2] / 10f;
        p.upgradeShotSpeed = playerToUpgrade[p.playerNumber - 1][3] / 10f;

        // Bullet force and fire rate are affected by multipliers purchased from the shop
        p.playerWeapons.bulletForce *= p.upgradeShotSpeed;
        p.playerWeapons.fireRateNormal /= p.upgradeFireRate;
        p.playerWeapons.fireRateRapid /= p.upgradeFireRate;
        p.playerWeapons.fireRateTriple /= p.upgradeFireRate;

        // Thrust and brake efficiency are affected by multipliers purchased from the shop
        p.playerInput.thrust *= p.upgradeSpeed;
        p.playerInput.brakingPower /= p.upgradeBrake;

        // Set text for credits & lives (TODO move)
        p.playerUI.scoreText.text = "Credits:\n" + p.credits;
        p.playerUI.livesText.text = "Lives: " + p.lives;
    }

    private IEnumerator FadeShip(string inOrOut)
    {
        if (inOrOut == "Out")
        {
            p.colliderEnabled = false;
            p.playerInput.isNotTeleporting = false;
            for (float fadeTick = 1f; fadeTick >= 0f; fadeTick -= 0.1f)
            {
                gameObject.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, fadeTick);
                yield return new WaitForSeconds(0.1f);
            }
            gameObject.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0f);
        }
        else
        {
            p.playerInput.isNotTeleporting = true;
            for (float fadeTick = 0f; fadeTick <= 1f; fadeTick += 0.1f)
            {
                gameObject.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, fadeTick);
                yield return new WaitForSeconds(0.1f);
            }
            gameObject.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
            p.colliderEnabled = true;
        }
    }
}
