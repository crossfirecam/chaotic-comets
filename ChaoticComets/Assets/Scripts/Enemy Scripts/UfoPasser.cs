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

    // UFO-Passer will move left to right, deviating with up/down/straight on Y axis
    private void MoveLogicPasser()
    {
        if (System.Math.Round(timer, 1) % 1f == 0f)
        {
            int whileTimer = 0;
            while(whileTimer < 20)
            {
                whileTimer += 1;
                float rand = Random.Range(0f, 3f);
                if (rand < 1f && PasserMovementIsValid(PasserMove.Up))
                {
                    currentDeviation = Random.Range(0.6f, 1f);
                    break;
                }
                else if (rand > 1f && rand < 2f && PasserMovementIsValid(PasserMove.Straight))
                {
                    currentDeviation = 0f;
                    break;
                }
                else if (rand > 2f && rand < 3f && PasserMovementIsValid(PasserMove.Down))
                {
                    currentDeviation = Random.Range(-1f, -0.6f);
                    break;
                }
            }
            if (whileTimer >= 20)
            {
                print("UFO Passer exceeded 20 while loops");
                currentDeviation = 0f;
            }
        }
        direction = new Vector2(1, currentDeviation);
    }
    //
    private enum PasserMove { Up, Down, Straight };
    private bool PasserMovementIsValid(PasserMove attemptedMoveType)
    {
        switch (attemptedMoveType)
        {
            case PasserMove.Up:
                if (transform.position.y < GameManager.i.screenTop - 3f)
                {
                    return true;
                }
                break;
            case PasserMove.Straight:
                if (transform.position.y < GameManager.i.screenTop - 0.5f && transform.position.y > GameManager.i.screenBottom + 0.5f)
                {
                    return true;
                }
                break;
            case PasserMove.Down:
                if (transform.position.y > GameManager.i.screenBottom + 3f)
                {
                    return true;
                }
                break;
        }
        return false;
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
        if (randomFloat >= 0.5f && !GameManager.i.player1dead && !GameManager.i.player1TEMPDEAD)
        {
            player = GameObject.FindWithTag("Player").transform;
        }
        else if (randomFloat <= 0.49f && !GameManager.i.player2dead && !GameManager.i.player2TEMPDEAD)
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
