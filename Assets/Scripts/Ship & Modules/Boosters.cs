using UnityEngine;

///
/// Boosters.cs
///
/// Author: Eric Thompson (Dead Battery Games)
/// Purpose: Provides vertical, horizontal and rotational thrust when connected to ship
///

public class Boosters : ShipModule {

    // Private
    [SerializeField] private float linearPower = 20f;
    [SerializeField] private float rotationalPower = 3f;

    [SerializeField] private float efficiency = 1f;

    private Vector3 linearInput;
    private Vector3 rotationalInput;

    // Ship module functions
    public override void ConnectShip(Transform ship) {
        base.ConnectShip(ship);
        Debug.Log("Boosters connected");
    }

    public override void DisconnectShip() {
        base.DisconnectShip();
        Debug.Log("Boosters disconnected");
    }

    public override void Activate() {
        base.Activate();
        if (shipController) shipController.lights.BoosterLights(true);
        Debug.Log("Boosters online");
    }

    public override void Deactivate() {
        base.Deactivate();
        if (shipController) shipController.lights.BoosterLights(false);
        Debug.Log("Boosters offline");
    }

    void Update () {
        if (connected && active && shipController.control) {
            linearInput = GetLinearInput();
            rotationalInput = GetRotationalInput();
        } else {
            linearInput = Vector3.zero;
            rotationalInput = Vector3.zero;
        }
	}

    // Get input axis for horizontal and vertical Boosters
    Vector3 GetLinearInput() {
        float boostX = -Input.GetAxis("BoostHorizontal");
        float boostY = Input.GetAxis("BoostVertical");

        return new Vector3(boostX, boostY, 0.0f);
    }

    // Get input axis for rotational Boosters
    Vector3 GetRotationalInput() {
        float boostX, boostY;
        if (Input.GetButton("RotateShipCam")) {
            boostX = 0f;
            boostY = 0f;
        } else {
            boostX = -Input.GetAxis("Pitch");
            boostY = Input.GetAxis("Yaw");
        }
        
        float boostZ = -Input.GetAxis("Roll");

        return new Vector3(boostX, boostY * 0.5f, boostZ);
    }

    protected override void FixedUpdate() {
        base.FixedUpdate();
        // Check that the boosters are active and there is a Fuel Tank attached to the ship
        if (connected && active && shipController.fuelTank && shipController.control) {
            // Check Fuel Tank
            if (shipController.fuelTank.IsActive() && !shipController.fuelTank.IsEmpty()) {
                // Apply forces
                shipController.shipRB.AddRelativeForce(linearInput * linearPower, ForceMode.Acceleration);
                shipController.shipRB.AddRelativeTorque(rotationalInput * rotationalPower, ForceMode.Acceleration);

                // Burn fuel
                if (linearInput != Vector3.zero || rotationalInput != Vector3.zero) {
                    shipController.fuelTank.Drain(Time.fixedDeltaTime * linearInput.magnitude / efficiency);
                    shipController.fuelTank.Drain(Time.fixedDeltaTime * rotationalInput.magnitude / Mathf.Pow(efficiency, 2f));
                }
            }
        }
    }
}
