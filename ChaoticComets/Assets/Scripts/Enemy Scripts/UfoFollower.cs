using UnityEngine;

/*
 * This class is used for typical UFOs, that chase the player in a straight line and shoot in a straight line
 */

public class UfoFollower : Ufo
{
    private bool movingRandomly = false;
    private Vector2 directionBeforeStopping = Vector2.zero;
    private new void Update()
    {
        base.Update();

        CheckUfoScreenWrap();
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
        if (IsPlayerTooClose(6f))
        {
            // Stores the previously used direction vector.
            // Prevents a bug where a player that very slowly leaves the screen can leave the UFO standing still.
            if (directionBeforeStopping == Vector2.zero)
            {
                directionBeforeStopping = direction;
            }
            direction = Vector2.zero;
        }
        else
        {
            if (IsPlayerTooFarX() == true || IsPlayerTooFarY() == true)
            {
                // Continue straight forward at a higher speed until player is close enough again.
                if (!playerTooFar)
                {
                    alienSpeedCurrent = alienSpeedBase * 1.5f;
                    playerTooFar = true;
                }

                // If player is too far, but direction is 0, then set direction to previously stored vector.
                // Prevents a bug where a player that very slowly leaves the screen can leave the UFO standing still.
                if (direction == Vector2.zero)
                {
                    direction = directionBeforeStopping;
                    directionBeforeStopping = Vector2.zero;
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

    internal override void ShootBullet()
    {
        if (!(GameManager.i.tutorialMode && TutorialManager.i.ufoFollowerDocile))
        {
            base.ShootBullet();
        }
    }

    private int lifeCountWhenUfoSpawns = -1;
    internal override void FindPlayer()
    {
        base.FindPlayer();

        // Store an int that represents how many Ships players have before engaging a UFO-Follower.
        // Whether a player dies via the UFO or a hazard, compare the old and new Ship count.
        // If the count has reduced, force UFO-Follower to teleport away.
        if (lifeCountWhenUfoSpawns == -1)
            lifeCountWhenUfoSpawns = GameManager.i.playerLives;
        if (GameManager.i.playerLives < lifeCountWhenUfoSpawns)
            TeleportStart(true);

        // Once player is found, disable random movement
        // If no player is found, then tell UFO to enable random movement
        if (player != null)
        {
            movingRandomly = false;
            direction = player.position - transform.position;
        }
        else
        {
            if (!movingRandomly)
            {
                direction = Random.insideUnitCircle;
                movingRandomly = true;
            }
        }

        // Edge case. If direction = zero, then UFO came into existence when both players are dead.
        if (direction == Vector2.zero)
        {
            print("UFO spawned when both players are dead. Randomise direction.");
            direction = Random.insideUnitCircle;
        }

        direction = direction.normalized;
        // If both players are dead, continue moving in same direction for this frame at double speed
        rb.MovePosition(rb.position + direction * (alienSpeedBase * 2) * Time.fixedDeltaTime);
    }
}
