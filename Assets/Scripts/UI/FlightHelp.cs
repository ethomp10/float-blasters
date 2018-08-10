using UnityEngine;
using UnityEngine.UI;

///
/// FlightHelp.cs
///
/// Author: Eric Thompson (Dead Battery Games)
/// Purpose: Displays flight controls dynamically based on ship components
///

public class FlightHelp : MonoBehaviour {

    // Public
    public Text reqThruster;
    public Text reqBooster;
    public Text reqAssistModule;
    public Text reqQuantumDrive;

    public Color componentAttached;
    public Color componentDetached;

    public float fadeSpeed = 1f;

    // Private
    private CanvasGroup canvasGroup;
    private bool show = false;
    private ShipController shipController;

    void Start() {
        shipController = FindObjectOfType<ShipController>();
        
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
    }
	
	void Update () {
        if (Input.GetButtonDown("ShowHelp") && shipController.control) {
            if (show) {
                ToggleShow(false);
            } else {
                ToggleShow(true);
            }
        }

		if (show) {
            Show();
        } else {
            Hide();
        }
	}

    public void ToggleShow(bool set, bool immediate = false) {
        if (set) {
            RefreshComponents();
            if (immediate) {
                canvasGroup.alpha = 1f;
            }
        } else {
            if (immediate) {
                canvasGroup.alpha = 0f;
            }
        }

        show = set;
    }

    public void RefreshComponents() {
        // Check for Thrusters
        if (shipController.thrusters) {
            reqThruster.color = componentAttached;
        } else {
            reqThruster.color = componentDetached;
        }

        // Check for Boosters
        if (shipController.boosters) {
            reqBooster.color = componentAttached;
        } else {
            reqBooster.color = componentDetached;
        }

        // Check for Assist Module
        if (shipController.assistModule) {
            reqAssistModule.color = componentAttached;
        } else {
            reqAssistModule.color = componentDetached;
        }

        // Check for Quantum Drive
        if (shipController.quantumDrive) {
            reqQuantumDrive.color = componentAttached;
        } else {
            reqQuantumDrive.color = componentDetached;
        }
    }

    void Show() {
        if (canvasGroup.alpha < 1) {
            canvasGroup.alpha += fadeSpeed * Time.deltaTime;
        }
    }

    void Hide() {
        if (canvasGroup.alpha > 0) {
            canvasGroup.alpha -= fadeSpeed * Time.deltaTime;
        }
    }
}
