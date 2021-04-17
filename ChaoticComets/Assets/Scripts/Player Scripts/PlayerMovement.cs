using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    [SerializeField] PlayerMain p = default;
    private bool autoBrakeEngaged = false;
    [SerializeField] private ParticleSystem thruster1, thruster2, autoBrakeEffect1, autoBrakeEffect2;

    private float currentSpeed;
    // Thrusting
    internal float thrustPower = 700f, turnThrustPower = 200f;
    // Braking
    private readonly float stopSpeedThreshold = 0.8f;
    internal float manualBrakePower = 1f;
    private readonly float autoBrakeStrength = 5f;

    public void ShipMovement()
    {
        // Rotate the ship
        if (p.plrInput.turnInput != 0 && p.modelPlayer.activeInHierarchy)
            transform.Rotate(Vector3.forward * -p.plrInput.turnInput * Time.deltaTime * turnThrustPower);

        currentSpeed = p.rbPlayer.velocity.magnitude;

        // Active thrusting (forward or braking thrust)
        if (p.plrInput.thrustInput != 0 && p.modelPlayer.activeInHierarchy && p.plrInput.isNotTeleporting)
        {
            if (autoBrakeEngaged)
                autoBrakeEngaged = false;
            ShipThrusterAudio("play");

            if (p.plrInput.thrustInput < 0)
            {
                ShipBraking();
            }
        else
            ShipThrusting();
        }
        // Passive Drag (no thruster controls pressed)
        else
        {
            ShipDrifting();
        }
    }

    private void ShipThrusting()
    {
        // Apply force on Y axis of spaceship, multiply by thrust
        p.rbPlayer.AddRelativeForce(Vector2.up * p.plrInput.thrustInput * Time.deltaTime * thrustPower);
        p.plrUiSound.audioShipThrust.pitch = .8f + (currentSpeed / 13f / 1.5f); // Pitch base is .8, add on a normalized currentSpeed value that's reduced by 50%
        if (currentSpeed != 0)
            p.rbPlayer.drag = currentSpeed * (1f / currentSpeed);
    }

    private void ShipBraking()
    {
        // If ship is braking, but not slow enough to stop, slow it down. In hard mode, the manual brake is weaker.
        if (currentSpeed > stopSpeedThreshold)
        {
            p.rbPlayer.drag = currentSpeed * (manualBrakePower / currentSpeed);

            if (BetweenScenes.Difficulty >= 2)
                p.rbPlayer.drag /= 1.5f;
        }
        // If ship is slow enough, stop it.
        else
            p.rbPlayer.velocity = new Vector2(0, 0);
    }

    private void ShipDrifting()
    {
        ShipThrusterAudio("stop");

        if (p.plrPowerups.ifAutoBrake && currentSpeed != 0)
            ShipAutoBraking();
        else
            p.rbPlayer.drag = currentSpeed / 8f;
    }

    private void ShipAutoBraking()
    {
        if (!autoBrakeEngaged && currentSpeed > 3)
        {
            autoBrakeEngaged = true;
            p.plrUiSound.audioShipAutoBrake.Play();
            autoBrakeEffect1.Play(); autoBrakeEffect2.Play();
        }

        // Activate Auto-Brake
        p.rbPlayer.drag = currentSpeed * autoBrakeStrength;

        // If ship is slow enough, stop it
        if (currentSpeed < stopSpeedThreshold)
            p.rbPlayer.velocity = new Vector2(0, 0);
    }

    private void ShipThrusterAudio(string status)
    {
        if (status == "play")
        {
            if (!p.plrUiSound.audioShipThrust.isPlaying && DialogsNotOpen()) { p.plrUiSound.audioShipThrust.Play(); }
            if (!thruster1.isPlaying) { thruster1.Play(); }
            if (!thruster2.isPlaying) { thruster2.Play(); }
        }
        else if (status == "stop")
        {
            if (p.plrUiSound.audioShipThrust.isPlaying) { p.plrUiSound.audioShipThrust.Stop(); }
            if (thruster1.isPlaying) { thruster1.Stop(); }
            if (thruster2.isPlaying) { thruster2.Stop(); }
        }
    }


    private bool DialogsNotOpen()
    {
        return !UiManager.i.GameIsPaused() && !UiManager.i.GameIsOnTutorialScreen();
    }
}
