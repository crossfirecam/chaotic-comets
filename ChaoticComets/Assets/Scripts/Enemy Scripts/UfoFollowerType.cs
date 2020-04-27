using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * This class is used for typical UFOs, that chase the player in a straight line and shoot in a straight line
 */

public class UfoFollowerType : MonoBehaviour
{
    [SerializeField] internal UfoAllTypes u = default;

    void FixedUpdate()
    {
        if (!u.ufoTeleporting && !u.deathStarted)
        {
            // If a player is not found, find one
            if (!u.playerFound)
            {
                float randomFloat = Random.Range(0.0f, 1f);
                if (randomFloat >= 0.5f && !u.gM.player1dead && !u.gM.player1TEMPDEAD)
                {
                    u.player = GameObject.FindWithTag("Player").transform;
                }
                else if (randomFloat <= 0.49f && !u.gM.player2dead && !u.gM.player2TEMPDEAD)
                {
                    u.player = GameObject.FindWithTag("Player 2").transform;
                }
                if (u.player != null)
                {
                    u.lastTimeShot = Time.time + 1f; // Once player is found, don't shoot for 1 second
                    u.playerFound = true;
                }
                if (u.alienHealth <= 10f && !u.ufoRetreating)
                { // Fringe case - one player alive, they die, a bullet hits almost dead UFO, and it wouldn't activate shields
                    u.AlienRetreat();
                }
                if (u.direction == Vector2.zero)
                { //Fringe case - UFO spawns while both players are dead. Randomise direction faced.
                    u.direction = Random.insideUnitCircle.normalized;
                }

                // Continue moving in same direction for this frame, if both players are dead
                u.rb.MovePosition(u.rb.position + u.direction * u.alienSpeed * Time.fixedDeltaTime);
            }

            // If a player IS found, then...
            else
            {
                // If UFO has more than 10 health, continue chasing player
                if (u.alienHealth > 10f)
                {
                    u.direction = (u.player.position - transform.position).normalized;
                }
                // If alien has less than 10 health, it will run away in a single direction and attempt to teleport
                else
                {
                    if (!u.ufoRetreating)
                    {
                        u.direction = (u.player.position + transform.position).normalized;
                        u.AlienRetreat();
                    }
                }
                u.rb.MovePosition(u.rb.position + u.direction * u.alienSpeed * Time.fixedDeltaTime);
            }
        }
    }
}
