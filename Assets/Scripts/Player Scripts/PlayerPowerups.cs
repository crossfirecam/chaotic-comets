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
                powerRandomiser = Random.Range(0, 100);
                if (RandCheck(0, 20) && !ifFarShot)
                { // Give far shot powerup, 20% chance
                    powerupUndecided = false; ApplyPowerup(Powerups.FarShot);
                }
                else if (RandCheck(21, 40) && !ifTripleShot)
                { // Give triple shot powerup, 20% chance
                    powerupUndecided = false; ApplyPowerup(Powerups.TripleShot);
                }
                else if (RandCheck(41, 60) && !ifRapidShot)
                { // Give rapid shot powerup, 20% chance
                    powerupUndecided = false; ApplyPowerup(Powerups.RapidShot);
                }
                else if (RandCheck(61, 80) && !ifAutoBrake)
                { // Give auto brake powerup, 20% chance
                    powerupUndecided = false; ApplyPowerup(Powerups.AutoBrake);
                }
                else if (RandCheck(81, 90) && !ifInsurance && AtLeastOneOtherPowerup())
                { // Give insurance powerup, 10% chance, needs another powerup active
                    powerupUndecided = false; ApplyPowerup(Powerups.Insurance);
                }
                else if (RandCheck(91, 97) && p.shields <= 60f && p.collisionsCanDamage)
                { // Give shield top-up, 8% chance, needs shields to be less than 60 and ship's collider to be active
                    powerupUndecided = false; ApplyPowerup(Powerups.ShieldRefill);
                }
                else
                { // Award extra life, 2% chance (or 10% at >60 shields)
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
    /// Only gives insurance powerup when least one other powerup is received.<br/>
    /// </summary>
    private bool AtLeastOneOtherPowerup()
    {
        return ifFarShot || ifTripleShot || ifRapidShot || ifAutoBrake;
    }
    public void AllPowerupsObtained()
    {
        print("All powerups obtained");
        powerRandomiser = Random.Range(0, 100);
        // 55% chance of shield refill, and only if shields are below 60, and if shields are allowed to be recharged
        if (RandCheck(1, 55) && p.shields <= 60f && p.collisionsCanDamage)
        {
            ApplyPowerup(Powerups.ShieldRefill);
        }
        // 40% of the time or if shield refill fails, give 1500 credits
        else if (RandCheck(56, 95))
        {
            ApplyPowerup(Powerups.PointPrize);
        }
        // 5% chance of extra life.
        else
        {
            ApplyPowerup(Powerups.ExtraLife);
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

    public void ApplyPowerup(Powerups powerup, bool playSound = true, bool updateBulletTime = true)
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
                if (updateBulletTime) p.plrWeapons.SetBulletTime();
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
                p.plrWeapons.SetBulletTime();
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
        if (Powerups.TryParse(powerup, out Powerups powerupToTry))
        {
            // Cancel cheat if the powerup already exists
            if (powerupToTry == Powerups.Insurance) { if (ifInsurance) return; }
            if (powerupToTry == Powerups.FarShot) { if (ifFarShot) return; }
            if (powerupToTry == Powerups.AutoBrake) { if (ifAutoBrake) return; }
            if (powerupToTry == Powerups.RapidShot) { if (ifRapidShot) return; }
            if (powerupToTry == Powerups.TripleShot) { if (ifTripleShot) return; }

            ApplyPowerup(powerupToTry);
            if (powerupToTry == Powerups.ShieldRefill)
                UiManager.i.SetPlayerStatusBars(p.playerNumber, p.shields, p.power);
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
