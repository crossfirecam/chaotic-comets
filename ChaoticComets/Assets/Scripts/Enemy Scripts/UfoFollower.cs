using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * This class is used for typical UFOs, that chase the player in a straight line and shoot in a straight line
 */

public class UfoFollower : Ufo
{
    private bool movingRandomly = false;

    private void Update()
    {
        // Weapon systems. If criteria are met, then shoot depending on enemy type
        if (ShipAbleToShoot())
        {
            WeaponLogicFollower();
        }

        // Stabilise 3D model
        transform.rotation = Quaternion.Euler(-50, 0, 0);

        CheckUfoScreenWrap();
    }

    private void FixedUpdate()
    {
        timer += Time.fixedDeltaTime;
        if (!ufoTeleporting && !deathStarted)
        {
            if (!playerFound)
            {
                FindPlayerFollower();
            }
            else
            {
                // If UFO has more than 10 health, continue chasing player
                if (alienHealth > 10f)
                {
                    ChaseLogicFollower();
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

    internal void ChaseLogicFollower()
    {
        if (IsPlayerTooClose(2f))
        {
            direction = Vector2.zero;
        }
        else
        {
            if (IsPlayerTooFarX() == true || IsPlayerTooFarY() == true)
            {
                // Continue straight forward at a higher speed until player is close enough again.
                if (!playerTooFar)
                {
                    alienSpeedCurrent *= 3f;
                    playerTooFar = true;
                }
            }
            else
            {
                // Constantly change direction to face player if within range.
                direction = (player.position - transform.position);
                if (playerTooFar)
                {
                    alienSpeedCurrent = alienSpeedBase;
                    playerTooFar = false;
                }
            }
        }
    }

    internal void WeaponLogicFollower()
    {

        Vector2 towardPlayer = (player.position - transform.position);
        float angle = Mathf.Atan2(towardPlayer.y, towardPlayer.x) * Mathf.Rad2Deg - 90f;
        Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);

        Vector3 bulletPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z - 4);
        GameObject newBullet = Instantiate(bullet, bulletPosition, q);
        newBullet.GetComponent<Rigidbody2D>().AddRelativeForce(new Vector2(0f, bulletSpeed));
        Destroy(newBullet, 2f);

        lastTimeShot = Time.time;
    }

    private void FindPlayerFollower()
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
}
