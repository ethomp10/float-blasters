using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

///
/// Lights.cs
///
/// Author: Eric Thompson (Dead Battery Games)
/// Purpose: Controls lights on the ship
///

public class Lights : ShipModule {

    // Public
    public float holoTextFadeDelay = 1f;

    // Private
    [SerializeField] private Material headlightMaterial;
    [SerializeField] private Material onMaterial;
    [SerializeField] private Material activeMaterial;
    [SerializeField] private Material inactiveMaterial;
    [SerializeField] private Material offMaterial;
    [SerializeField] private MeshRenderer hoverIndicator;
    [SerializeField] private MeshRenderer astroIndicator;
    [SerializeField] private MeshRenderer quantumIndicator;
    [SerializeField] private Text hoverHolotext;
    [SerializeField] private Text astroHolotext;
    [SerializeField] private Text quantumHolotext;
    [SerializeField] private CanvasGroup holoTextGroup;

    private MeshRenderer headlight;
    private MeshRenderer[] thrusterLights;
    private MeshRenderer[] boosterLights;
    private MeshRenderer[] assistLights;
    private MeshRenderer[] quantumDriveLights;

    private Light headlightGlow;
    private Light[] thrusterGlow;
    private Light[] boosterGlow;
    private Light[] assistGlow;
    private Light[] quantumDriveGlow;

    private bool fadeText;
    private float fadeSpeed = 1f;

    private bool headlightOn = false;

    public override void Activate() {
        base.Activate();
        Debug.Log("Lights online");

        if (shipController.thrusters) ThrusterLights(true);
        if (shipController.boosters) BoosterLights(true);
        if (shipController.assistModule && shipController.assistModule.IsActive()) AssistLights(true);
        if (shipController.quantumDrive) QuantumLights(true);
    }

    public override void Deactivate() {
        base.Deactivate();
        Debug.Log("Lights offline");

        Headlight(false);
        if (shipController.thrusters) ThrusterLights(false);
        if (shipController.boosters) BoosterLights(false);
        if (shipController.assistModule) AssistLights(false);
        if (shipController.quantumDrive) QuantumLights(false);
    }

    protected override void Start() {
        base.Start();

        hoverHolotext.enabled = false;
        astroHolotext.enabled = false;
        quantumHolotext.enabled = false;

        List<GameObject> objects = new List<GameObject>();

        // Mesh Renderers
        headlight = GameObject.Find("Headlight").GetComponent<MeshRenderer>();

        objects.AddRange(GameObject.FindGameObjectsWithTag("ThrustLights"));
        thrusterLights = new MeshRenderer[objects.Count];
        for (int i = 0; i < objects.Count; i++) {
            thrusterLights[i] = objects[i].GetComponent<MeshRenderer>();
        }

        objects.Clear(); objects.AddRange(GameObject.FindGameObjectsWithTag("BoostLights"));
        boosterLights = new MeshRenderer[objects.Count];
        for (int i = 0; i < objects.Count; i++) {
            boosterLights[i] = objects[i].GetComponent<MeshRenderer>();
        }

        objects.Clear(); objects.AddRange(GameObject.FindGameObjectsWithTag("AssistLights"));
        assistLights = new MeshRenderer[objects.Count];
        for (int i = 0; i < objects.Count; i++) {
            assistLights[i] = objects[i].GetComponent<MeshRenderer>();
        }

        objects.Clear(); objects.AddRange(GameObject.FindGameObjectsWithTag("QuantumLights"));
        quantumDriveLights = new MeshRenderer[objects.Count];
        for (int i = 0; i < objects.Count; i++) {
            quantumDriveLights[i] = objects[i].GetComponent<MeshRenderer>();
        }

        // Lights
        headlightGlow = GameObject.Find("HeadlightGlow").GetComponent<Light>();

        objects.Clear(); objects.AddRange(GameObject.FindGameObjectsWithTag("ThrustGlow"));
        thrusterGlow = new Light[objects.Count];
        for (int i = 0; i < objects.Count; i++) {
            thrusterGlow[i] = objects[i].GetComponent<Light>();
        }

        objects.Clear(); objects.AddRange(GameObject.FindGameObjectsWithTag("BoostGlow"));
        boosterGlow = new Light[objects.Count];
        for (int i = 0; i < objects.Count; i++) {
            boosterGlow[i] = objects[i].GetComponent<Light>();
        }

        objects.Clear(); objects.AddRange(GameObject.FindGameObjectsWithTag("QuantumGlow"));
        quantumDriveGlow = new Light[objects.Count];
        for (int i = 0; i < objects.Count; i++) {
            quantumDriveGlow[i] = objects[i].GetComponent<Light>();
        }
    }

    void Update() {
        if (active && Input.GetButtonDown("Light")) {
            if (headlightOn) Headlight(false);
            else Headlight(true);
        }

        if (fadeText) {
            FadeHoloText();
        }
    }

    // Lights
    public void Headlight(bool toggle) {
        if (toggle) {
            headlight.material = headlightMaterial;
            headlightGlow.enabled = true;
            headlightOn = true;
        } else {
            headlight.material = offMaterial;
            headlightGlow.enabled = false;
            headlightOn = false;
        }
    }

    public void ThrusterLights(bool toggle) {
        if (toggle) {
            foreach (MeshRenderer mesh in thrusterLights) {
                mesh.material = onMaterial;
            }
            foreach (Light glow in thrusterGlow) {
                glow.enabled = true;
            }
        } else {
            foreach (MeshRenderer mesh in thrusterLights) {
                mesh.material = offMaterial;
            }
            foreach (Light glow in thrusterGlow) {
                glow.enabled = false;
            }
        }
    }

    public void BoosterLights(bool toggle) {
        if (toggle) {
            foreach (MeshRenderer mesh in boosterLights) {
                mesh.material = onMaterial;
            }
            foreach (Light glow in boosterGlow) {
                glow.enabled = true;
            }
        } else {
            foreach (MeshRenderer mesh in boosterLights) {
                mesh.material = offMaterial;
            }
            foreach (Light glow in boosterGlow) {
                glow.enabled = false;
            }
        }
    }

    public void AssistLights(bool toggle) {
        if (toggle) {
            switch (shipController.assistModule.GetMode()) {
                case AssistModule.AssistMode.HoverMode:
                    HoverIndicator();
                    break;
                case AssistModule.AssistMode.AstroFlight:
                    AstroIndicator();
                    break;
                case AssistModule.AssistMode.Off:
                    AssistOffIndicator();
                    break;
                default:
                    break;
            }
        } else {
            foreach (MeshRenderer mesh in assistLights) {
                mesh.material = offMaterial;
                holoTextGroup.alpha = 0f;
            }
        }
    }

    public void QuantumLights(bool toggle) {
        if (toggle) {
            foreach (MeshRenderer mesh in quantumDriveLights) {
                mesh.material = activeMaterial;
            }
            foreach (Light glow in quantumDriveGlow) {
                glow.enabled = true;
            }
        } else {
            foreach (MeshRenderer mesh in quantumDriveLights) {
                mesh.material = offMaterial;
            }
            foreach (Light glow in quantumDriveGlow) {
                glow.enabled = false;
            }
        }
    }
    
    // Indicators
    public void HoverIndicator() {
        if (active) {
            hoverIndicator.material = activeMaterial;
            hoverHolotext.enabled = true;

            astroIndicator.material = inactiveMaterial;
            astroHolotext.enabled = false;

            StopCoroutine("FadeDelay"); StartCoroutine("FadeDelay");
        }
    }

    public void AstroIndicator() {
        if (active) {
            astroIndicator.material = activeMaterial;
            astroHolotext.enabled = true;

            hoverIndicator.material = inactiveMaterial;
            hoverHolotext.enabled = false;

            quantumHolotext.enabled = false;

            StopCoroutine("FadeDelay"); StartCoroutine("FadeDelay");
        }
    }

    public void AssistOffIndicator() {
        if (active) {
            hoverIndicator.material = inactiveMaterial;
            hoverHolotext.enabled = false;

            astroIndicator.material = inactiveMaterial;
            astroHolotext.enabled = false;


            StopCoroutine("FadeDelay"); StartCoroutine("FadeDelay");
        }
    }

    public void QuantumReadyIndicator() {
        if (active) {
            quantumIndicator.material = onMaterial;

            hoverHolotext.enabled = false;
            quantumHolotext.enabled = false;
        }
    }

    public void QuantumActiveIndicator() {
        if (active) {
            quantumIndicator.material = activeMaterial;
            quantumHolotext.enabled = true;

            hoverHolotext.enabled = false;
            astroHolotext.enabled = false;

            StopCoroutine("FadeDelay"); StartCoroutine("FadeDelay");
        }
    }

    public void QuantumInactiveIndicator() {
        if (active) {
            quantumIndicator.material = offMaterial;
            quantumHolotext.enabled = false;
        }
    }

    IEnumerator FadeDelay() {
        fadeText = false;
        holoTextGroup.alpha = 1f;
        yield return new WaitForSeconds(holoTextFadeDelay);
        fadeText = true;
    }

    void FadeHoloText() {
        if (holoTextGroup.alpha > 0f) {
            holoTextGroup.alpha -= fadeSpeed * Time.deltaTime;
        } else {
            fadeText = false;
        }
    }
}
