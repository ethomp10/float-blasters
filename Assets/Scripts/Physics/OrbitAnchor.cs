using UnityEngine;

///
/// OrbitAnchor.cs
///
/// Author: Eric Thompson (Dead Battery Games)
/// Purpose: Ensures the nearest celestial body remains static
///

[RequireComponent(typeof(SphereCollider))]

public class OrbitAnchor : MonoBehaviour {

    public GameObject gorbons = null;

	private OrbitManager orbitManager;
    private ShipController shipController;

	// Use this for initialization
	void Start () {
		orbitManager = FindObjectOfType<OrbitManager>();
        shipController = FindObjectOfType<ShipController>();
	}

	void OnTriggerEnter(Collider other) {
        if ((other.CompareTag("PlayerShip") && shipController.control) || other.CompareTag("Player")) {
            orbitManager.SetAnchor(transform.parent);

            if (gorbons) gorbons.SetActive(true);
        }

        if (other.CompareTag("PlayerShip") && shipController.quantumDrive) {
            shipController.quantumDrive.Lock();
            if (shipController.assistModule) shipController.lights.QuantumInactiveIndicator();
        }

        if (other.CompareTag("Player") && transform.parent.name == "Moon") {
            other.GetComponent<PlayerControl>().canSprint = false;
        }
    }

	void OnTriggerExit(Collider other) {
        if ((other.CompareTag("PlayerShip") && shipController.control) || other.CompareTag("Player")) {
            orbitManager.SetAnchor(null);

            if (gorbons) gorbons.SetActive(false);
        }

        if (other.CompareTag("PlayerShip") && shipController.quantumDrive) {
            shipController.quantumDrive.Unlock();
            if (shipController.assistModule) shipController.lights.QuantumReadyIndicator();
        }
    }
}
