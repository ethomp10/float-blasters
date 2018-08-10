using UnityEngine;

///
/// Sun.cs
///
/// Author: Eric Thompson (Dead Battery Games)
/// Purpose: Changes skybox colour based on player position
///

public class Sun : MonoBehaviour {

    // Public
    public float planetAtmosphereThickness = 50f;
    public float transitionSpeed = 2f; 
    [HideInInspector] public Camera currentCam;

    // Private
    private Transform planet;

    private Vector3 brightestPoint;
    //private float distanceToBrightestPoint;

    private LayerMask layerMask;

    void Start() {
        planet = GameObject.Find("Planet").transform;
        layerMask = LayerMask.GetMask("Planet");
    }

    void LateUpdate() {
        if (currentCam) {
            currentCam.backgroundColor = Color.Lerp(currentCam.backgroundColor, GetSkyColour(currentCam.transform.position), transitionSpeed * Time.deltaTime);
        }
    }

    public void ChangeCamera(Camera camera) {
        currentCam = camera;
        UpdateBrightestPoint();
        currentCam.backgroundColor = GetSkyColour(currentCam.transform.position);
    }

    public Color GetSkyColour(Vector3 currentPosition) {
        float distanceToBrightestPoint = (brightestPoint - currentPosition).magnitude;

        float atmosphereInfluence = 255f / planetAtmosphereThickness * ((currentPosition - planet.position).magnitude - 519f);

        float redValue = 80f - Mathf.Abs(765f - distanceToBrightestPoint) - atmosphereInfluence;
        float blueValue = (850f - distanceToBrightestPoint) - atmosphereInfluence;

        redValue = Mathf.Clamp(redValue, 0f, 80f);
        blueValue = Mathf.Clamp(blueValue, 50f, 255f);

        float greenValue = (blueValue / 2f);

        return new Color(redValue / 255f, greenValue / 255f, blueValue / 255f);
    }

    void UpdateBrightestPoint() {
        RaycastHit hit;
        if (Physics.Linecast(transform.position, planet.position, out hit, layerMask)) {
            brightestPoint = hit.point;
        }
    }

    void FixedUpdate() {
        UpdateBrightestPoint();
    }

    void OnTriggerEnter(Collider other) {
        if (other.CompareTag("PlayerShip")) {
            other.GetComponent<ShipCollision>().BreakShip(true);
        }

        if (other.CompareTag("Player")) {
            other.GetComponent<PlayerControl>().DamagePlayer(PlayerStats.maxPlayerHealth);
        }
    }
}
