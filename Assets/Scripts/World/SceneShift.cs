using UnityEngine;

///
/// SceneShift.cs
///
/// Author: Eric Thompson (Dead Battery Games)
/// Purpose: Periodically move scene objects in order to center object at the origin
///

public class SceneShift : MonoBehaviour {

    // Public
    public float threshold = 1000.0f;

    // Private
    private Transform[] objects;
    private ParticleSystem[] particles;
    private ParticleSystem.Particle[] parts = null;
    private ShipController shipController;

    void Start() {
        shipController = FindObjectOfType<ShipController>();
    }

    void FixedUpdate() {
        Vector3 currentPos = transform.position;
        
        if (currentPos.magnitude > threshold) {
            objects = FindObjectsOfType<Transform>();

            foreach (Transform thing in objects) {
                if (thing.parent == null && thing.name != "GameManager" && thing.name != "HUD") {
                    thing.position -= currentPos;
                }
            }

            particles = FindObjectsOfType<ParticleSystem>();
            foreach (ParticleSystem system in particles) {
                if (system.main.simulationSpace != ParticleSystemSimulationSpace.World) continue;

                int numParts = system.main.maxParticles;
                if (numParts <= 0) continue;

                bool wasPaused = system.isPaused;
                bool wasPlaying = system.isPlaying;
                if (!wasPaused) system.Pause();

                if (parts == null || parts.Length < numParts) {
                    parts = new ParticleSystem.Particle[numParts];
                }

                int num = system.GetParticles(parts);
                for (int i = 0; i < num; i++) {
                    parts[i].position -= currentPos;
                }

                system.SetParticles(parts, num);

                if (shipController.thrusters) {
                    if (system.transform.parent == shipController.thrusters.transform) {
                        if (!shipController.assistModule || shipController.assistModule.GetMode() == AssistModule.AssistMode.Off) {
                            if (Input.GetAxis("Vertical") == 0) continue;
                        }
                    }
                }
                
                if (wasPlaying) system.Play();

            }
            Debug.LogWarning("Scene shifted");
        }
    }
}
