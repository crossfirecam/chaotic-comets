using Rewired;
using System.Collections;
using UnityEngine;

public class PlayerInput : MonoBehaviour {

    [SerializeField] PlayerMain p = default;
    private int playerId;
    private Player player;

    // Ship input variables
    public float thrustInput, turnInput;
    internal bool canUseButtons = true, isNotTeleporting = true;

    private void Awake()
    {
        playerId = p.playerNumber - 1;
        player = ReInput.players.GetPlayer(playerId);
    }

    // Contains code for receiving inputs from player
    internal void CheckInputs()
    {
        if (canUseButtons)
        {
            turnInput = player.GetAxis("Rotate");
            thrustInput = player.GetAxis("Move");

            // If fire button is pressed or held, and ship is not teleporting, not dead, and able to fire, then fire
            if (player.GetButton("Shoot"))
            {
                if (isNotTeleporting && p.shields != 0 && Time.time > p.plrWeapons.nextFire)
                {
                    p.plrWeapons.FiringLogic();
                }
            }

            if (player.GetButtonDown("Ability"))
            {
                UseAbility();
            }
            if (player.GetButtonDown("Pause"))
            {
                p.gM.OnPause();
            }
        }
    }

    private void UseAbility()
    {
        // If power button is pressed, and level has no asteroids, then proceed (criteria skipped in tutorial mode).
        // Then check if ship has full power with colliders enabled.
        if (((p.gM.asteroidCount != 0) || p.gM.tutorialMode))
        {
            if (p.collisionsCanDamage && p.power == 80)
            {
                p.plrUiSound.abilityBar.sprite = p.plrUiSound.abilityWhenCharging;
                p.plrAbility.teleportIn.SetActive(true);
                p.plrMisc.StartCoroutine("FadeShip", "Out");
                p.plrAbility.Invoke("Hyperspace", 2f);
            }
        }
    }

    public IEnumerator DelayNewInputs()
    {
        canUseButtons = false;
        yield return new WaitForSeconds(0.2f);
        canUseButtons = true;
    }

    public void SwapToP2InputForTutorial()
    {
        playerId = 1;
        player = ReInput.players.GetPlayer(playerId);
    }
}
