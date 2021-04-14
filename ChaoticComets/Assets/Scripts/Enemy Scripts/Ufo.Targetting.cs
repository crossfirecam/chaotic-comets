using UnityEngine;
using static Constants;

public abstract partial class Ufo : MonoBehaviour
{
    [Header("Weapon System Variables")]
    internal float bulletRange = 400f;
    internal float shootingDelay = 1.5f;
    internal float bulletForce = 250f;
    internal float bulletExpireTime;
    internal float bulletDeviationLimit = 16f;

    internal float lastTimeShot = 0f; // Keeps track of when UFO last fired a bullet

    [Header("Targetting System Variables")]
    internal float timer = 0;
    internal Transform player;
    internal bool playerFound = false, playerTooFar = false;

    internal virtual void ShootBullet()
    {
        Vector2 towardPlayer = player.position - transform.position;
        float angle = Mathf.Atan2(towardPlayer.y, towardPlayer.x) * Mathf.Rad2Deg - 90f;
        float slightBulletDeviation = Random.Range(-bulletDeviationLimit, bulletDeviationLimit);
        angle += slightBulletDeviation;
        Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);

        Vector3 bulletPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z - 4);
        GameObject newBullet = Instantiate(bullet, bulletPosition, q);
        newBullet.GetComponent<Rigidbody2D>().AddRelativeForce(new Vector2(0f, bulletForce));
        newBullet.GetComponent<BulletBehaviour>().FizzleOutBullet(bulletExpireTime);

        lastTimeShot = Time.time;
    }

    internal virtual void FindPlayer()
    {
        // Find both players if they're present
        Transform tempPlayer1 = null, tempPlayer2 = null;
        if (!GameManager.i.player1dead && !GameManager.i.player1TEMPDEAD)
            tempPlayer1 = GameObject.FindWithTag(Tag_Player1).transform;
        if (!GameManager.i.player2dead && !GameManager.i.player2TEMPDEAD)
            tempPlayer2 = GameObject.FindWithTag(Tag_Player2).transform;

        // Assign target based on if only one player is present, or select the closest if both are alive
        if (tempPlayer1 != null && tempPlayer2 == null)
            player = tempPlayer1;
        else if (tempPlayer1 == null && tempPlayer2 != null)
            player = tempPlayer2;
        else if (tempPlayer1 != null && tempPlayer2 != null)
        {
            if (Vector2.Distance(tempPlayer1.position, transform.position) < Vector2.Distance(tempPlayer2.position, transform.position))
                player = tempPlayer1;
            else
                player = tempPlayer2;
        }
        
        // Once player is found, don't shoot for 1 second
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






    // If a player the UFO is following has died, reset variables. So it chases after another player
    public virtual void PlayerDied()
    {
        playerFound = false;
        player = null;
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
        return Mathf.Abs(player.position.y - transform.position.y) > 7f;
    }


    // Screen Wrapping. UFO does not screen wrap when in the first 2 seconds of spawning onto level, or crossing a border
    internal void CheckUfoScreenWrap(bool doesDespawnAtEdge = false)
    {
        if (timer > 2f)
        {
            Vector3 savedPosition = transform.position;
            GameManager.i.CheckScreenWrap(transform);

            if (savedPosition != transform.position)
            {
                if (!doesDespawnAtEdge) { timer = 0; }

                // If UFO is Passer, kill the UFO when it reaches the edge, unless retreating
                if (doesDespawnAtEdge && !ufoRetreating)
                {
                    if (savedPosition.x > GameManager.i.screenRight || savedPosition.x < GameManager.i.screenLeft)
                    {
                        DeathRoutine();
                        if (GameManager.i.tutorialMode)
                            GameManager.i.RespawnPropForTutorial("ufo-passer");
                    }
                }
                // If UFO screenwraps, tell UFO to face the player again so it can accelerate toward them once popping out the other side
                // This isn't to happen while UFO is retreating, or else it gets stuck on the edges of the screen
                else if (!ufoRetreating)
                {
                    if (player != null) 
                    {
                        direction = player.position - transform.position;
                    }
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
}
