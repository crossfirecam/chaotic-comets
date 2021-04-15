using System.Collections;
using UnityEngine;

/*
 * This class concerns all actions of the Player to do with the power ability. Currently only teleports.
 */

public class PlayerAbility : MonoBehaviour
{
    [SerializeField] PlayerMain p = default;

    internal GameObject teleportIn, teleportOut, teleportReadyIndicator;

    private void Awake()
    {
        teleportIn = gameObject.transform.Find("TeleportParticlesIn").gameObject;
        teleportOut = gameObject.transform.Find("TeleportParticlesOut").gameObject;
        teleportReadyIndicator = gameObject.transform.Find("TeleportReadyParticles").gameObject;
    }

    private const float maxPowerMeter = 80f,
                        powerDepleteTime = 1f, powerChargeTimeShort = 4f;

    public float powerChargeTime = 16f;

    private IEnumerator RechargePowerMeter()
    {
        p.power = 0f;
        float delay = powerChargeTime / maxPowerMeter;

        if (GameManager.i.tutorialMode)
        {
            // If in tutorial (popup 17/18), recharge power meter faster
            if (TutorialManager.i.popUpIndex >= 17)
                delay = powerChargeTimeShort / maxPowerMeter;

            // If in tutorial (not popup 17), don't recharge power meter after death
            else
                yield break;
        }
        for (int powerTick = 0; powerTick <= maxPowerMeter; powerTick++)
        {
            p.power = powerTick;
            yield return new WaitForSeconds(delay);
        }
        p.power = 80f;
        StartCoroutine(nameof(TeleportReadyEffect));
    }

    private IEnumerator DepletePowerMeter()
    {
        p.power = 80f;
        float delay = powerDepleteTime / maxPowerMeter;
        for (int powerTick = (int)maxPowerMeter; powerTick >= 0; powerTick--)
        {
            p.power = powerTick;
            yield return new WaitForSeconds(delay);
        }
        p.power = 0f;
    }

    /* ------------------------------------------------------------------------------------------------------------------
     * Hyperspace Power
     * ------------------------------------------------------------------------------------------------------------------ */
    internal void HyperspaceStart()
    {
        teleportIn.SetActive(true);
        p.plrMisc.StartCoroutine(p.plrMisc.FadeShip("Out"));
        StartCoroutine(nameof(DepletePowerMeter));
        Invoke(nameof(HyperspaceEnd), 2f);
    }

    private void HyperspaceEnd()
    {
        // Initiate local newPosition, and pick new position
        Vector2 newPosition;
        newPosition = new Vector2(Random.Range(-12f, 12f), Random.Range(-6f, 6f));

        transform.position = newPosition;
        p.rbPlayer.velocity = Vector2.zero;
        teleportIn.SetActive(false);
        p.plrMisc.StartCoroutine(p.plrMisc.FadeShip("In"));
        StartCoroutine(nameof(TeleportOutEffect));
        StartCoroutine(nameof(RechargePowerMeter));
    }



    // When a teleportation ends (either after player uses Hyperspace ability, or when respawning)
    private IEnumerator TeleportOutEffect()
    {
        teleportOut.SetActive(true);
        yield return new WaitForSeconds(3);
        teleportOut.SetActive(false);
    }

    // When player's teleportation ability is ready, show an effect then disable it
    private IEnumerator TeleportReadyEffect()
    {
        teleportReadyIndicator.SetActive(true);
        yield return new WaitForSeconds(1);
        teleportReadyIndicator.SetActive(false);
    }

    internal void ResetPowerMeter()
    {
        teleportOut.SetActive(false);
        p.power = 0;
        StopCoroutine(nameof(TeleportOutEffect));
    }
}
