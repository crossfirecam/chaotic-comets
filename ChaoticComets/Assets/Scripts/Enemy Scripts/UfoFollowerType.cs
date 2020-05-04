using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * This class is used for typical UFOs, that chase the player in a straight line and shoot in a straight line
 */

public class UfoFollowerType : MonoBehaviour
{
    [SerializeField] internal UfoAllTypes u = default;

    internal void ChaseLogicFollower()
    {
        if (u.IsPlayerTooClose())
        {
            u.direction = (u.player.position + transform.position);
        }
        else
        {
            if (u.IsPlayerTooFarX() == true || u.IsPlayerTooFarY() == true)
            {
                // Continue straight forward at a higher speed until player is close enough again.
                if (!u.playerTooFar)
                {
                    u.alienSpeedCurrent *= 3f;
                    u.playerTooFar = true;
                }
            }
            else
            {
                // Constantly change direction to face player if within range.
                u.direction = (u.player.position - transform.position);
                if (u.playerTooFar)
                {
                    u.alienSpeedCurrent = u.alienSpeedBase;
                    u.playerTooFar = false;
                }
            }
        }
    }

    internal void WeaponLogicFollower()
    {

        Vector2 towardPlayer = (u.player.position - transform.position);
        float angle = Mathf.Atan2(towardPlayer.y, towardPlayer.x) * Mathf.Rad2Deg - 90f;
        Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);

        Vector3 bulletPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z - 4);
        GameObject newBullet = Instantiate(u.bullet, bulletPosition, q);
        newBullet.GetComponent<Rigidbody2D>().AddRelativeForce(new Vector2(0f, u.bulletSpeed));
        Destroy(newBullet, 2f);

        u.lastTimeShot = Time.time;
    }
}
