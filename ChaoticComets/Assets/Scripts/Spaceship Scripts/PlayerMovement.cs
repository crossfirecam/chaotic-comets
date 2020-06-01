using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    [SerializeField] PlayerMain p = default;
    private bool retroThrustEngaged = false;
    public ParticleSystem thruster1, thruster2, retroThruster1, retroThruster2;

    public void ShipMovement()
    {
        // Rotate the ship
        if (p.plrInput.turnInput != 0 && p.modelPlayer.activeInHierarchy)
        {
            transform.Rotate(Vector3.forward * -p.plrInput.turnInput * Time.deltaTime * p.plrInput.turnThrust);
        }

        // Active thrusting (forward or braking thrust)
        // Apply force on Y axis of spaceship, multiply by thrust
        if (p.plrInput.thrustInput != 0 && p.modelPlayer.activeInHierarchy && p.plrInput.isNotTeleporting)
        {
            if (retroThrustEngaged)
                retroThrustEngaged = false;

            if (!p.plrUiSound.audioShipThrust.isPlaying && DialogsNotOpen())
                p.plrUiSound.audioShipThrust.Play();

            if (!thruster1.isPlaying) { thruster1.Play(); }
            if (!thruster2.isPlaying) { thruster2.Play(); }

            // If thrust is less than 0, then ship is braking. On hard difficulty, brake is less powerful.
            if (p.plrInput.thrustInput < 0)
            {
                if (BetweenScenesScript.Difficulty != 2)
                    p.rbPlayer.drag = p.rbPlayer.velocity.magnitude / p.plrInput.brakingPower;
                else
                    p.rbPlayer.drag = p.rbPlayer.velocity.magnitude / p.plrInput.brakingPower / 2;

                // If ship is slow enough, stop it
                if (p.rbPlayer.velocity.magnitude < 0.8f)
                    p.rbPlayer.velocity = new Vector2(0, 0);
            }
            // If thrust is more than 0, then ship is moving forward.
            else
            {
                p.rbPlayer.AddRelativeForce(Vector2.up * p.plrInput.thrustInput * Time.deltaTime * p.plrInput.thrust);
                p.rbPlayer.drag = p.rbPlayer.velocity.magnitude / 10f;
            }
        }
        // Passive Drag (no thruster controls pressed)
        // Apply passive drag depending on if retro thrusters are equipped or not
        else
        {
            if (p.plrUiSound.audioShipThrust.isPlaying) { p.plrUiSound.audioShipThrust.Stop(); }
            if (thruster1.isPlaying) { thruster1.Stop(); }
            if (thruster2.isPlaying) { thruster2.Stop(); }
            if (p.plrPowerups.ifRetroThruster && p.rbPlayer.velocity.magnitude != 0)
            {
                if (!retroThrustEngaged && p.rbPlayer.velocity.magnitude > 4)
                {
                    print(p.rbPlayer.velocity.magnitude);
                    p.plrUiSound.audioShipRetroThrust.Play();
                    retroThrustEngaged = true;
                    retroThruster1.Play();
                    retroThruster2.Play();
                }
                if (BetweenScenesScript.Difficulty != 2)
                {
                    p.rbPlayer.drag = p.rbPlayer.velocity.magnitude / 0.2f;
                }
                else
                {
                    p.rbPlayer.drag = p.rbPlayer.velocity.magnitude / 2f;
                }
                // If ship is slow enough, stop it
                if (p.rbPlayer.velocity.magnitude < 0.4f)
                {
                    p.rbPlayer.velocity = new Vector2(0,0);
                }
            }
            else { p.rbPlayer.drag = p.rbPlayer.velocity.magnitude / 8f; }
        }
    }

    private bool DialogsNotOpen()
    {
        return !p.gM.Refs.gamePausePanel.activeInHierarchy && !p.gM.Refs.tutorialChoicePanel.activeInHierarchy;
    }
}
