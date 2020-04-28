using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPowerups : MonoBehaviour
{
    [SerializeField] PlayerMain p = default;

    // Powerup booleans
    public bool ifInsuranceActive, ifFarShot, ifRetroThruster, ifRapidShot, ifTripleShot;
    private bool powerupUndecided;

    internal void GivePowerup()
    {
        powerupUndecided = true;
        while (powerupUndecided)
        {
            // If all powerups have been collected, then give a random reward of credits or shield refill
            if (ifInsuranceActive && ifFarShot && ifTripleShot && ifRapidShot && ifRetroThruster)
            {
                PowerupDecided(); GivePowerup("AllObtained");
            }
            // If not all have been collected, then run a randomiser and pick a powerup
            else
            {
                int randomiser = Random.Range(0, 100);
                Debug.Log(randomiser);
                if (randomiser < 5)
                { // Award extra life, 1 in 24 chance
                    PowerupDecided(); GivePowerup("ExtraLife");
                }
                else if (randomiser < 20 && !ifFarShot)
                { // Give far shot powerup
                    PowerupDecided(); GivePowerup("FarShot");
                }
                else if (randomiser < 35 && !ifTripleShot)
                { // Give triple shot powerup
                    PowerupDecided(); GivePowerup("TripleShot");
                }
                else if (randomiser < 50 && !ifRapidShot)
                { // Give rapid shot powerup
                    PowerupDecided(); GivePowerup("RapidShot");
                }
                else if (randomiser < 65 && !ifRetroThruster)
                { // Give retro thruster powerup
                    PowerupDecided(); GivePowerup("RetroThruster");
                }
                else if (randomiser < 80 && !ifInsuranceActive && AtLeastOneOtherPowerup())
                { // Give insurance powerup
                    PowerupDecided(); GivePowerup("Insurance");
                }
                // Give shield top-up if randomiser = 80 to 100, and shields are less than 60
                else if (p.shields <= 60f && p.colliderEnabled)
                {
                    PowerupDecided(); GivePowerup("ShieldRefill");
                }
                // If shield regen is selected but shields are fine or ship is respawning, loop back and select another powerup
                // This while loop will not get stuck, because even if the loop somehow gets in an invalid position, an extra life will be granted eventually
            }
        }
        Destroy(p.canister);
        p.audioShipImpact.clip = p.powerupReceived;
        p.audioShipImpact.Play();
    }
    private void PowerupDecided()
    {
        p.gM.AlienAndPowerupLogic("powerupRespawn");
        powerupUndecided = false;
    }

    // Basically only gives insurance powerup once at least one other powerup is received
    // Does this by determining if at least one of the others is received,
    // Then if Easy mode is selected, check that retro thrusters isn't the only one equipped (pointless to insure it)
    // If not at least one powerup has been received yet, tell GivePowerup() to select another powerup
    private bool AtLeastOneOtherPowerup()
    {
        if (ifFarShot || ifTripleShot || ifRapidShot || ifRetroThruster)
        {
            if (BetweenScenesScript.Difficulty == 0 && ifRetroThruster && !ifFarShot && !ifTripleShot && !ifRapidShot)
            {
                return false;
            }
            else { return true; }
        }
        else
        {
            return false;
        }
    }

    public void GivePowerup(string powerup)
    {
        Debug.Log(powerup + " given to player " + p.playerNumber);
        if (powerup == "Insurance")
        {
            ifInsuranceActive = true;
            p.playerUI.insurancePowerup.gameObject.SetActive(true);
        }
        else if (powerup == "FarShot")
        {
            ifFarShot = true;
            p.playerUI.farShotPowerup.gameObject.SetActive(true);
            p.playerWeapons.bulletDestroyTime = 1.4f;
        }
        else if (powerup == "TripleShot")
        {
            ifTripleShot = true;
            p.playerUI.tripleShotPowerup.gameObject.SetActive(true);
        }
        else if (powerup == "RapidShot")
        {
            ifRapidShot = true;
            p.playerUI.rapidShotPowerup.gameObject.SetActive(true);
        }
        else if (powerup == "RetroThruster")
        {
            ifRetroThruster = true;
            p.playerUI.retroThrusterPowerup.gameObject.SetActive(true);
        }
        else if (powerup == "ShieldRefill")
        {
            p.shields = 80;
        }
        else if (powerup == "ExtraLife")
        {
            GrantExtraLife();
        }
        else if (powerup == "AllObtained")
        {
            float randomiser = Random.Range(0f, 1f);
            // 5% chance of extra life
            if (randomiser < 0.05f) {
                GrantExtraLife();
                Debug.Log("All powerups obtained: Give Extra Life");
            }
            // 10% chance of 2000 credits
            if (randomiser < 0.15f) {
                p.ScorePoints(2000);
                Debug.Log("All powerups obtained: Give 2000c");
            }
            // 40% chance of shield refill. If respawning to full shields already, or above 60 shields, skips to last prize
            else if (randomiser < 0.55f && p.shields <= 60f && p.colliderEnabled) {
                GivePowerup("ShieldRefill");
                Debug.Log("All powerups obtained: Give shield refill");
            }
            // 45% chance of 500 credits (85% if respawning or has above 60 shields)
            else {
                p.ScorePoints(500);
                Debug.Log("All powerups obtained: Give 500c");
            }
        }
        else
        {
            Debug.Log("Invalid powerup requested in PlayerPowerups");
        }
    }

    public void GrantExtraLife()
    {
        p.lives++;
        p.audioShipImpact.clip = p.lifeGained;
        p.audioShipImpact.Play();
        p.playerUI.UpdatePointDisplays();
    }
    public void CheatGiveCredits()
    {
        p.credits += 10000;
        p.bonus = 0;
        p.playerUI.UpdatePointDisplays();
    }
}
