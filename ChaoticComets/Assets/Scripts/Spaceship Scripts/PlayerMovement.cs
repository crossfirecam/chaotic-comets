using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    [SerializeField] PlayerMain p = default;

    public void ShipMovement()
    {
        // Rotate the ship
        if (p.playerInput.turnInput != 0 && p.spritePlayer.enabled)
        {
            transform.Rotate(Vector3.forward * p.playerInput.turnInput * Time.deltaTime * p.playerInput.turnThrust);
        }

        // Active thrusting (forward or braking thrust)
        // Apply force on Y axis of spaceship, multiply by thrust
        if (p.playerInput.thrustInput != 0 && p.spritePlayer.enabled && p.playerInput.isNotTeleporting)
        {
            if (!p.audioShipThrust.isPlaying) { p.audioShipThrust.Play(); }
            if (!p.playerInput.thruster1.isPlaying) { p.playerInput.thruster1.Play(); }
            if (!p.playerInput.thruster2.isPlaying) { p.playerInput.thruster2.Play(); }

            // If thrust is less than 0, then ship is braking. On hard difficulty, brake is less powerful.
            if (p.playerInput.thrustInput > 0)
            {
                if (BetweenScenesScript.Difficulty != 2)
                {
                    p.rbPlayer.drag = p.rbPlayer.velocity.magnitude / p.playerInput.brakingPower;
                }
                else
                {
                    p.rbPlayer.drag = p.rbPlayer.velocity.magnitude / p.playerInput.brakingPower / 2;
                }
            }
            // If thrust is more than 0, then ship is moving forward.
            else
            {
                p.rbPlayer.AddRelativeForce(Vector2.up * -p.playerInput.thrustInput * Time.deltaTime * p.playerInput.thrust);
                p.rbPlayer.drag = p.rbPlayer.velocity.magnitude / 10f;
            }
        }
        // Passive Drag (no thruster controls pressed)
        // Apply passive drag depending on if retro thrusters are equipped or not
        else
        {
            if (p.audioShipThrust.isPlaying) { p.audioShipThrust.Stop(); }
            if (p.playerInput.thruster1.isPlaying) { p.playerInput.thruster1.Stop(); }
            if (p.playerInput.thruster2.isPlaying) { p.playerInput.thruster2.Stop(); }
            if (p.playerPowerups.ifRetroThruster)
            {
                if (BetweenScenesScript.Difficulty != 2)
                {
                    p.rbPlayer.drag = p.rbPlayer.velocity.magnitude / 0.2f;
                }
                else
                {
                    p.rbPlayer.drag = p.rbPlayer.velocity.magnitude / 2f;
                }
            }
            else { p.rbPlayer.drag = p.rbPlayer.velocity.magnitude / 8f; }
        }
    }

    // Screen Wrapping
    public void CheckScreenWrap()
    {
        Vector2 newPosition = transform.position;
        if (transform.position.y > p.gM.screenTop) { newPosition.y = p.gM.screenBottom; }
        if (transform.position.y < p.gM.screenBottom) { newPosition.y = p.gM.screenTop; }
        if (transform.position.x > p.gM.screenRight) { newPosition.x = p.gM.screenLeft; }
        if (transform.position.x < p.gM.screenLeft) { newPosition.x = p.gM.screenRight; }
        transform.position = newPosition;
    }
}
