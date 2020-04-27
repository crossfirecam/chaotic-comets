using System.Collections;
using System.Collections.Generic;
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
        teleportIn = gameObject.transform.Find("P" + p.playerNumber + "-TeleportParticlesIn").gameObject;
        teleportOut = gameObject.transform.Find("P" + p.playerNumber + "-TeleportParticlesOut").gameObject;
    }
    private void Hyperspace()
    {
        // Initiate local newPosition, and pick new position depending on what screen the player is using (game or help screen)
        Vector2 newPosition = new Vector2(0, 0);
        if (!p.helpMenuMode)
        {
            newPosition = new Vector2(Random.Range(-9f, 9f), Random.Range(-5f, 5f));
        }
        else
        {
            newPosition = new Vector2(Random.Range(-7.4f, -2.6f), Random.Range(-4.0f, 1.2f));
        }
        transform.position = newPosition;
        p.rb.velocity = Vector2.zero;
        teleportIn.SetActive(false);
        p.playerMisc.StartCoroutine("FadeShip", "In");
        teleportOut.SetActive(true);
        StartCoroutine("PowerTimer", "Hyperspace");
    }

    // When power is used, take 12 seconds total to recharge power. Ship can use power after those 12s.
    internal IEnumerator PowerTimer(string powerType)
    {
        if (powerType == "Hyperspace")
        {
            p.power = 0;
            for (int powerTick = 0; powerTick <= 80; powerTick++)
            {
                p.power = powerTick;
                yield return new WaitForSeconds(0.15f);
            }
            teleportOut.SetActive(false);
            p.power = 80f;
            p.playerUI.powerBar.sprite = p.playerUI.powerWhenReady;
            StopCoroutine("PowerTimer");
        }
    }
}
