using UnityEngine;

///
/// TankConnect.cs
///
/// Author: Eric Thompson (Dead Battery Games)
/// Purpose: Connects fuel tank to fuel pad & automatically fuels ship
///

public class TankConnect : MonoBehaviour {

    private ShipController shipController;

    private bool tankInRange = false;
    private bool tankConnected = false;
    private FuelPad pad = null;

	// Use this for initialization
	void Start () {
        shipController = FindObjectOfType<ShipController>();
	}
	
	// Update is called once per frame
	void Update () {
		if (tankInRange) {
            if (!shipController.IsPowered() && shipController.fuelTank && shipController.landingGear.extended) {
                if (!tankConnected) {
                    pad.ConnectTank(shipController.fuelTank);
                    tankConnected = true;
                }
            } else {
                if (tankConnected) {
                    pad.DisconnectTank();
                    tankConnected = false;
                }
            }
        }
	}

    void OnTriggerEnter(Collider other) {
        if (other.CompareTag("PadConnect")) {
            tankInRange = true;
            pad = other.GetComponentInParent<FuelPad>();
        }
    }

    void OnTriggerExit(Collider other) {
        if (other.CompareTag("PadConnect")) {
            tankInRange = false;
            pad.DisconnectTank();
            pad = null;
            tankConnected = false;
        }
    }
}
