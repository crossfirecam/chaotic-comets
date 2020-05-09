using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class UfoAllTypes : MonoBehaviour
{
    internal float timer = 0;
    private bool movingRandomly = false;

    void FixedUpdate()
    {
        timer += Time.fixedDeltaTime;
        if (!ufoTeleporting && !deathStarted)
        {
            if (!playerFound)
            {
                FindPlayer();
            }
            else
            {
                // If UFO has more than 10 health, continue chasing player
                if (alienHealth > 10f)
                {
                    if (!ufoFollower.Equals(null))
                    {
                        ufoFollower.ChaseLogicFollower();
                    }
                }
                // If alien has less than 10 health, it will run away in a single direction and attempt to teleport
                else
                {
                    if (!ufoRetreating)
                    {
                        direction = player.position - transform.position; // Direction is reversed in AlienRetreat();
                        AlienRetreat();
                    }
                }
                direction = direction.normalized;
                rb.MovePosition(rb.position + direction * alienSpeedCurrent * Time.fixedDeltaTime);
            }
        }
    }

    internal void FindPlayer()
    {
        // Choose a player to target
        float randomFloat = Random.Range(0.0f, 1f);
        if (randomFloat >= 0.5f && !gM.player1dead && !gM.player1TEMPDEAD)
        {
            player = GameObject.FindWithTag("Player").transform;
            direction = (player.position - transform.position);
        }
        else if (randomFloat <= 0.49f && !gM.player2dead && !gM.player2TEMPDEAD)
        {
            player = GameObject.FindWithTag("Player 2").transform;
            direction = (player.position - transform.position);
        }

        // Once player is found, don't shoot for 1 second, and disable random movement
        // If no player is found, then tell UFO to enable random movement
        if (player != null)
        {
            lastTimeShot = Time.time + 1f;
            playerFound = true;
            movingRandomly = false;
        }
        else
        {
            if (!movingRandomly)
            {
                direction = Random.insideUnitCircle;
                movingRandomly = true;
            }
        }

        // Fringe case - one player alive, they die, a bullet hits almost dead UFO, and it wouldn't activate shields
        if (alienHealth <= 10f && !ufoRetreating)
        { 
            AlienRetreat();
        }
        if (direction == Vector2.zero)
        {
            Debug.Log("UFO spawned when both players are dead. Randomise direction.");
            direction = Random.insideUnitCircle;
        }

        direction = direction.normalized;
        // If both players are dead, continue moving in same direction for this frame at double speed
        rb.MovePosition(rb.position + direction * (alienSpeedCurrent * 2) * Time.fixedDeltaTime);
    }

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
    void CheckScreenWrap()
    {
        if (timer > 3f)
        {
            Vector3 savedPosition = transform.position;
            gM.CheckScreenWrap(transform, 0f);

            // If UFO screenwraps, tell UFO to face the player again so it can accelerate toward them once popping out the other side
            // This isn't to happen while UFO is retreating, or else it gets stuck on the edges of the screen
            if (savedPosition != transform.position)
            {
                timer = 0;
                if (!ufoRetreating)
                {
                    direction = (player.position - transform.position);
                }

            }
        }
    }

    // Returns if the UFO is able to shoot, based on criteria
    private bool ShipAbleToShoot()
    {
        return Time.time > lastTimeShot + shootingDelay             // UFO has not shot too recently
            && playerFound && !playerTooFar                         // UFO has found a player, and they're not far away
            && !deathStarted && !ufoTeleporting && !ufoRetreating;  // UFO is not dying, teleporting, or retreating
    }
}
