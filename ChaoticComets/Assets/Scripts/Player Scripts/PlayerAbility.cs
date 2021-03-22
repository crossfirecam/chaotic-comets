using System.Collections;
using UnityEngine;

/*
 * This class concerns all actions of the Player to do with the power ability. Currently only teleports.
 */

public class PlayerAbility : MonoBehaviour
{
    [SerializeField] PlayerMain p = default;

    internal GameObject teleportIn, teleportOut;

    private void Start()
    {
        teleportIn = gameObject.transform.Find("TeleportParticlesIn").gameObject;
        teleportOut = gameObject.transform.Find("TeleportParticlesOut").gameObject;
    }

    private void Hyperspace()
    {
        // Initiate local newPosition, and pick new position
        Vector2 newPosition;
        newPosition = new Vector2(Random.Range(-7.4f, -2.6f), Random.Range(-4.0f, 1.2f));

        transform.position = newPosition;
        p.rbPlayer.velocity = Vector2.zero;
        teleportIn.SetActive(false);
        p.plrMisc.StartCoroutine(p.plrMisc.FadeShip("In"));
        StartCoroutine(nameof(TeleportOutTimer));
    }

    // When a teleportation ends (either after player uses Hyperspace ability, or when respawning)
    // Take 12 seconds total to recharge power. Ship can use power after those 12s.
    internal IEnumerator TeleportOutTimer()
    {
        teleportOut.SetActive(true);
        p.power = 0;

        // If not in tutorial, recharge power meter
        if (GameManager.i.tutorialMode == false)
        {
            for (int powerTick = 0; powerTick <= 80; powerTick++)
            {
                p.power = powerTick;
                yield return new WaitForSeconds(0.15f);
            }
            p.power = 80f;
        }
        // If in tutorial (popup 17/18), recharge power meter faster
        else if (TutorialManager.i.popUpIndex >= 17)
        {
            for (int powerTick = 0; powerTick <= 80; powerTick++)
            {
                p.power = powerTick;
                yield return new WaitForSeconds(0.03f);
            }
            p.power = 80f;
        }
        // If in tutorial (not popup 17), don't recharge power meter after death
        else
        {
            yield return new WaitForSeconds(2f);
        }
        teleportOut.SetActive(false);
    }

    internal void ResetPowerMeter()
    {
        teleportOut.SetActive(false);
        p.power = 0;
        StopCoroutine(nameof(TeleportOutTimer));
    }
}
