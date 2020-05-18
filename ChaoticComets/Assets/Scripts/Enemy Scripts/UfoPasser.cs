using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/*
 * This class is used for typical UFOs, that chase the player in a straight line and shoot in a straight line
 */

public class UfoPasser : Ufo
{
    public float currentDeviation;
    private void Update()
    {
        // Weapon systems. If criteria are met, then shoot depending on enemy type
        if (ShipAbleToShoot())
        {
            WeaponLogicPasser();
        }

        // Stabilise 3D model
        transform.rotation = Quaternion.Euler(-50, 0, 0);

        CheckUfoScreenWrap(true);
    }
    private void FixedUpdate()
    {
        timer += Time.fixedDeltaTime;
        if (!ufoTeleporting && !deathStarted)
        {
            if (!playerFound)
            {
                FindPlayerPasser();
            }

            // If UFO has more than 10 health, continue moving
            if (alienHealth > 10f)
            {
                MoveLogicPasser();
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

    private void MoveLogicPasser()
    {
        if (System.Math.Round(timer, 1) % 5f == 0f)
        {
            Debug.Log(System.Math.Round(timer, 1));
            currentDeviation = Random.Range(-1f, 1f);
        }
        direction = new Vector2(1, currentDeviation);
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
    private void FindPlayerPasser()
    {
        // Choose a player to target
        float randomFloat = Random.Range(0.0f, 1f);
        if (randomFloat >= 0.5f && !gM.player1dead && !gM.player1TEMPDEAD)
        {
            player = GameObject.FindWithTag("Player").transform;
        }
        else if (randomFloat <= 0.49f && !gM.player2dead && !gM.player2TEMPDEAD)
        {
            player = GameObject.FindWithTag("Player 2").transform;
        }

        // Once player is found, don't shoot for 1 second, and disable random movement
        // If no player is found, then tell UFO to enable random movement
        if (player != null)
        {
            lastTimeShot = Time.time + 1f;
            playerFound = true;
        }

        // Fringe case - one player alive, they die, a bullet hits almost dead UFO, and it wouldn't activate shields
        if (alienHealth <= 10f && !ufoRetreating)
        {
            AlienRetreat();
        }
    }
}
