using UnityEngine;

public class GravClaw : MonoBehaviour {

    // Public
    public float grabRange = 5f;
    public LayerMask grabMask;
    public LayerMask dropMask;
    public Transform snapPoint;
    public Transform prongs;
    public float snapSpeed = 3f;
    public ParticleSystem gravParticles;

    [HideInInspector] public bool canGrab = true;
    [HideInInspector] public bool holdingModule = false;
    [HideInInspector] public bool canConnect = false;

    // Private
    private Camera playerCam;
    private MouseLook mouseLook;
    private PlayerControl playerMove;
    private Transform module = null;
    private ShipController shipController;

    private Quaternion targetRotation = Quaternion.identity;
    private Vector3 prongPos;
    private bool particlesPlaying;

	void Start() {
        playerCam = GetComponentInParent<Camera>();
        playerMove = GetComponentInParent<PlayerControl>();
        mouseLook = GetComponentInParent<MouseLook>();

        shipController = FindObjectOfType<ShipController>();

        prongPos = prongs.localPosition;
        gravParticles.transform.localPosition = prongPos;
    }

    void Update() {
        if (Input.GetButtonDown("Fire") && canGrab) {
            if (holdingModule) {
                Drop();
            } else {
                Grab();
            }
        }

        var main = gravParticles.main;
        if (holdingModule) {
            SnapModule();

            prongs.localPosition = Vector3.Lerp(prongs.localPosition, prongPos + Vector3.forward * 0.24f, 10f * Time.deltaTime);
            if (!particlesPlaying) {
                gravParticles.transform.parent = transform;
                gravParticles.transform.localPosition = prongPos;
                gravParticles.transform.localRotation = Quaternion.identity;
                gravParticles.Clear();
                gravParticles.Play();
                particlesPlaying = true;
            }
        } else {
            prongs.localPosition = Vector3.Lerp(prongs.localPosition, prongPos, 10f * Time.deltaTime);
            if (particlesPlaying) {
                gravParticles.transform.parent = null;
                gravParticles.Stop();
                particlesPlaying = false;
            }
        }
    }

    void Drop() {
        // Make sure the module is not colliding with anything
        if (!Physics.Linecast(transform.position, snapPoint.position, dropMask)
        && !Physics.Linecast(transform.position, snapPoint.position + transform.right, dropMask)
        && !Physics.Linecast(transform.position, snapPoint.position - transform.right, dropMask)) {
            foreach (GameObject slot in shipController.moduleSlots) {
                if (slot.name == module.name) {
                    slot.GetComponent<ModuleSlot>().ResetCollisionCount();
                    slot.SetActive(false);
                }
            }

            if (canConnect) {
                SwitchLayer(module, LayerMask.NameToLayer("Ship"));
                module.GetComponent<ShipModule>().ConnectShip(shipController.transform);
                canConnect = false;
            } else {
                // Physics
                SwitchLayer(module, LayerMask.NameToLayer("ShipModule"));
                module.transform.parent = null;
                module.GetComponent<Rigidbody>().isKinematic = false;
                module.GetComponent<Rigidbody>().velocity = GetComponentInParent<Rigidbody>().velocity;
            }

            // Controls
            mouseLook.yLock = false;
            if (GetComponentInParent<PlanetSnap>().FindClosestGravSource().name != "Moon") {
                playerMove.canSprint = true;
            }

            module = null;
            playerMove.crosshair.enabled = true;
            holdingModule = false;
        }
    }

    void Grab() {
        Vector3 aimPoint = playerCam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit hit;
        if (Physics.Raycast(aimPoint, playerCam.transform.forward, out hit, grabRange, grabMask)) {
            if (hit.collider.GetComponentInParent<ShipModule>()) {
                if (hit.collider.GetComponentInParent<ShipModule>().name != "LandingGear") {
                    // Handle physics for module
                    module = hit.collider.GetComponentInParent<ShipModule>().transform;
                    SwitchLayer(module, LayerMask.NameToLayer("HeldModule"));
                    module.GetComponent<ShipModule>().DisconnectShip();
                    module.GetComponent<Rigidbody>().isKinematic = true;
                    module.parent = transform;

                    // Modify controls
                    mouseLook.yLock = true;
                    playerMove.canSprint = false;

                    // Show component slot on ship
                    foreach (GameObject slot in shipController.moduleSlots) {
                        if (slot.name == module.name) {
                            slot.SetActive(true);
                        }
                    }

                    playerMove.crosshair.enabled = false;
                    holdingModule = true;
                }
            }
        }
    }

    void SwitchLayer(Transform parent, int layer) {
        parent.gameObject.layer = layer;
        foreach (Transform transform in parent) {
            transform.gameObject.layer = layer;
        }
    }

    void SnapModule () {
        if (module.name == "FuelTank") {
            targetRotation = Quaternion.Euler(0f, 90f, 0f);
        } else if (module.name == "QuantumDrive") {
            targetRotation = Quaternion.Euler(0f, 180f, 0f);
        } else {
            targetRotation = Quaternion.identity;
        }

        module.transform.localPosition = Vector3.Lerp(module.transform.localPosition, snapPoint.localPosition, snapSpeed * Time.deltaTime);
        module.transform.localRotation = Quaternion.Lerp(module.transform.localRotation, targetRotation, snapSpeed * Time.deltaTime);
    }
}
