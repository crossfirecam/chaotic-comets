using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    [SerializeField] PlayerMain p = default;
    private bool autoBrakeEngaged = false;
    public ParticleSystem thruster1, thruster2, autoBrakeEffect1, autoBrakeEffect2;

    private float currentSpeed;
    // Acceleration
    internal float thrustPower = 700f, turnThrustPower = 200f;
    // Braking
    private readonly float stopSpeedThreshold = 0.8f;
    internal float manualBrakePower = 1f;
    private readonly float autoBrakeStrength = 5f;

    public void ShipMovement()
    {
        // Rotate the ship
        if (p.plrInput.turnInput != 0 && p.modelPlayer.activeInHierarchy)
        {
            transform.Rotate(Vector3.forward * -p.plrInput.turnInput * Time.deltaTime * turnThrustPower);
        }

        currentSpeed = p.rbPlayer.velocity.magnitude;
        // Active thrusting (forward or braking thrust)
        // Apply force on Y axis of spaceship, multiply by thrust
        if (p.plrInput.thrustInput != 0 && p.modelPlayer.activeInHierarchy && p.plrInput.isNotTeleporting)
        {
            if (autoBrakeEngaged)
                autoBrakeEngaged = false;

            if (!p.plrUiSound.audioShipThrust.isPlaying && DialogsNotOpen())
                p.plrUiSound.audioShipThrust.Play();

            if (!thruster1.isPlaying) { thruster1.Play(); }
            if (!thruster2.isPlaying) { thruster2.Play(); }

            // If thrust is less than 0, then ship is braking.
            if (p.plrInput.thrustInput < 0)
            {
                if (currentSpeed > stopSpeedThreshold)
                {
                    // If hard, manual brake is less effective
                    p.rbPlayer.drag = currentSpeed * (manualBrakePower / currentSpeed);
                    if (BetweenScenes.Difficulty == 2)
                    { 
                        p.rbPlayer.drag /= 1.5f;
                    }
                }
                // If ship is slow enough, stop it
                else
                {
                    p.rbPlayer.velocity = new Vector2(0, 0);
                }
            }

            // If thrust is more than 0, then ship is moving forward.
            else
            {
                p.rbPlayer.AddRelativeForce(Vector2.up * p.plrInput.thrustInput * Time.deltaTime * thrustPower);
                if (currentSpeed != 0) { p.rbPlayer.drag = currentSpeed * (1f / currentSpeed); }
            }
        }
        // Passive Drag (no thruster controls pressed)
        // Apply passive drag depending on if Auto-Brake equipped or not
        else
        {
            if (p.plrUiSound.audioShipThrust.isPlaying) { p.plrUiSound.audioShipThrust.Stop(); }
            if (thruster1.isPlaying) { thruster1.Stop(); }
            if (thruster2.isPlaying) { thruster2.Stop(); }
            if (p.plrPowerups.ifAutoBrake && currentSpeed != 0)
            {
                if (!autoBrakeEngaged && currentSpeed > 3)
                {
                    p.plrUiSound.audioShipAutoBrake.Play();
                    autoBrakeEngaged = true;
                    autoBrakeEffect1.Play();
                    autoBrakeEffect2.Play();
                }

                // If hard, auto-brake is 5x less effective. Stops with about a ship-length of clearance instead of instantly.
                p.rbPlayer.drag = currentSpeed * autoBrakeStrength;
                if (BetweenScenes.Difficulty == 2)
                {
                    p.rbPlayer.drag /= 5f;
                }

                // If ship is slow enough, stop it
                if (currentSpeed < stopSpeedThreshold)
                {
                    p.rbPlayer.velocity = new Vector2(0,0);
                }
            }
            else { p.rbPlayer.drag = currentSpeed / 8f; }
        }
    }

    private bool DialogsNotOpen()
    {
        return !UiManager.i.GameIsPaused() && !UiManager.i.GameIsOnTutorialScreen();
    }
}
