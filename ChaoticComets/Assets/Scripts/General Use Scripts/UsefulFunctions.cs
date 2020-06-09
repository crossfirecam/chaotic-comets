using Rewired;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UsefulFunctions : MonoBehaviour
{
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

    private static Controller controller;
    private static GraphicRaycaster mouseInputOnCanvas;
    private static bool willMouseReturn = true;
    public static IEnumerator CheckForControllerChanges()
    {
        while (true)
        {
            CheckLastUsedController();
            yield return new WaitForSecondsRealtime(0.5f);
        }
    }

    public static void SetupControllerCheck()
    {
        print("Finding Canvas");
        mouseInputOnCanvas = FindObjectOfType<Canvas>().GetComponent<GraphicRaycaster>();
        controller = ReInput.controllers.GetLastActiveController();
        Cursor.visible = controller.hardwareName == "Mouse";
    }
    public static void CheckLastUsedController()
    {
        if (mouseInputOnCanvas == null)
        {
            SetupControllerCheck();
        }
        controller = ReInput.controllers.GetLastActiveController();
        print(controller.hardwareName);
        Cursor.visible = controller.hardwareName == "Mouse";
        mouseInputOnCanvas.enabled = controller.hardwareName == "Mouse";

    }

}
