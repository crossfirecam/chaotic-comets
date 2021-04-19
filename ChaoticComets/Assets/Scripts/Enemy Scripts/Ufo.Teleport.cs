using System.Collections;
using UnityEngine;

public abstract partial class Ufo : MonoBehaviour
{
    [Header("Teleport System Variables")]
    public float retreatSpeedMultiplier;
    private readonly int teleBorderOffset = 2;
    private int teleAttempts = 0;

    internal void AlienRetreat()
    {
        if (!ufoRetreating)
        {
            // Face the player (or random direction if player is null) but then reverse direction.
            if (player != null)
                direction = player.position - transform.position;
            else
                direction = Random.insideUnitCircle;

            alienSpeedCurrent = alienSpeedBase * retreatSpeedMultiplier;

            ufoRetreating = true;
            StartCoroutine(nameof(PanicScanningNoise));
            forceField.SetActive(true);
            Invoke(nameof(TeleportStartFromInvoke), 3f);
        }
    }

    // A bodge to allow Unity to invoke TeleportStart().
    // Even though the parameter on the function is entirely optional, Unity still insists on not allowing Invoke() to call it, since it can't handle parameters at all.
    private void TeleportStartFromInvoke()
    {
        TeleportStart();
    }

    // If the UFO is in a visible area of the screen and not dying, then start the teleport sequence.
    // Caused either by the UFO getting to low health, tutorial popup 12, or because level is ending.
    public void TeleportStart(bool forceLeave = false)
    {
        if (UfoIsInVisibleArea() ||
            (GameManager.i.tutorialMode && alienHealth == 70) || // Exception for Tutorial popup 12, the Red UFO teleports immediately
            forceLeave == true)             // Exception for end of level, UFO will teleport immediately to avoid hurting player during ending sequence
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
                Invoke(nameof(TeleportEnd), 2f);
            }
        }
        else
        {
            Invoke(nameof(TeleportStartFromInvoke), 1f);
        }
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
            if (GameManager.i.tutorialMode)
            {
                TutorialManager.i.ufoGone = true;
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
        if (!ufoRetreating) { StartCoroutine(ShieldFadesOn()); Invoke(nameof(FlickShieldOff), 0.3f); }
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

    private const float TimeToPanicSound = 1f, VolumeChange = 0.1f, PitchChange = 0.15f;
    private IEnumerator PanicScanningNoise()
    {
        float panicPitch = audioAlienHum.pitch + PitchChange;
        while (audioAlienHum.pitch < panicPitch)
        {
            audioAlienHum.volume += VolumeChange / 20f;
            audioAlienHum.pitch += PitchChange / 20f;
            yield return new WaitForSeconds(TimeToPanicSound / 20f);
        }
    }

    private const float TimeToPerishSound = 3f, EndPitch = 0.2f;
    private IEnumerator PerishScanningNoise()
    {
        float perishPitch = audioAlienHum.pitch - EndPitch;
        while (audioAlienHum.pitch > EndPitch)
        {
            audioAlienHum.pitch -= perishPitch / 20f;
            yield return new WaitForSeconds(TimeToPerishSound / 20f);
        }
    }

    /// <summary>
    /// The UFO can only stop retreating if within a certain area of the screen.<br/>
    /// If an attempt is made 3 times, the UFO is stuck just on the edge, and will teleport anyway.
    /// </summary>
    private bool UfoIsInVisibleArea()
    {
        print("Checking if UFO is in visible area. Attempt #" + teleAttempts);
        if (transform.position.x > GameManager.i.screenLeft + teleBorderOffset && transform.position.x < GameManager.i.screenRight - teleBorderOffset)
        {
            if (transform.position.y > GameManager.i.screenBottom + teleBorderOffset && transform.position.y < GameManager.i.screenTop - teleBorderOffset)
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
