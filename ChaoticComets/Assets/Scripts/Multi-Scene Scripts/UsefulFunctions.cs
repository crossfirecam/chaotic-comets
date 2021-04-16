using Rewired;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UsefulFunctions : MonoBehaviour
{
    /* ------------------------------------------------------------------------------------------------------------------
     * Initial Prop Movement Calculation
     * ------------------------------------------------------------------------------------------------------------------ */

    /// <summary>
    /// Give random thrust to newly spawned props.
    /// </summary>
    /// <param name="rb">Spawned object's rigidbody</param>
    /// <param name="maxThrust">The maximum thrust a spawned object can be given</param>
    // Thanks to metalted and GlassesGuy on Unity forum (https://answers.unity.com/questions/1646067/)
    public static void FindThrust(Rigidbody2D rb, float maxThrust)
    {
        float minThrust = 0.7f * maxThrust;
        float angle = Random.Range(-359, 359);
        float thrust = Random.Range(minThrust, maxThrust);
        float xAxisForce = Mathf.Cos(angle * Mathf.PI / 180) * thrust;
        float yAxisForce = Mathf.Sin(angle * Mathf.PI / 180) * thrust;
        Vector2 force = new Vector2(xAxisForce, yAxisForce);
        rb.AddForce(force);
    }

    /// <summary>
    /// Give random spin to newly spawned props. This spin does not affect movement calulcations.
    /// </summary>
    /// <param name="rb">Spawned object's rigidbody</param>
    /// <param name="maxSpin">The maximum spin a spawned object can be given</param>
    public static void FindTorque(Rigidbody2D rb, float maxSpin)
    {
        float spin = Random.Range(-maxSpin, maxSpin);
        rb.AddTorque(spin);
    }




    /* ------------------------------------------------------------------------------------------------------------------
     * Enable cursor if mouse is moved, disable cursor if another controller is used.
     * ------------------------------------------------------------------------------------------------------------------ */

    private static Controller controller;
    public static GraphicRaycaster mouseInputOnCanvas;

    /// <summary>
    /// Check every fifth of a second if the last used controller type has changed.
    /// </summary>
    public static IEnumerator CheckController()
    {
        if (mouseInputOnCanvas == null)
        {
            mouseInputOnCanvas = FindObjectOfType<Canvas>().GetComponent<GraphicRaycaster>();
            controller = ReInput.controllers.GetLastActiveController();
            Cursor.visible = controller.hardwareName == "Mouse";
        }

        while (true)
        {
            CheckLastUsedController();
            yield return new WaitForSecondsRealtime(0.2f);
        }
    }

    /// <summary>
    /// Return which controller has last been used.
    /// </summary>
    public static void CheckLastUsedController()
    {

        controller = ReInput.controllers.GetLastActiveController();
        //print(controller.hardwareName + " " + cursorForcedStay);
        Cursor.visible = controller.hardwareName == "Mouse";
        mouseInputOnCanvas.enabled = controller.hardwareName == "Mouse";
    }




    /* ------------------------------------------------------------------------------------------------------------------
     * Other
     * ------------------------------------------------------------------------------------------------------------------ */

    /// <summary>
    /// When returning to main menu from gameplay, reset the states of certain BetweenScenes variables.
    /// </summary>
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
    /// <param name="secondsToFade">Approximate time it takes for screen to fade in or out. Default is 0.4f</param>
    public static IEnumerator FadeScreenBlack(string intent, Image fadeScreenOverlay, float secondsToFade = 0.4f)
    {
        Color origColor = fadeScreenOverlay.color;
        fadeScreenOverlay.gameObject.SetActive(true);
        if (intent == "from") // Fade FROM black
        {
            fadingAlpha = 1f;
            while (fadingAlpha > 0f && !fadingInterrupted)
            {
                fadingAlpha -= 0.04f;
                fadeScreenOverlay.color = new Color(origColor.r, origColor.g, origColor.b, fadingAlpha);
                yield return new WaitForSeconds(secondsToFade / 25f);
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
                fadingAlpha += 0.04f;
                fadeScreenOverlay.color = new Color(origColor.r, origColor.g, origColor.b, fadingAlpha);
                yield return new WaitForSeconds(secondsToFade / 25f);
            }
            fadingInterrupted = false;
        }
    }
}
