using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

///
/// FuelPad.cs
///
/// Author: Eric Thompson (Dead Battery Games)
/// Purpose: Refuels ship
///

public class FuelPad : MonoBehaviour {

    // Public
    public float fillSpeed = 1f;

    // Private
    [SerializeField] private Material connectedMaterial;
    [SerializeField] private Material disconnectedMaterial;
    [SerializeField] private Color connectedColour;
    [SerializeField] private Color disconnectedColour;

    private List<MeshRenderer> lights;
    private List<Light> glow;

    private bool fuelTankConnected;
    private FuelTank fuelTank = null;

	// Use this for initialization
	void Start () {
        List<GameObject> objects = new List<GameObject>();

        // Mesh renderers
        objects.AddRange(GameObject.FindGameObjectsWithTag("PadLight"));
        lights = new List<MeshRenderer>();
        for (int i = 0; i < objects.Count; i++) {
            if (objects[i].transform.parent == transform) {
                lights.Add(objects[i].GetComponent<MeshRenderer>());
            }
        }

        // Lights
        objects.Clear(); objects.AddRange(GameObject.FindGameObjectsWithTag("PadGlow"));
        glow = new List<Light>();
        for (int i = 0; i < objects.Count; i++) {
            if (objects[i].transform.parent.parent == transform) {
                glow.Add(objects[i].GetComponent<Light>());
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
        if (fuelTankConnected) {
            fuelTank.Fill(fillSpeed * Time.deltaTime);
        }
	}

    public void ConnectTank(FuelTank tank) {
        if (!fuelTankConnected) {
            // Assign tank
            fuelTank = tank;
            fuelTank.uiFuelGuage.alpha = 1f;

            // Fuel pad indicator lights
            foreach (MeshRenderer light in lights) {
                light.material = connectedMaterial;
            }
            foreach (Light light in glow) {
                light.color = connectedColour;
            }

            fuelTankConnected = true;
        }
    }

    public void DisconnectTank() {
        if (fuelTankConnected) {
            // Assign tank
            if (!fuelTank.IsActive()) fuelTank.uiFuelGuage.alpha = 0f;
            fuelTank = null;

            // Fuel pad indicator lights
            foreach (MeshRenderer light in lights) {
                light.material = disconnectedMaterial;
            }
            foreach (Light light in glow) {
                light.color = disconnectedColour;
            }

            fuelTankConnected = false;
        }
    }
}
