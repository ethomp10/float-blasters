using UnityEngine;
using UnityEngine.UI;

///
/// AssistModule.cs
///
/// Author: Eric Thompson (Dead Battery Games)
/// Purpose: Provides two flight assist modes to the ship; Hover Mode & Astro Flight
///

public class AssistModule : ShipModule {

    // Public
    public enum AssistMode { Off, HoverMode, AstroFlight }
    public CanvasGroup uiThrottle;
    public Image uiThrottleFill;

    // Private
    private AssistMode currentMode = AssistMode.Off;

    [SerializeField] private float efficiency = 2.0f;
    [SerializeField] private float idleBurnRate = 2.0f;
    [SerializeField] private float hoverDampening = 1.5f;
    [SerializeField] private float astroTurnSpeed = 1.0f;
    [SerializeField] private float astroThrottleSensitivity = 50.0f;
    [SerializeField] private float astroMaxSpeed = 50.0f;

    private float targetSpeed;

    // Ship module functions
    public override void ConnectShip(Transform ship) {
		base.ConnectShip(ship);
		Debug.Log("Assist Module connected");
	}

    public override void DisconnectShip() {
        base.DisconnectShip();
        Debug.Log("Assist Module disconnected");
	}

	public override void Activate() {
		base.Activate();
        SetMode(AssistMode.HoverMode);
        Debug.Log("Assist Module online");
	}

	public override void Deactivate() {
        base.Deactivate();
        if (shipController) {
            SetMode(AssistMode.Off);
            shipController.lights.AssistLights(false);
        }
    }

    public void SetMode(AssistMode mode) {
        switch (mode) {
            case AssistMode.HoverMode:
                shipController.shipGB.enabled = false;
                shipController.lights.HoverIndicator();
                uiThrottle.alpha = 0f;
                Debug.Log("Hover Mode active");
                break;
            case AssistMode.AstroFlight:
                shipController.shipGB.enabled = false;
                shipController.lights.AstroIndicator();
                targetSpeed = transform.InverseTransformDirection(shipController.shipRB.velocity).z;
                uiThrottle.alpha = 1f;
                Debug.Log("Astro Flight active");
                break;
            case AssistMode.Off:
                shipController.shipGB.enabled = true;
                shipController.lights.AssistOffIndicator();
                uiThrottle.alpha = 0f;
                Debug.Log("Assist Module offline");
                break;
            default:
                break;
        }
        currentMode = mode;
    }

    public AssistMode GetMode() {
        return currentMode;
    }

    protected override void Start() {
        base.Start();
        uiThrottle.alpha = 0f;
    }

    void Update() {
        if (connected && active) {
            // Check for required components
            if (shipController.boosters && shipController.fuelTank && shipController.IsPowered()) {
                // Activates Hover Mode
                if (Input.GetButtonDown("HoverMode") && shipController.control && !shipController.fuelTank.IsEmpty()) {
                    if (currentMode != AssistMode.HoverMode) SetMode(AssistMode.HoverMode);
                }

                // Activates Astro Flight
                if (Input.GetButtonDown("AstroFlight") && shipController.control && shipController.thrusters && !shipController.fuelTank.IsEmpty()) {
                    if (shipController.landingGear.extended) {
                        shipController.landingGear.extended = false;
                    }
                    if (currentMode != AssistMode.AstroFlight) SetMode(AssistMode.AstroFlight);
                }

                // Deactivates Assist Module
                if ((Input.GetButtonDown("AssistOff") && shipController.control) || shipController.fuelTank.IsEmpty()) {
                    if (currentMode != AssistMode.Off) SetMode(AssistMode.Off);
                }
            }

            // Apply assists
            switch (currentMode) {
                case AssistMode.HoverMode:
                    HoverMode();
                    break;
                case AssistMode.AstroFlight:
                    AstroFlight();
                    break;
                default:
                    break;
            }
        }
    }

    // Allows the player to make fine adjustments to the ship for landing/taking off, ignores planetary gravity; 
    void HoverMode() {
        // Drain fuel
        shipController.fuelTank.Drain(idleBurnRate / efficiency * Time.deltaTime);
        
        // Hover
        shipController.shipRB.velocity = Vector3.Lerp(shipController.shipRB.velocity, Vector3.zero, hoverDampening * Time.deltaTime);
    }

    // Allows the player to fly at variable speeds with precision, ignores planetary gravity, good for dogfighting and planetary flight
    void AstroFlight() {
        if (Input.GetAxis("Thrust") != 0 && shipController.control) {
            // Adjust target speed
            targetSpeed += Input.GetAxis("Thrust") * astroThrottleSensitivity * Time.deltaTime;
        }

        if (Input.GetButtonDown("CutThrottle") && shipController.control) {
            targetSpeed = 0f;
        }

        // Limit target speed
        targetSpeed = Mathf.Clamp(targetSpeed, -astroMaxSpeed * 0.5f, astroMaxSpeed);

        // Cut throttle on exit
        if (!shipController.control) {
            targetSpeed = 0f;
        }

        // Refresh UI
        float throttle = targetSpeed / astroMaxSpeed;
        if (targetSpeed > 0) {
            uiThrottleFill.color = Color.green;
            uiThrottleFill.fillAmount = throttle;
        } else {
            uiThrottleFill.color = Color.red;
            uiThrottleFill.fillAmount = -throttle;
        }

        // Drain fuel
        shipController.fuelTank.Drain((Mathf.Abs(throttle) / efficiency + idleBurnRate) * Time.deltaTime);

        // Astro flight
        shipController.shipRB.velocity = Vector3.Lerp(shipController.shipRB.velocity, transform.forward * targetSpeed, astroTurnSpeed * Time.deltaTime);
    }
}
