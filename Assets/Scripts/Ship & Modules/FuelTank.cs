using UnityEngine;
using UnityEngine.UI;

///
/// FuelTank.cs
///
/// Author: Eric Thompson (Dead Battery Games)
/// Purpose: Provides fuel for the ship; other modules depend on the amount of fuel in this tank
///

public class FuelTank : ShipModule {

    // Public
    public CanvasGroup uiFuelGuage;
    public Image uiFuelGuageFill;

	// Private
    [SerializeField] private float maxFuel;
	private float currentFuel;
    private bool shipStability = true;

    private Color fillColour;

	public override void ConnectShip(Transform ship) {
        base.ConnectShip(ship);
        Debug.Log("Fuel Tank connected");
    }

    // Ship module functions
    public override void DisconnectShip() {
        base.DisconnectShip();
        Debug.Log("Fuel Tank disconnected");
    }

    public override void Activate() {
        base.Activate();
        uiFuelGuage.alpha = 1f;
        if (!IsEmpty()) shipController.shipRB.angularDrag = 2f;
        Debug.Log("Fuel Tank online");
    }

    public override void Deactivate() {
        base.Deactivate();
        uiFuelGuage.alpha = 0f;
        Debug.Log("Fuel Tank offline");
    }

	protected override void Start () {
        base.Start();
        currentFuel = maxFuel;
        uiFuelGuage.alpha = 0f;
        fillColour = uiFuelGuageFill.color;
    }

    // Removes fuel from tank
    public void Drain(float amount) {
        if (active) {
            currentFuel -= amount;
            if (currentFuel < 0) currentFuel = 0;
            RefreshUI();
        }
    }

    // Adds fuel to tank
    public void Fill(float amount) {
        if (!active) {
            currentFuel += amount;
            if (currentFuel > maxFuel) currentFuel = maxFuel;
            RefreshUI();
        }
    }

    // Checks if the tank is empty & toggles ship stability
    public bool IsEmpty() {
        if (currentFuel == 0) {
            if (shipStability) {
                shipController.shipRB.angularDrag = 0f;
                shipController.shipGB.enabled = true;
                shipStability = false;
            }
            return true;
        } else {
            if (!shipStability) {
                shipController.shipRB.angularDrag = 2f;
                shipStability = true;
            }
            return false;
        }
    }

    // Updates UI for fuel levels
    void RefreshUI() {
        float fill = currentFuel / maxFuel;
        uiFuelGuageFill.fillAmount = fill;
        if (fill < 0.25) {
            uiFuelGuageFill.color = Color.red;
        } else {
            uiFuelGuageFill.color = fillColour;
        }
    }
}
