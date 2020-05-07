using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInput : MonoBehaviour {

    [SerializeField] PlayerMain p = default;

    // Ship movement & teleport variables
    public float thrust, turnThrust;
    internal float brakingPower = 2f;
    public float thrustInput, turnInput;
    internal bool isNotTeleporting = true;
    public ParticleSystem thruster1, thruster2;

    // Contains code for receiving inputs from player
    internal void GetInputs()
    {
        // Get axis-based inputs
        thrustInput = -Input.GetAxis($"Thrust{p.inputNameInsert}");
        turnInput = -Input.GetAxis($"Rotate Ship{p.inputNameInsert}");

        // Get button-based inputs
        // If fire button is pressed, and ship is not teleporting, not dead, and able to fire, then fire
        if (Input.GetButton($"Primary Fire{p.inputNameInsert}")
            && isNotTeleporting && p.shields != 0 && Time.time > p.plrWeapons.nextFire)
        {
            p.plrWeapons.FiringLogic();
        }
        // If power button is pressed, and ship has full power with colliders enabled, and level has no asteroids, then use power
        if (Input.GetButtonDown($"Power{p.inputNameInsert}") && p.gM.asteroidCount != 0)
        {
            if (p.colliderEnabled && p.power == 80)
            {
                p.plrUiSound.powerBar.sprite = p.plrUiSound.powerWhenCharging;
                p.plrAbility.teleportIn.SetActive(true);
                p.plrMisc.StartCoroutine("FadeShip", "Out");
                p.plrAbility.Invoke("Hyperspace", 2f);
            }
        }
    }

    // Alters inputTypeAdd string, so that only buttons on the selected controller work
    internal void InputChoice()
    {
        if (BetweenScenesScript.ControlTypeP1 == 0 && p.playerNumber == 1)
        {
            p.inputNameInsert = " (P1joy)";
        }
        else if (BetweenScenesScript.ControlTypeP1 == 1 && p.playerNumber == 1)
        {
            p.inputNameInsert = " (P1key)";
        }
        else if (BetweenScenesScript.ControlTypeP2 == 0 && p.playerNumber == 2)
        {
            p.inputNameInsert = " (P2joy)";
        }
        else if (BetweenScenesScript.ControlTypeP2 == 1 && p.playerNumber == 2)
        {
            p.inputNameInsert = " (P2key)";
        }
        else
        {
            Debug.LogError("Invalid player/controller configuration.");
        }
    }
}
