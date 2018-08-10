using UnityEngine;

///
/// GravSource.cs
///
/// Author: Eric Thompson (Dead Battery Games)
/// Purpose: Attract any GravBodies in the world, simulates celestial gravity
///

public class GravSource : MonoBehaviour {

    // Public
    public float gravityStrength = 50f;

    public static float gravityMultiplyer = 10000f;

    public void Pull(Transform body) {
        // Calculate gravity vector
        Vector3 toBody = body.position - transform.position;
        float distance = toBody.magnitude;
        Vector3 gravity = -toBody.normalized * gravityStrength  * gravityMultiplyer / (distance * distance);

        // Pull GravBody towards center
        if (body.GetComponent<Rigidbody>()) body.GetComponent<Rigidbody>().AddForce(gravity, ForceMode.Acceleration);
    }
}
