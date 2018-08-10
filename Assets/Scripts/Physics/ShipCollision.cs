using System.Collections;
using UnityEngine;

///
/// ShipCollision.cs
///
/// Author: Eric Thompson (Dead Battery Games)
/// Purpose: Handles ship collision and damage
///

public class ShipCollision : MonoBehaviour {

    // Public
    public float velocityThreshold = 10f;

    // Private
    private ShipController shipController;
    private FlightHelp flightHelp;

    // Use this for initialization
    void Start () {
        shipController = GetComponent<ShipController>();
        flightHelp = FindObjectOfType<FlightHelp>();
	}

    void OnCollisionEnter(Collision collision) {
        if (!collision.gameObject.CompareTag("Enemy") && !collision.gameObject.CompareTag("Bullet")) {
            if (collision.relativeVelocity.magnitude > velocityThreshold) {
                BreakShip(true);
            }
        }
    }

    public void BreakShip(bool detach = false) {
        // Turn off ship control & stability
        shipController.shipRB.angularDrag = 0f;

        // Disable space particles
        shipController.spaceParticles.Stop();
        shipController.spaceParticles.Clear();

        // Detatch physical modules
        shipController.MainPower(false);
        shipController.DetachAllPhysical();

        flightHelp.RefreshComponents();

        if (detach) StartCoroutine("DetachCam");
    }

    IEnumerator DetachCam() {
        ShipCam shipCam = FindObjectOfType<ShipCam>();
        shipCam.Detach();

        shipController.enabled = false;

        yield return new WaitForSeconds(5f);

        FindObjectOfType<MenuManager>().RestartLevel();
    }
}
