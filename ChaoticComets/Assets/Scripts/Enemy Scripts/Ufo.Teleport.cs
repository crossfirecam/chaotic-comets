using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract partial class Ufo : MonoBehaviour
{
    private int teleBorderOffset = 2;
    private int teleAttempts = 0;
    internal void AlienRetreat()
    {
        alienSpeedCurrent = alienSpeedBase * -3f; // Reverse direction x3 speed
        ufoRetreating = true;
        forceField.SetActive(true);
        Invoke(nameof(TeleportStart), 3f);
    }

    // If the UFO is not dying, then start the teleport sequence at the end of a level
    public void TeleportStart()
    {
        if (UfoIsInVisibleArea())
        {
            if (!deathStarted)
            {
                forceField.SetActive(false);

                ufoRetreating = false;
                ufoTeleporting = true;
                teleportEffect.SetActive(true);

                Renderer[] listOfUFOparts = GetComponentsInChildren<Renderer>();
                foreach (Renderer rend in listOfUFOparts)
                {
                    StartCoroutine(FadeOut(rend));
                }
                Invoke("TeleportEnd", 2f);
            }
        }
        else
            Invoke(nameof(TeleportStart), 1f);
    }

    // Fade the ship's material color as it teleports
    private IEnumerator FadeOut(Renderer ufoPart)
    {
        Material partMaterial = ufoPart.material;
        Color origColor = partMaterial.color;
        float speedOfFade = 0.5f;
        float alpha = 1f;

        while (alpha > 0f)
        {
            if (deathStarted) { break; }
            alpha -= speedOfFade * Time.deltaTime;
            partMaterial.color = new Color(origColor.r, origColor.g, origColor.b, alpha);
            yield return null;
        }
        // If during the while loop, death is started - then UFO will have a unique death animation
        if (deathStarted)
        {
            ufoTeleporting = false;
            teleportEffect.SetActive(false);
            speedOfFade = 2f;
            while (alpha < 1f)
            {
                alpha += speedOfFade * Time.deltaTime;
                partMaterial.color = new Color(origColor.r, origColor.g, origColor.b, alpha);
                yield return null;
            }
        }
    }

    private void TeleportEnd()
    {
        if (!deathStarted)
        {
            if (!gM.tutorialMode) {
                gM.AlienAndPowerupLogic(GameManager.PropSpawnReason.AlienRespawn);
            }
            else {
                gM.Refs.tutorialManager.GetComponent<TutorialManager>().ufoGone = true;
            }
            Destroy(gameObject);
        }
    }

    // Flicking shields on and off over 0.3 seconds
    private void FlickShieldOn()
    {
        forceField.SetActive(true);
        audioAlienSfx.clip = audClipAliexSfxShieldReflect;
        audioAlienSfx.pitch = 0.7f;
        audioAlienSfx.Play();
        if (!ufoRetreating) { StartCoroutine(ShieldFadesOn()); Invoke("FlickShieldOff", 0.3f); }
    }

    private void FlickShieldOff()
    {
        audioAlienSfx.pitch = 1f;
        forceField.SetActive(false);
    }

    // Shield fading in/out. If interrupted by the UFO retreating, the fading will skip to max shield transparency
    private IEnumerator ShieldFadesOn()
    {
        Material shieldMaterial = forceField.GetComponent<Renderer>().material;
        Color origColor = shieldMaterial.color;
        float speedOfFade = 3f;
        float alpha = 0f;

        while (alpha < 0.6f)
        {
            if (ufoRetreating) { break; }
            alpha += speedOfFade * Time.deltaTime;
            shieldMaterial.color = new Color(origColor.r, origColor.g, origColor.b, alpha);
            yield return null;
        }
        StartCoroutine(ShieldFadesOff());
    }
    private IEnumerator ShieldFadesOff()
    {
        Material shieldMaterial = forceField.GetComponent<Renderer>().material;
        Color origColor = shieldMaterial.color;
        float speedOfFade = 3f;
        float alpha = 0.6f;

        while (alpha > 0f)
        {
            if (ufoRetreating) { break; }
            alpha -= speedOfFade * Time.deltaTime;
            shieldMaterial.color = new Color(origColor.r, origColor.g, origColor.b, alpha);
            yield return null;
        }
        shieldMaterial.color = new Color(origColor.r, origColor.g, origColor.b, 0.5f); // Set to default
    }

    // The UFO can only stop retreating if within a certain area of the screen.
    // If an attempt is made 3 times, the UFO is stuck just on the edge, and will teleport anyway.
    private bool UfoIsInVisibleArea()
    {
        print("Checking if UFO is in visible area. Attempt #" + teleAttempts);
        if (transform.position.x > gM.screenLeft + teleBorderOffset && transform.position.x < gM.screenRight - teleBorderOffset)
        {
            if (transform.position.y > gM.screenBottom + teleBorderOffset && transform.position.y < gM.screenTop - teleBorderOffset)
            {
                return true;
            }
        }
        if (teleAttempts > 3)
        {
            return true;
        }
        teleAttempts++;
        return false;
    }
}
