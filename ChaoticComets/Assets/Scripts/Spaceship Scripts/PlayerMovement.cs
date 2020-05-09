using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    [SerializeField] PlayerMain p = default;

    public void ShipMovement()
    {
        // Rotate the ship
        if (p.plrInput.turnInput != 0 && p.modelPlayer.activeInHierarchy)
        {
            transform.Rotate(Vector3.forward * p.plrInput.turnInput * Time.deltaTime * p.plrInput.turnThrust);
        }

        // Active thrusting (forward or braking thrust)
        // Apply force on Y axis of spaceship, multiply by thrust
        if (p.plrInput.thrustInput != 0 && p.modelPlayer.activeInHierarchy && p.plrInput.isNotTeleporting)
        {
            if (!p.plrUiSound.audioShipThrust.isPlaying) { p.plrUiSound.audioShipThrust.Play(); }
            if (!p.plrInput.thruster1.isPlaying) { p.plrInput.thruster1.Play(); }
            if (!p.plrInput.thruster2.isPlaying) { p.plrInput.thruster2.Play(); }

            // If thrust is less than 0, then ship is braking. On hard difficulty, brake is less powerful.
            if (p.plrInput.thrustInput > 0)
            {
                if (BetweenScenesScript.Difficulty != 2)
                {
                    p.rbPlayer.drag = p.rbPlayer.velocity.magnitude / p.plrInput.brakingPower;
                }
                else
                {
                    p.rbPlayer.drag = p.rbPlayer.velocity.magnitude / p.plrInput.brakingPower / 2;
                }
            }
            // If thrust is more than 0, then ship is moving forward.
            else
            {
                p.rbPlayer.AddRelativeForce(Vector2.up * -p.plrInput.thrustInput * Time.deltaTime * p.plrInput.thrust);
                p.rbPlayer.drag = p.rbPlayer.velocity.magnitude / 10f;
            }
        }
        // Passive Drag (no thruster controls pressed)
        // Apply passive drag depending on if retro thrusters are equipped or not
        else
        {
            if (p.plrUiSound.audioShipThrust.isPlaying) { p.plrUiSound.audioShipThrust.Stop(); }
            if (p.plrInput.thruster1.isPlaying) { p.plrInput.thruster1.Stop(); }
            if (p.plrInput.thruster2.isPlaying) { p.plrInput.thruster2.Stop(); }
            if (p.plrPowerups.ifRetroThruster)
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
