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
    private static GraphicRaycaster mouseInputOnCanvas;
    
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
        if (mouseInputOnCanvas == null) { SetupControllerCheck(); }

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
        BetweenScenes.MusicVolume = PlayerPrefs.GetFloat("Music");
        BetweenScenes.SFXVolume = PlayerPrefs.GetFloat("SFX");
        BetweenScenes.UpgradesP1 = new int[] { 10, 10, 10, 10 };
        BetweenScenes.UpgradesP2 = new int[] { 10, 10, 10, 10 };
    }
}
