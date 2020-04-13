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
            float randomiser = Random.Range(0f, 6f);
            if (randomiser < 0.2f)
            { // Award 1000 credits (least likely - 1/30 chance)
                PowerupDecided(); p.ScorePoints(1000);
            }
            else if (randomiser < 1f && !ifInsuranceActive && AtLeastOneOtherPowerup())
            { // Give insurance powerup
                PowerupDecided(); ifInsuranceActive = true; p.insurancePowerup.gameObject.SetActive(true);
            }
            else if (randomiser < 2f && !ifFarShot)
            { // Give far shot powerup
                PowerupDecided(); ifFarShot = true; p.farShotPowerup.gameObject.SetActive(true);
                p.playerWeapons.bulletDestroyTime = 1.4f;
            }
            else if (randomiser < 3f && !ifTripleShot)
            { // Give triple shot powerup
                PowerupDecided(); ifTripleShot = true; p.tripleShotPowerup.gameObject.SetActive(true);
            }
            else if (randomiser < 4f && !ifRapidShot)
            { // Give rapid shot powerup
                PowerupDecided(); ifRapidShot = true; p.rapidShotPowerup.gameObject.SetActive(true);
            }
            else if (randomiser < 5f && !ifRetroThruster)
            { // Give retro thruster powerup
                PowerupDecided(); ifRetroThruster = true; p.retroThrusterPowerup.gameObject.SetActive(true);
            }
            // Give shield top-up if less than 60 (if respawning, select another powerup)
            else if (randomiser < 6f && p.shields <= 60f && p.colliderEnabled)
            {
                PowerupDecided(); p.shields = 80;
            }
            // If all powerups have been collected, refill shields or award 1000/200 credits
            else if (ifInsuranceActive && ifFarShot && ifTripleShot && ifRapidShot && ifRetroThruster)
            {
                PowerupDecided();
                if (randomiser < 0.2f) { p.ScorePoints(1000); }
                else if (randomiser < 4f && p.shields <= 60f && p.colliderEnabled) { p.shields = 80; } // (if respawning, select a score prize)
                else { p.ScorePoints(200); }
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
}
