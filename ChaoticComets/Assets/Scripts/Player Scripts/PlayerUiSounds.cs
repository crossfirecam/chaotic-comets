using UnityEngine;

public class PlayerUiSounds : MonoBehaviour
{
    [SerializeField] PlayerMain p = default;

    // UI Systems
    const int bonusInterval = 5000;
    internal float prevshields;

    // Sound Systems
    public AudioSource audioShipThrust, audioShipAutoBrake, audioShipSFX; // Thrust: passive thruster noise, SFX: powerup, extra life, impact noises
    public AudioClip audClipPlrSfxImpactSoft, audClipPlrSfxImpactHard, audClipPlrSfxDeath;

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

    public GameObject[] ReturnPlayerSounds()
    {
        GameObject[] playerSfx = { audioShipThrust.gameObject, audioShipSFX.gameObject, audioShipAutoBrake.gameObject,
            p.plrAbility.teleportIn, p.plrAbility.teleportOut };
        return playerSfx;
    }
}
