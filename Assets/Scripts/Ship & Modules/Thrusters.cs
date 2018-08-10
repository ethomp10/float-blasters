using UnityEngine;

///
/// Thrusters.cs
///
/// Author: Eric Thompson (Dead Battery Games)
/// Purpose: Provides forward and reverse thrust when attached to a ship
///

public class Thrusters : ShipModule {

    // Private
    [SerializeField] public float thrusterPower = 50f;
    [SerializeField] private float efficiency = 1f;
    [SerializeField] private ParticleSystem[] engineParticles;

    private float thrusterInput;
    private bool particlesPlaying = false;

    // Ship module functions
    public override void ConnectShip(Transform ship) {
        base.ConnectShip(ship);
        Debug.Log("Thrusters connected");
    }

    public override void DisconnectShip() {
        base.DisconnectShip();
        Debug.Log("Thrusters disconnected");
    }

    public override void Activate() {
        base.Activate();
        if (shipController) shipController.lights.ThrusterLights(true);
        Debug.Log("Thrusters online");
    }

    public override void Deactivate() {
        base.Deactivate();
        StopParticles();
        if (shipController) shipController.lights.ThrusterLights(false);
        Debug.Log("Thrusters offline");
    }

    void PlayParticles() {
        if (!particlesPlaying) {
            foreach (ParticleSystem particleSystem in engineParticles) {
                particleSystem.Play();
            }
            particlesPlaying = true;
        }
    }

    void StopParticles() {
        if (particlesPlaying) {
            foreach (ParticleSystem particleSystem in engineParticles) {
                particleSystem.Stop();
            }
            particlesPlaying = false;
        }
    }

    void Update () {
        if (connected && active && shipController.fuelTank && shipController.control) {
            thrusterInput = Input.GetAxis("Thrust");

            // Handle Particles
            if (!shipController.fuelTank.IsEmpty()) {
                if (shipController.assistModule && shipController.assistModule.GetMode() != AssistModule.AssistMode.Off) {
                    PlayParticles();
                } else {
                    if (thrusterInput != 0) PlayParticles();
                    else StopParticles();
                }
            } else {
                StopParticles();
            }
        } else {
            thrusterInput = 0f;
        }
	}

    protected override void FixedUpdate() {
        base.FixedUpdate();
        // Check that the boosters are active and there is a Fuel Tank attached to the ship
        if (connected && active && shipController.fuelTank) {
            // Check Fuel Tank
            if (shipController.fuelTank.IsActive() && !shipController.fuelTank.IsEmpty()) {
                // Apply forces
                shipController.shipRB.AddRelativeForce(Vector3.forward * thrusterInput * thrusterPower, ForceMode.Acceleration);

                // Burn fuel
                if (thrusterInput != 0) {
                    shipController.fuelTank.Drain(Mathf.Abs(thrusterInput) / efficiency  * Time.fixedDeltaTime);
                }
            }
        }
    }
}
