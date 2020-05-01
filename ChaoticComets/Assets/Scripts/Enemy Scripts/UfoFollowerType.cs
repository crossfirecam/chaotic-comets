using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * This class is used for typical UFOs, that chase the player in a straight line and shoot in a straight line
 */

public class UfoFollowerType : MonoBehaviour
{
    [SerializeField] internal UfoAllTypes u = default;
    public bool playerCrossedScreen;
    float timer = 0;
    public bool movingThruX = false, movingThruY = false;
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
                timer += Time.deltaTime;
                // If UFO has more than 10 health, continue chasing player
                if (u.alienHealth > 10f)
                {
                    if (IsPlayerTooClose())
                    {
                        u.direction = (u.player.position + transform.position);
                    }
                    else
                    {
                        u.direction = (u.player.position - transform.position);

                        if (IsPlayerTooFarX() == true && timer > 4.0f)
                        {
                            timer = 0f;
                            movingThruX = true;
                            Debug.Log("Too far X");
                        }
                        if (IsPlayerTooFarY() == true && timer > 4.0f)
                        {
                            timer = 0f;
                            movingThruY = true;
                            Debug.Log("Too far Y");
                        }
                        if (movingThruX)
                        {
                            u.direction = (u.player.position + transform.position);
                        }
                        else if (movingThruY)
                        {
                            u.direction = (u.player.position + transform.position);
                        }
                    }
                }
                // If alien has less than 10 health, it will run away in a single direction and attempt to teleport
                else
                {
                    if (!u.ufoRetreating)
                    {
                        u.direction = (u.player.position + transform.position);
                        u.AlienRetreat();
                    }
                }
                u.direction = u.direction.normalized;
                u.rb.MovePosition(u.rb.position + u.direction * u.alienSpeed * Time.fixedDeltaTime);
            }
        }
    }

    private bool IsPlayerTooClose()
    {
        return Vector2.Distance(u.player.position, transform.position) < 3f;
    }
    private bool IsPlayerTooFarX()
    {
        return Mathf.Abs(u.player.position.x - transform.position.x) > 14f;
    }
    private bool IsPlayerTooFarY()
    {
        return Mathf.Abs(u.player.position.y - transform.position.y) > 10f;
    }
}
