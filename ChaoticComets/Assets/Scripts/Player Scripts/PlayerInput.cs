using Rewired;
using System;
using System.Collections;
using UnityEngine;

public class PlayerInput : MonoBehaviour {

    [SerializeField] PlayerMain p = default;
    private Player player;

    // Ship input variables
    public float thrustInput, turnInput;
    internal bool canUseButtons = true, isNotTeleporting = true;

    private void Awake()
    {
        player = ReInput.players.GetPlayer(p.playerNumber);
    }

    // Contains code for receiving inputs from player
    internal void CheckInputs()
    {
        if (canUseButtons)
        {
            turnInput = player.GetAxis("Rotate");
            thrustInput = player.GetAxis("Move");


            if (player.GetButtonDown("Shoot"))
            {
                // If shields are 0, check if player is dead, then if there are enough lives to attempt a respawn. Only allow this if game is unpaused.
                if (p.shields == 0)
                {
                    if (ValidTimeToRespawn())
                    {
                        p.plrSpawnDeath.ShipChoseToRespawn();
                    }
                }
                else if (ValidTimeToFire(p.plrWeapons.nextFireQuickFire))
                {
                    p.plrWeapons.FiringLogic();
                }
            }
            // If fire button is pressed or held, and ship is not teleporting, not dead, and able to fire, then fire
            else if (player.GetButton("Shoot"))
            {
                if (ValidTimeToFire(p.plrWeapons.nextFire))
                {
                    p.plrWeapons.FiringLogic();
                }
            }

            if (player.GetButtonDown("Teleport"))
            {
                UseAbility();
            }
            if (player.GetButtonDown("Pause"))
            {
                UiManager.i.OnPressingPauseButton();
            }
        }
    }

    private void UseAbility()
    {
        // If power button is pressed, and level has no asteroids, then proceed (criteria skipped in tutorial mode).
        // Then check if ship has full power with colliders enabled.
        if (((GameManager.i.asteroidCount != 0) || GameManager.i.tutorialMode))
        {
            if (p.collisionsCanDamage)
            {
                if (p.power == 80)
                    p.plrAbility.HyperspaceStart();
                else
                    p.plrUiSound.audioShipTeleFail.Play();
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
        player = ReInput.players.GetPlayer(1); // Force 2P controls for tutorial
    }

    private bool ValidTimeToFire(float nextValidFiringTime)
    {
        return isNotTeleporting && p.shields != 0 && Time.time > nextValidFiringTime;
    }

    private bool ValidTimeToRespawn()
    {
        bool playerIsDead = p.playerNumber == 0 ? GameManager.i.player1dead : GameManager.i.player2dead;
        return playerIsDead && GameManager.i.playerLives >= 1;
    }
}
