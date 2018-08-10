using UnityEngine;
using UnityEngine.UI;

///
/// QuantumDrive.cs
///
/// Author: Eric Thompson (Dead Battery Games)
/// Purpose: Allows player to Quantum jump to various locations
///

public class QuantumDrive : ShipModule {


    // Public
    public enum QuantumState { Idle, Searching, Breaking, Jumping }

    public float jumpSpeed = 500f;
    public float breakSpeed = 3f;
    public float jumpFOV = 90f;
    public float burnRate = 10f;
    public LayerMask quantumTargetMask;

    // Private
    [SerializeField] private ParticleSystem quantumParts;

    private GameObject target;
    private Image crosshair;
    private bool jumpComplete = false;
    private bool quantumLock = true;
    private Camera shipCam;
    private float originalFOV;

    private QuantumState currentState;

    // Ship module functions
    public override void ConnectShip(Transform ship) {
        base.ConnectShip(ship);
        Debug.Log("Quantum Drive connected");
    }

    public override void DisconnectShip() {
        base.DisconnectShip();
        Debug.Log("Quantum Drive disconnected");
    }

    public override void Activate() {
        base.Activate();
        Debug.Log("Quantum Drive online");
    }

    public override void Deactivate() {
        base.Deactivate();
        if (shipController) shipController.lights.QuantumLights(false);
        quantumParts.Stop(); quantumParts.Clear();
        Debug.Log("Quantum Drive offline");
    }

    protected override void Start() {
        base.Start();
        crosshair = GameObject.Find("Crosshair").GetComponent<Image>();
        currentState = QuantumState.Idle;
        shipCam = FindObjectOfType<ShipCam>().GetComponentInChildren<Camera>();
        originalFOV = shipCam.fieldOfView;
    }

    void Update() {
        if (connected && active) {
            switch (currentState) {
                case QuantumState.Idle:
                    Idle();
                    ResetFOV();
                    break;
                case QuantumState.Searching:
                    Search();
                    ResetFOV();
                    break;
                case QuantumState.Breaking:
                    SpaceBreak();
                    ResetFOV();
                    break;
                case QuantumState.Jumping:
                    Jump();
                    break;
            }
        } else {
            ResetFOV();
        }
    }

    public void Lock() {
        quantumLock = true;
        jumpComplete = true;
        if (active && connected && currentState == QuantumState.Jumping) {
            ChangeState(QuantumState.Breaking);
            if (shipController.assistModule) shipController.lights.QuantumInactiveIndicator();
        }
    }

    public void Unlock() {
        quantumLock = false;
        if (active && connected && shipController.assistModule) shipController.lights.QuantumReadyIndicator();
    }

    void ChangeState(QuantumState state) {
        switch (state) {
            case QuantumState.Idle:
                crosshair.enabled = false;
                shipController.thrusters.Activate();
                shipController.boosters.Activate();

                // Indicator Lights
                if (shipController.assistModule) {
                    if (quantumLock) shipController.lights.QuantumInactiveIndicator();
                    else shipController.lights.QuantumReadyIndicator();
                    shipController.assistModule.Activate();
                }
                break;
            case QuantumState.Searching:
                // Indicator Lights
                if (shipController.assistModule) {
                    shipController.lights.QuantumActiveIndicator();
                }

                crosshair.enabled = true;
                break;
            case QuantumState.Breaking:
                crosshair.enabled = false;
                shipController.thrusters.Deactivate();
                shipController.boosters.Deactivate();

                // Indicator Lights
                if (shipController.assistModule) {
                    shipController.assistModule.Deactivate();
                    shipController.lights.QuantumActiveIndicator();
                }

                quantumParts.Stop();
                break;
            case QuantumState.Jumping:
                shipController.thrusters.Deactivate();
                shipController.boosters.Deactivate();
                quantumParts.Play();

                // Indicator Lights
                if (shipController.assistModule) {
                    shipController.lights.QuantumActiveIndicator();
                }

                break;
            default:
                break;
        }

        currentState = state;
    }

    public QuantumState GetCurrentState() {
        return currentState;
    }

    void Idle() {
        if (!quantumLock && Input.GetButtonDown("QuantumJump")) {
            jumpComplete = false;
            shipController.landingGear.extended = false;
            if (CheckAlignment()) {
                ChangeState(QuantumState.Breaking);
            } else {
                ChangeState(QuantumState.Searching);
            }
        }
    }

    void Search() {
        if (!quantumLock && Input.GetButtonDown("QuantumJump")) {
            ChangeState(QuantumState.Idle);
        }

        if (!quantumLock && CheckAlignment()) {
            if (shipController.assistModule) shipController.lights.QuantumReadyIndicator();
            ChangeState(QuantumState.Breaking);
        }
    }

    void SpaceBreak() {
        if (shipController.shipRB.velocity.magnitude > 3f || shipController.shipRB.angularVelocity.magnitude > 0.01f) {
            shipController.shipRB.velocity = Vector3.Lerp(shipController.shipRB.velocity, Vector3.zero, breakSpeed * Time.deltaTime);
            shipController.shipRB.angularVelocity = Vector3.Lerp(shipController.shipRB.angularVelocity, Vector3.zero,  5f * Time.deltaTime);
        } else {
            if (jumpComplete) {
                ChangeState(QuantumState.Idle);
            } else {
                ChangeState(QuantumState.Jumping);
            }
        }
    }

    void Jump() {
        if (!shipController.fuelTank.IsEmpty()) {
            shipController.shipRB.velocity = Vector3.Lerp(shipController.shipRB.velocity, transform.forward * jumpSpeed, breakSpeed * Time.deltaTime);
            shipController.fuelTank.Drain(burnRate * Time.deltaTime);
        }

        if (Input.GetButtonDown("QuantumJump") || shipController.fuelTank.IsEmpty()) {
            ChangeState(QuantumState.Breaking);
            jumpComplete = true;
        }

        shipCam.fieldOfView = Mathf.Lerp(shipCam.fieldOfView, jumpFOV, Time.deltaTime);
    }

    void ResetFOV() {
        if (shipCam.fieldOfView != originalFOV) {
            shipCam.fieldOfView = Mathf.Lerp(shipCam.fieldOfView, originalFOV, Time.deltaTime);
        }
    }

    bool CheckAlignment() {
        if (Physics.Raycast(transform.position, transform.forward, Mathf.Infinity, quantumTargetMask)) {
            return true;
        } else {
            return false;
        }
    }
}
