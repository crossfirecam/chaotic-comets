using UnityEngine;

public class PlayerUiSounds : MonoBehaviour
{
    [SerializeField] PlayerMain p = default;

    // UI Systems
    internal readonly int bonusInterval = 10000;
    internal float prevshields;

    // Sound Systems
    public AudioSource audioShipThrust, audioShipAutoBrake, audioShipSFX; // Thrust: passive thruster noise, SFX: powerup, extra life, impact noises
    public AudioClip audClipPlrSfxImpactSoft, audClipPlrSfxDeath;

    public void UpdatePointDisplays()
    {
        // If total credits are higher than bonus threshold, then grant a life
        if (p.totalCredits > p.bonus && !GameManager.i.tutorialMode)
        {
            p.bonus += bonusInterval;
            p.plrPowerups.GrantExtraLife();
        }

        UiManager.i.SetPlayerCredits(p.playerNumber, p.credits, p.totalCredits);
        UiManager.i.SetShipsText(GameManager.i.playerLives);
    }

    public AudioSource[] ReturnPlayerSounds()
    {
        AudioSource[] playerSfx = { audioShipThrust, audioShipSFX, audioShipAutoBrake,
            p.plrAbility.teleportIn.GetComponent<AudioSource>(), p.plrAbility.teleportOut.GetComponent<AudioSource>() };
        return playerSfx;
    }
}
