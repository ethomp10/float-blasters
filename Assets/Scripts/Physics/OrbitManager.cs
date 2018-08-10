using UnityEngine;

///
/// OrbitManager.cs
///
/// Author: Eric Thompson (Dead Battery Games)
/// Purpose: Simulates orbits
///

public class OrbitManager : MonoBehaviour {

    // Public
    public Transform sun;
    public Transform planet;
    public Transform moon;

    public float planetOrbitPeriod = 24f;
    public float moonOrbitPeriod = 5f;

    // Private
    private Vector3 planetPos;
    private Vector3 moonPos;

    private Transform anchor;

    void Start() {
        // Change virtual orbit coordinates so sun is at origin
        planetPos = planet.position - sun.position;
        moonPos = moon.position - sun.position;

        // Multiply orbital periods to use minutes
        planetOrbitPeriod *= 60f;
        moonOrbitPeriod *= 60f;
    }

    void FixedUpdate() {
        if (anchor) UpdateOrbitPositions();
        if (anchor) UpdateRealPositions();
    }

    // Calculate virtual orbit positions with sun at origin
    void UpdateOrbitPositions() {
        // Sun orbit
        planetPos = Orbit(planetPos, Vector3.zero, 360f / planetOrbitPeriod);
        moonPos = Orbit(moonPos, Vector3.zero, 360f / planetOrbitPeriod);

        // Planet orbit
        moonPos = Orbit(moonPos, planetPos, 360f / moonOrbitPeriod);
    }

    // Update real planet positions around the anchor planet
    void UpdateRealPositions() {
        if (anchor == sun) {
            planet.position = planetPos + sun.position;
            moon.position = moonPos + sun.position;
        } else if (anchor == planet) {
            sun.position = -planetPos + planet.position;
            moon.position = moonPos - planetPos + planet.position;
        } else if (anchor == moon) {
            sun.position = - moonPos + moon.position;
            planet.position = planetPos - moonPos + moon.position;
        }
    }

    // Calculate orbit for a single body
    Vector3 Orbit(Vector3 satelite, Vector3 body, float angle) {
        return Quaternion.Euler(0f, angle * Time.fixedDeltaTime, 0f) * (satelite - body) + body;
    }

    public void SetAnchor(Transform transform) {
        anchor = transform;
    }
}
