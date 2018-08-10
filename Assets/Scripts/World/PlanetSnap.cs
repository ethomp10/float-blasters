using UnityEngine;

///
/// PlanetSnap.cs
///
/// Author: Eric Thompson (Dead Battery Games)
/// Purpose: Orients objects so their Y axis is facing away from the surface of the nearest planet
///

[ExecuteInEditMode]
public class PlanetSnap : MonoBehaviour {

    public float snapSpeed = 5f;

    private GameObject snapBody;
    private GameObject[] celestials;

    void Start () {
        celestials = GameObject.FindGameObjectsWithTag("Celestial");
        snapBody = FindClosestGravSource();
    }

    public GameObject FindClosestGravSource() {
        float shortestDistance = float.MaxValue;
        GameObject closest = null;

        foreach (GameObject body in celestials) {
            float currentDistance = (body.transform.position - transform.position).magnitude;
            if (currentDistance < shortestDistance) {
                shortestDistance = currentDistance;
                closest = body;
            }
        }
        return closest;
    }
	
	void Update () {
        Vector3 planetUp = (transform.position - snapBody.transform.position).normalized;
        
        if ((transform.up - planetUp).magnitude > 0.002f) {
            Quaternion targetRotation = Quaternion.FromToRotation(transform.up, planetUp) * transform.rotation;
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, snapSpeed * Time.deltaTime);
        }
    }
}
