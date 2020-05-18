using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract partial class Ufo : MonoBehaviour
{
    internal float timer = 0;

    internal bool IsPlayerTooClose(float threshold)
    {
        return Vector2.Distance(player.position, transform.position) < threshold;
    }
    internal bool IsPlayerTooFarX()
    {
        return Mathf.Abs(player.position.x - transform.position.x) > 10f;
    }
    internal bool IsPlayerTooFarY()
    {
        return Mathf.Abs(player.position.y - transform.position.y) > 6f;
    }


    // Screen Wrapping. UFO does not screen wrap when in the first 3 seconds of spawning onto level, or crossing a border
    internal void CheckUfoScreenWrap(bool doesDespawnAtEdge = false)
    {
        if (timer > 2f)
        {
            Vector3 savedPosition = transform.position;
            gM.CheckScreenWrap(transform);

            if (savedPosition != transform.position)
            {
                if (!doesDespawnAtEdge) { timer = 0; }

                // If UFO is Passer, kill the UFO when it reaches the edge, unless retreating
                if (doesDespawnAtEdge && savedPosition.x > gM.screenRight && !ufoRetreating)
                {
                    DeathRoutine();
                }
                // If UFO screenwraps, tell UFO to face the player again so it can accelerate toward them once popping out the other side
                // This isn't to happen while UFO is retreating, or else it gets stuck on the edges of the screen
                else if (!ufoRetreating)
                {
                    direction = (player.position - transform.position);
                }

            }
        }
    }

    // Returns if the UFO is able to shoot, based on criteria
    internal bool ShipAbleToShoot()
    {
        return Time.time > lastTimeShot + shootingDelay             // UFO has not shot too recently
            && playerFound && !playerTooFar                         // UFO has found a player, and they're not far away
            && !deathStarted && !ufoTeleporting && !ufoRetreating;  // UFO is not dying, teleporting, or retreating
    }

    // If a player the UFO is following has died, reset variables. So it chases after another player
    public void PlayerDied()
    {
        playerFound = false;
        player = null;
    }
}
