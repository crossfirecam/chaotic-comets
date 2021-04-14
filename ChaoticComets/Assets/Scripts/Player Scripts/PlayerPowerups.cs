using UnityEngine;

public class PlayerPowerups : MonoBehaviour
{
    [SerializeField] PlayerMain p = default;

    // Powerup Variables
    internal bool ifInsurance, ifFarShot, ifAutoBrake, ifRapidShot, ifTripleShot;
    private bool powerupUndecided;
    private int powerRandomiser;
    public AudioClip lifeGained, powerupReceived;

    /* ------------------------------------------------------------------------------------------------------------------
     * Powerup Deciding
     * ------------------------------------------------------------------------------------------------------------------ */
    internal void GivePowerup()
    {
        // If in the tutorial, a collected canister will only ever give the first demo, TripleShot
        if (GameManager.i.tutorialMode)
        {
            ApplyPowerup(Powerups.TripleShot);
            return;
        }
        int loopFailsafe = 0;
        powerupUndecided = true;
        while (powerupUndecided)
        {
            // If all powerups have been collected, then give a random reward of credits or shield refill
            if (ifInsurance && ifFarShot && ifTripleShot && ifRapidShot && ifAutoBrake)
            {
                powerupUndecided = false; AllPowerupsObtained();
            }
            // If not all have been collected, then run a randomiser and pick a powerup
            // All powerups are 15% chance, shield top-up is 15%, and extra life is 5%
            else
            {
                powerRandomiser = Random.Range(0, 20);
                if (RandCheck(0, 3) && !ifFarShot)
                { // Give far shot powerup, 15% chance
                    powerupUndecided = false; ApplyPowerup(Powerups.FarShot);
                }
                else if (RandCheck(3, 6) && !ifTripleShot)
                { // Give triple shot powerup, 15% chance
                    powerupUndecided = false; ApplyPowerup(Powerups.TripleShot);
                }
                else if (RandCheck(6, 9) && !ifRapidShot)
                { // Give rapid shot powerup, 15% chance
                    powerupUndecided = false; ApplyPowerup(Powerups.RapidShot);
                }
                else if (RandCheck(9, 12) && !ifAutoBrake)
                { // Give auto brake powerup, 15% chance
                    powerupUndecided = false; ApplyPowerup(Powerups.AutoBrake);
                }
                else if (RandCheck(12, 15) && !ifInsurance && AtLeastOneOtherPowerup())
                { // Give insurance powerup, 15% chance, needs another powerup active
                    powerupUndecided = false; ApplyPowerup(Powerups.Insurance);
                }
                else if (RandCheck(15, 19) && p.shields <= 60f && p.collisionsCanDamage)
                { // Give shield top-up, 15% chance, needs shields to be less than 60 and ship's collider to be active
                    powerupUndecided = false; ApplyPowerup(Powerups.ShieldRefill);
                }
                else if (powerRandomiser == 19)
                { // Award extra life, 5% chance
                    powerupUndecided = false; ApplyPowerup(Powerups.ExtraLife);
                }
            }
            if (loopFailsafe > 20)
            {
                Debug.LogWarning($"Powerup selection has failed after 20 rolls. Give extra life to avoid infinite loop.");
                powerupUndecided = false; ApplyPowerup(Powerups.ExtraLife);
            }
            loopFailsafe++;
        }
    }

    // 

    /// <summary>
    /// Basically only gives insurance powerup once at least one other powerup is received.<br/>
    /// Does this by determining if at least one of the others is received,<br/>
    /// If not at least one powerup has been received yet, tell GivePowerup() to select another powerup
    /// </summary>
    private bool AtLeastOneOtherPowerup()
    {
        if (ifFarShot || ifTripleShot || ifRapidShot || ifAutoBrake)
        {
            // If Easy mode is selected, check that auto-brake isn't the only one equipped (pointless to insure it)
            if (BetweenScenes.Difficulty == 0 && ifAutoBrake && !ifFarShot && !ifTripleShot && !ifRapidShot)
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
    public void AllPowerupsObtained()
    {
        print("All powerups obtained");
        powerRandomiser = Random.Range(0, 20);
        // 5% chance of extra life.
        if (RandCheck(0, 1))
        {
            ApplyPowerup(Powerups.ExtraLife);
        }
        // 55% chance of shield refill, and only if shields are below 60, and if shields are allowed to be recharged
        else if (RandCheck(1, 12) && p.shields <= 60f && p.collisionsCanDamage)
        {
            ApplyPowerup(Powerups.ShieldRefill);
        }
        // 40% of the time or if shield refill fails, give 1500 credits
        else
        {
            ApplyPowerup(Powerups.PointPrize);
        }
    }

    private bool RandCheck(int min, int max)
    {
        return (powerRandomiser >= min && powerRandomiser < max);
    }

    /* ------------------------------------------------------------------------------------------------------------------
     * Powerup Application, Removal
     * ------------------------------------------------------------------------------------------------------------------ */

    public enum Powerups { Insurance, FarShot, RapidShot, AutoBrake, TripleShot, ShieldRefill, ExtraLife, PointPrize };

    public void ApplyPowerup(Powerups powerup, bool playSound = true)
    {
        // If powerup is one that triggers a UI icon to turn on, then tell UiManager
        if ((int)powerup <= 4)
        {
            UiManager.i.SetPlayerPowerup(p.playerNumber, (int)powerup, true);
        }

        switch (powerup)
        {
            case Powerups.Insurance:
                ifInsurance = true;
                break;
            case Powerups.FarShot:
                ifFarShot = true;
                p.plrWeapons.bulletDestroyTime = p.plrWeapons.bulletTimeIfFar;
                break;
            case Powerups.TripleShot:
                ifTripleShot = true;
                break;
            case Powerups.RapidShot:
                ifRapidShot = true;
                break;
            case Powerups.AutoBrake:
                ifAutoBrake = true;
                break;
            case Powerups.ShieldRefill:
                p.shields = 80;
                break;
            
            case Powerups.ExtraLife:
                GrantExtraLife();
                break;
            case Powerups.PointPrize:
                p.ScorePoints(1500);
                break;
            default:
                print("Invalid powerup requested in PlayerPowerups");
                break;

        }
        // When the ship is given an extra life, it plays the sound effect itself.
        // Other powerup, play the sound - unless credited to player after a shop screen.
        if (powerup != Powerups.ExtraLife && playSound)
        {
            print($"{powerup} given to player {p.playerNumber + 1}");
            p.plrUiSound.audioShipSFX.clip = powerupReceived;
            p.plrUiSound.audioShipSFX.Play();
        }
    }

    public void GrantExtraLife()
    {
        GameManager.i.PlayerGainedLife();
        p.plrUiSound.audioShipSFX.clip = lifeGained;
        p.plrUiSound.audioShipSFX.Play();
        p.plrUiSound.UpdatePointDisplays();
    }
    public void RemovePowerup(Powerups powerup)
    {
        // If powerup is one that triggers a UI icon to turn off, then tell UiManager
        if ((int)powerup <= 4)
        {
            UiManager.i.SetPlayerPowerup(p.playerNumber, (int)powerup, false);
        }

        switch (powerup)
        {
            case Powerups.Insurance:
                ifInsurance = false;
                break;
            case Powerups.FarShot:
                ifFarShot = false;
                p.plrWeapons.bulletDestroyTime = p.plrWeapons.bulletTimeIfNormal;
                break;
            case Powerups.TripleShot:
                ifTripleShot = false;
                break;
            case Powerups.RapidShot:
                ifRapidShot = false;
                break;
            case Powerups.AutoBrake:
                ifAutoBrake = false;
                break;
            default:
                print("Invalid powerup requested in PlayerPowerups");
                break;
        }
    }


    /* ------------------------------------------------------------------------------------------------------------------
     * Cheat Methods
     * ------------------------------------------------------------------------------------------------------------------ */

    // This function only exists because Enum values can't be used with Unity's button OnClick
    public void CheatGivePowerup(string powerup)
    {

        if (powerup == "Random")
        {
            GivePowerup();
        }
        else if (Powerups.TryParse(powerup, out Powerups powerupToTry))
        {
            ApplyPowerup(powerupToTry);
        }
        else
        {
            print("Unity Button attempted to spawn an invalid powerup.");
        }
    }

    // Give cheating player 5k credits
    public void CheatGiveCredits()
    {
        p.bonus += 5000;
        p.ScorePoints(5000);
    }
}
