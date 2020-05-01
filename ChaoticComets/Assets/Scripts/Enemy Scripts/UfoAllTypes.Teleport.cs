using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class UfoAllTypes : MonoBehaviour
{
    internal void AlienRetreat()
    {
        alienSpeed = alienSpeed * 3f;
        ufoRetreating = true;
        forceField.SetActive(true);
        Invoke("TeleportStart", 3f);
    }

    // If the UFO is not dying, then start the teleport sequence at the end of a level
    public void TeleportStart()
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

    // Destroy gameobject
    private void TeleportEnd()
    {
        if (!deathStarted)
        {
            gM.AlienAndPowerupLogic(GameManager.PropSpawnReason.AlienRespawn);
            Destroy(gameObject);
        }
    }
}
