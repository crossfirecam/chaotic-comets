using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

}
