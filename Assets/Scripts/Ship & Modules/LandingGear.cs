using UnityEngine;

///
/// LandingGear.cs
///
/// Author: Eric Thompson (Dead Battery Games)
/// Purpose: Extendable landing gear for ship
///

public class LandingGear : ShipModule {

    // Public
    public float gearSpeed = 1f;
    [HideInInspector] public bool extended = true;

    // Private
    [SerializeField] private Transform frontSki;
    [SerializeField] private float frontYRetracted;
    [SerializeField] private Transform backSki;
    [SerializeField] private float backYRetracted;
    private float yExtended;

    public override void Activate() {
        base.Activate();
        Debug.Log("Landing Gear online");
    }

    public override void Deactivate() {
        base.Deactivate();
        Debug.Log("Landing Gear offline");
    }

    protected override void Start() {
        base.Start();
        yExtended = frontSki.localPosition.y;
    }

    void Update () {
        if (active) {
            // Toggle landing gear
            if (Input.GetButtonDown("LandingGear") && shipController.control) {
                if ((shipController.quantumDrive && shipController.quantumDrive.GetCurrentState() == QuantumDrive.QuantumState.Idle)
                || !shipController.quantumDrive) {
                    if (extended) {
                        extended = false;
                    } else {
                        extended = true;
                        // Force player out of astro flight
                        if (shipController.assistModule) {
                            if (shipController.assistModule.GetMode() == AssistModule.AssistMode.AstroFlight) {
                                shipController.assistModule.SetMode(AssistModule.AssistMode.HoverMode);
                            }
                        }
                    }
                }
            }

            // Animation
            if (extended) {
                ExtendGear();
            } else {
                RetractGear();
            }
        }
    }

    void ExtendGear() {
        Vector3 targetPosition = new Vector3(frontSki.localPosition.x, yExtended, frontSki.localPosition.z);
        frontSki.localPosition = Vector3.MoveTowards(frontSki.localPosition, targetPosition, Time.deltaTime * gearSpeed);

        targetPosition = new Vector3(backSki.localPosition.x, yExtended, backSki.localPosition.z);
        backSki.localPosition = Vector3.MoveTowards(backSki.localPosition, targetPosition, Time.deltaTime * gearSpeed);
    }

    void RetractGear() {
        Vector3 targetPosition = new Vector3(frontSki.localPosition.x, frontYRetracted, frontSki.localPosition.z);
        frontSki.localPosition = Vector3.MoveTowards(frontSki.localPosition, targetPosition, Time.deltaTime * gearSpeed);

        targetPosition = new Vector3(backSki.localPosition.x, backYRetracted, backSki.localPosition.z);
        backSki.localPosition = Vector3.MoveTowards(backSki.localPosition, targetPosition, Time.deltaTime * gearSpeed);
    }
}
