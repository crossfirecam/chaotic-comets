using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * This class is used for typical UFOs, that chase the player in a straight line and shoot in a straight line
 */

public class UfoPasser : Ufo
{
    private void Update()
    {
        // Weapon systems. If criteria are met, then shoot depending on enemy type
        if (ShipAbleToShoot())
        {
            WeaponLogicPasser();
        }

        // Stabilise 3D model
        transform.rotation = Quaternion.Euler(-50, 0, 0);

        CheckScreenWrap();
    }
    private void FixedUpdate()
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
                    ChaseLogicPasser();
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

    private void ChaseLogicPasser()
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

    internal void WeaponLogicPasser()
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
}
