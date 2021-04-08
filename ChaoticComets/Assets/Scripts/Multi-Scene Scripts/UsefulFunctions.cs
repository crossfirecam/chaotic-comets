using Rewired;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UsefulFunctions : MonoBehaviour
{
    /* ------------------------------------------------------------------------------------------------------------------
     * Initial Prop Movement Calculation
     * ------------------------------------------------------------------------------------------------------------------ */
    // Give random thrust to newly spawned props. Thanks to metalted and GlassesGuy on Unity forum (https://answers.unity.com/questions/1646067/)
    public static void FindThrust(Rigidbody2D rb, float maxThrust)
    {

        // Add random spin and thrust
        float minThrust = 0.7f * maxThrust;
        float angle = Random.Range(-359, 359);
        float thrust = Random.Range(minThrust, maxThrust);
        float xAxisForce = Mathf.Cos(angle * Mathf.PI / 180) * thrust;
        float yAxisForce = Mathf.Sin(angle * Mathf.PI / 180) * thrust;
        Vector2 force = new Vector2(xAxisForce, yAxisForce);
        rb.AddForce(force);
    }

    public static void FindTorque(Rigidbody2D rb, float maxSpin)
    {
        float spin = Random.Range(-maxSpin, maxSpin);
        rb.AddTorque(spin);
    }


    /* ------------------------------------------------------------------------------------------------------------------
     * Controller Detection
     * ------------------------------------------------------------------------------------------------------------------ */

    private static Controller controller;
    public static GraphicRaycaster mouseInputOnCanvas;
    
    // Check every fifth of a second if the last used controller type has changed
    public static IEnumerator CheckController()
    {
        while (true)
        {
            CheckLastUsedController();
            yield return new WaitForSecondsRealtime(0.2f);
        }
    }

    // Return which controller has last been used
    public static void CheckLastUsedController()
    {
        if (mouseInputOnCanvas == null)
        { SetupControllerCheck(); }

        controller = ReInput.controllers.GetLastActiveController();
        //print(controller.hardwareName + " " + cursorForcedStay);
        Cursor.visible = controller.hardwareName == "Mouse";
        mouseInputOnCanvas.enabled = controller.hardwareName == "Mouse";
    }

    // When a scene starts, find the Canvas, so the GraphicRaycaster can be disabled when cursor is invisible
    public static void SetupControllerCheck()
    {
        mouseInputOnCanvas = FindObjectOfType<Canvas>().GetComponent<GraphicRaycaster>();
        controller = ReInput.controllers.GetLastActiveController();
        Cursor.visible = controller.hardwareName == "Mouse";
    }

    /* ------------------------------------------------------------------------------------------------------------------
     * Other
     * ------------------------------------------------------------------------------------------------------------------ */

    public static void ResetBetweenScenesScript()
    {
        BetweenScenes.ResumingFromSave = false;
        BetweenScenes.TutorialMode = false;
        BetweenScenes.CheaterMode = false;
        BetweenScenes.PlayerShopUpgrades[0] = new int[] { 0, 0, 0, 0, 0, 0, 0, 0 };
        BetweenScenes.PlayerShopUpgrades[1] = new int[] { 0, 0, 0, 0, 0, 0, 0, 0 };
    }

    private static float fadingAlpha = 0f;
    private static bool fadingInterrupted = false;
    /// <summary>
    /// Fade the screen to or from black.<br/>
    /// If the screen is already fading FROM black, and TO black is called, then interrupt FROM. Continue TO from the current alpha level.
    /// </summary>
    /// <param name="intent">0 = Fade TO black<br/>1 = Fade FROM black</param>
    /// <param name="fadeScreenOverlay">The fading image component that covers the screen</param>
    /// <param name="secondsToFade">Time it takes for screen to fade in or out. Default is 0.4f</param>
    public static IEnumerator FadeScreenBlack(string intent, Image fadeScreenOverlay, float secondsToFade = 0.4f)
    {
        Color origColor = fadeScreenOverlay.color;
        fadeScreenOverlay.gameObject.SetActive(true);
        if (intent == "from") // Fade FROM black
        {
            fadingAlpha = 1f;
            while (fadingAlpha > 0f && !fadingInterrupted)
            {
                fadingAlpha -= 0.01f;
                fadeScreenOverlay.color = new Color(origColor.r, origColor.g, origColor.b, fadingAlpha);
                yield return new WaitForSeconds(secondsToFade / 100f);
            }
            if (!fadingInterrupted)
                fadeScreenOverlay.gameObject.SetActive(false);
        }
        else if (intent == "to") // Fade TO black
        {
            if (fadeScreenOverlay.color.a != 0f)
                fadingInterrupted = true;

            while (fadingAlpha < 1f)
            {
                fadingAlpha += 0.01f;
                fadeScreenOverlay.color = new Color(origColor.r, origColor.g, origColor.b, fadingAlpha);
                yield return new WaitForSeconds(secondsToFade / 100f);
            }
            fadingInterrupted = false;
        }
    }
}
