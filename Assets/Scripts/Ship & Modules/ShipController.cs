using System.Collections;
using UnityEngine;
using UnityEngine.UI;

///
/// ShipController.cs
///
/// Author: Eric Thompson (Dead Battery Games)
/// Purpose: Handles communication between connected modules and mode switching flight modes
///

public class ShipController : MonoBehaviour {

    // Public
    public bool control = false;
    public ParticleSystem spaceParticles;
    public LayerMask spawnCheckMask;
    public Transform playerExit;

    public Text speedometer;

    [HideInInspector] public Boosters boosters = null;
    [HideInInspector] public Thrusters thrusters = null;
    [HideInInspector] public FuelTank fuelTank = null;
    [HideInInspector] public AssistModule assistModule = null;
    [HideInInspector] public QuantumDrive quantumDrive = null;
    [HideInInspector] public LandingGear landingGear = null;
    [HideInInspector] public Lights lights = null;

    [HideInInspector] public GameObject[] moduleSlots;

    [HideInInspector] public Rigidbody shipRB;
    [HideInInspector] public GravBody shipGB;

	// Private
	[SerializeField] private float powerOnTime = 2f;
    [SerializeField] private float projectileCleanDelay = 5f;
    
    private ShipCam shipCamJib;
    private bool powered = false;

    private bool firstEntry = true;
    private FlightHelp flightHelp;

	void Start () {
        shipRB = GetComponent<Rigidbody>();
        shipGB = GetComponent<GravBody>();
        if (playerExit == null) Debug.LogError("ShipController: No player exit; set in the inspector");

        shipCamJib = GetComponentInChildren<ShipCam>();
        flightHelp = FindObjectOfType<FlightHelp>();

        if (!control) Exit(false);
        else Enter();

        moduleSlots = GameObject.FindGameObjectsWithTag("ModuleSlot");
        foreach (GameObject slot in moduleSlots) {
            slot.SetActive(false);
        }

        speedometer.enabled = false;

        RefreshModules();
	}

    void Update() {
        if (Input.GetButtonDown("ShipPower") && control) {
            if (powered) MainPower(false);
            else MainPower(true);
        }

        if (powered) {
            UpdateSpeedometer();
        }

        if (Input.GetButtonDown("EnterExitShip") && control && !powered) Exit();
    }

    public void RefreshModules() {
		boosters = GetComponentInChildren<Boosters>();
		thrusters = GetComponentInChildren<Thrusters>();
		fuelTank = GetComponentInChildren<FuelTank>();
        assistModule = GetComponentInChildren<AssistModule>();
        quantumDrive = GetComponentInChildren<QuantumDrive>();
        landingGear = GetComponentInChildren<LandingGear>();
        lights = GetComponentInChildren<Lights>();
    }

    public void DetachAllPhysical() {
        if (boosters) boosters.DisconnectShip();
        if (thrusters) thrusters.DisconnectShip();
        if (fuelTank) fuelTank.DisconnectShip();
        if (assistModule) assistModule.DisconnectShip();
        if (quantumDrive) quantumDrive.DisconnectShip();
    }

    // Toggles power for every component connected to the ship
	public void MainPower(bool toggle) {
		RefreshModules();

		if (toggle) {
            StopCoroutine("PowerOn"); StartCoroutine(PowerOn(powerOnTime));
		} else {
            if (!quantumDrive || (quantumDrive && quantumDrive.GetCurrentState() == QuantumDrive.QuantumState.Idle)) {
                if (thrusters) thrusters.Deactivate();
                if (boosters) boosters.Deactivate();
                if (assistModule) assistModule.Deactivate();
                if (quantumDrive) quantumDrive.Deactivate();
                if (fuelTank) fuelTank.Deactivate();

                landingGear.Deactivate();
                lights.Deactivate();

                speedometer.enabled = false;

                powered = false;
                Debug.Log("Ship Power Off; All connected modules offline");
            }
        }
	}

    public bool IsPowered() {
        return powered;
    }

    public void Enter() {
        // Enable controls
        control = true;
        shipCamJib.SetControl(true);

        // Show help screen for first-time entry
        if (firstEntry) {
            flightHelp.ToggleShow(true);
            firstEntry = false;
        }

        // Change camera background colour
        Camera shipCam = shipCamJib.GetComponentInChildren<Camera>();
        Sun sun = FindObjectOfType<Sun>();
        shipCam.backgroundColor = sun.GetSkyColour(transform.position);
        sun.ChangeCamera(shipCam);

        // Enable scene shift
        shipCamJib.GetComponentInChildren<SceneShift>().enabled = true;

        // Enable ambient particles
        spaceParticles.Play();

        // Clean up bullets
        StopCoroutine("CleanProjectiles"); StartCoroutine(CleanProjectiles(projectileCleanDelay));
    }

    public void Exit(bool spawnPlayer = true) {
        if (Physics.Linecast(transform.position, playerExit.position, spawnCheckMask)) {
            Debug.LogWarning("Stuck!");
        } else {
            // Disable controls
            control = false;
            shipCamJib.SetControl(false);

            // Disable scene shift
            shipCamJib.GetComponentInChildren<SceneShift>().enabled = false;

            // Disable ambient particles
            spaceParticles.Stop();
            spaceParticles.Clear();

            // Spawn player
            if (spawnPlayer) {
                FindObjectOfType<Spawner>().SpawnPlayer(playerExit);
                Rigidbody playerRB = FindObjectOfType<PlayerControl>().GetComponent<Rigidbody>();
                playerRB.velocity = shipRB.velocity;
            }

            flightHelp.ToggleShow(false, true);
        }
    }

    void UpdateSpeedometer() {
        float speed;
        string formattedSpeed;

        if (shipRB.velocity.magnitude > 999.5f) {
            speed = shipRB.velocity.magnitude / 1000f;
            formattedSpeed = speed.ToString("F1") + " km/s";
        } else {
            speed = shipRB.velocity.magnitude;
            formattedSpeed = Mathf.RoundToInt(speed) + " m/s";
        }

        if (transform.InverseTransformDirection(shipRB.velocity).z < -0.5f) {
            speedometer.text = "-" + formattedSpeed;

        } else {
            speedometer.text = formattedSpeed;
        }
    }

    // Ship reboot cooldown
    IEnumerator PowerOn(float time) {
        Debug.Log("Power On in " + time + " seconds...");
        yield return new WaitForSeconds(time);

        if (thrusters) thrusters.Activate();
        if (boosters) boosters.Activate();
		if (boosters && assistModule) assistModule.Activate();
        if (quantumDrive) quantumDrive.Activate();
        if (fuelTank) fuelTank.Activate();

        landingGear.Activate();
        lights.Activate();

        speedometer.enabled = true;

        powered = true;

		Debug.Log("Ship Power On; All connected modules online");
	}

    IEnumerator CleanProjectiles(float delay) {
        Bullet[] bullets = FindObjectsOfType<Bullet>();

        yield return new WaitForSeconds(delay);

        foreach (Bullet bullet in bullets) {
            if (bullet) Destroy(bullet.gameObject);
        }
    }
}
