using UnityEngine;
using UnityEngine.UI;

///
/// PlayerInteract.cs
///
/// Author: Eric Thompson (Dead Battery Games)
/// Purpose: Handles various player interaction with objects and vehicles
///

public class PlayerInteract : MonoBehaviour {

    // Private
    private bool nearShip = false;
    private Image crosshairUI;
    private CanvasGroup scopeUI;
    private CanvasGroup healthUI;
    private PlayerControl playerControl;

    void Start() {
        crosshairUI = GameObject.Find("Crosshair").GetComponent<Image>();
        scopeUI = GameObject.Find("Scope").GetComponent<CanvasGroup>();
        healthUI = GameObject.Find("HealthBar").GetComponent<CanvasGroup>();
        healthUI.alpha = 1.0f;

        playerControl = GetComponent<PlayerControl>();
    }

    // Update is called once per frame
    void Update () {
        if (Input.GetButtonDown("EnterExitShip")) {
            if (nearShip) {
                EnterShip();
            }
        }
    }

    void EnterShip () {
        if ((playerControl.gravClaw && !playerControl.gravClaw.holdingModule) || !playerControl.gravClaw) {
            FindObjectOfType<ShipController>().Enter();
            crosshairUI.enabled = false;
            scopeUI.alpha = 0f;
            healthUI.alpha = 0.0f;
            playerControl.hitSplash.alpha = 0.0f;
            playerControl.healSplash.alpha = 0.0f;

            PlayerStats.currentPlayerHealth = playerControl.GetCurrentHealth();

            GameObject gravParts = GameObject.Find("GravParticles");
            if (gravParts) Destroy(gravParts);
            Destroy(gameObject);
        }
    }

    public void SetNearShip(bool set) {
        nearShip = set;
    }
}
