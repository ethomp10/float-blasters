using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

///
/// MultiCannon.cs
///
/// Author: Eric Thompson (Dead Battery Games)
/// Purpose: Script for main player weapon
///

public class MultiCannon : MonoBehaviour {

    // Public
    public Transform firePoint;

    [HideInInspector] public Bullet.BulletType bulletType = Bullet.BulletType.Pulse;
    public GameObject[] bulletPrefabs;
    public Image typeScreen;
    public Sprite[] typeIcons;

    public Transform armPivot;
    public float zoomSpeed = 5f;
    public float zoomFOV = 20f;
    public LayerMask layerMask;

    [HideInInspector] public bool canFire = false;

    public ParticleSystem[] muzzleFlashes;
    public Light flashlightGlow;

    // Private
    [SerializeField] private float[] bulletForces = { 100f, 20f, 30f, 50f };
    [SerializeField] private float[] fireRates = { 500f, 120f, 80f, 250f };
    [SerializeField] private float aimRange = 100f;
    [SerializeField] private int maxBullets = 10;
    [SerializeField] private float recoilOffset = 0.1f;
    [SerializeField] private float recoilAngleOffset = 4f;

    [SerializeField] private Material flashlightOnMaterial;
    [SerializeField] private Material flashlightOffMaterial;
    private MeshRenderer flashlight;
    private bool flashlightOn = false;

    private Camera playerCam;
    private Rigidbody playerRB;
    //private AudioSource speaker;
    private int[] bulletIndicies = { 0, 0, 0, 0 };

    private List<GameObject>[] bulletLists = new List<GameObject>[4];

    private bool cooldown = false;
    private bool recoil = false;
    private bool recoilReturn = false;
    private Vector3 gunPos;
    private Vector3 armPos;

    private MouseLook mouseLook;
    private bool zoom = false;
    private float camFOV;
    private float weaponOffset;
    private CanvasGroup scopeUI;

    void Start() {
        mouseLook = GetComponentInParent<MouseLook>();
        playerCam = GetComponentInParent<Camera>();
        scopeUI = GameObject.Find("Scope").GetComponent<CanvasGroup>();
        scopeUI.alpha = 0f;
        weaponOffset = transform.localPosition.x;

        for (int i = 0; i < bulletLists.Length; i++) {
            bulletLists[i] = new List<GameObject>();
        }

        playerRB = GetComponentInParent<Rigidbody>();
        if (playerRB && playerRB.CompareTag("Player")) canFire = true;

        //speaker = GetComponent<AudioSource>();

        flashlight = flashlightGlow.transform.parent.GetComponent<MeshRenderer>();

        // Save gun & arm positions
        gunPos = transform.localPosition;

        // Save camera's FOV
        camFOV = playerCam.fieldOfView;
    }

    void Update() {
        if (Input.GetButtonDown("Fire") && canFire && !cooldown) {
            Fire();
        }

        if (Input.GetButtonDown("Scope") && canFire) {
            if (!zoom) {
                EnableScope();
            } else DisableScope();
        }

        if (Input.GetButtonDown("Light")) {
            if (flashlightOn) {
                flashlight.material = flashlightOffMaterial;
                flashlightGlow.enabled = false;
                flashlightOn = false;
            } else {
                flashlight.material = flashlightOnMaterial;
                flashlightGlow.enabled = true;
                flashlightOn = true;
            }
        }

        SwitchWeapon();

        if (recoil && !zoom) {
            Recoil();
        }

        if (zoom) {
            Zoom();
        }
    }

    void SwitchWeapon() {
        // Switch Bullet Type
        if (Input.GetButtonDown("PulseBeam")) {
            bulletType = Bullet.BulletType.Pulse;
        } else if (Input.GetButtonDown("FlameThrower")) {
            bulletType = Bullet.BulletType.Fire;
        } else if (Input.GetButtonDown("IceBreaker")) {
            bulletType = Bullet.BulletType.Ice;
        } else if (Input.GetButtonDown("GelCaster")) {
            bulletType = Bullet.BulletType.Gel;
        }

        float input = Input.GetAxisRaw("Mouse ScrollWheel");
        if (input > 0) {
            bulletType++;
            if ((int)bulletType > 3) bulletType = Bullet.BulletType.Pulse;
        } else if (input < 0) {
            bulletType--;
            if ((int)bulletType < 0) bulletType = Bullet.BulletType.Gel;
        }

        typeScreen.sprite = typeIcons[(int)bulletType];
    }

    void Fire() {
        StartCoroutine(Cooldown(60f / fireRates[(int)bulletType]));

        Vector3 aimPoint = playerCam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0f));
        Vector3 fireDirection;
        RaycastHit hit;
        // Check if gun is aiming forward
        if (armPivot.localEulerAngles.x > 180 || armPivot.localEulerAngles.x < 1f) {
            // Cast a ray from the crosshair and set gun's fire direction based on ray
            if (Physics.Raycast(aimPoint, playerCam.transform.forward, out hit, aimRange, layerMask)) {
                fireDirection = (hit.point - firePoint.position);
                if (fireDirection.magnitude > 1) {
                    fireDirection.Normalize();
                } else {
                    fireDirection = transform.forward;
                }
            } else {
                fireDirection = playerCam.transform.forward;
            }
        } else {
            fireDirection = transform.forward;
        }

        RecycleBullets(bulletType, fireDirection);

        // Effects
        //speaker.play();
        if (bulletType == Bullet.BulletType.Fire) {
            muzzleFlashes[(int)bulletType].Emit(100);
        } else if (bulletType == Bullet.BulletType.Gel) {
            muzzleFlashes[(int)bulletType].Emit(10);
        } else {
            muzzleFlashes[(int)bulletType].Emit(50);
        }

        if (!zoom) {
            recoil = true;
            recoilReturn = false;
        } else {
            mouseLook.yOffset = recoilAngleOffset;
        }
    }

    void RecycleBullets(Bullet.BulletType type, Vector3 fireDirection) {
        GameObject current; // The current bullet
        int i = (int)type;

        // Bullet Recycling
        if (bulletLists[i].Count < maxBullets) {
            // If there are less than the max bullets, make more
            current = Instantiate(bulletPrefabs[i], firePoint.position, firePoint.rotation);
            if (bulletType == Bullet.BulletType.Fire) current.GetComponent<Rigidbody>().velocity = Vector3.zero;
            else current.GetComponent<Rigidbody>().velocity = playerRB.velocity;
            bulletLists[i].Add(current);
        } else {
            // If max bullets is exceted, reuse the oldest bullet
            current = bulletLists[i][bulletIndicies[i]];
            if (bulletType == Bullet.BulletType.Fire) current.GetComponent<Bullet>().ResetBullet(firePoint, Vector3.zero);
            else current.GetComponent<Bullet>().ResetBullet(firePoint, playerRB.velocity);
            CycleIndex(type);
        }

        // Add force to bullet based on fire direction
        current.GetComponent<Rigidbody>().AddForce(fireDirection * bulletForces[i], ForceMode.VelocityChange);
        Vector3 spin = new Vector3(Random.Range(-100, 100), Random.Range(-100, 100), Random.Range(-100, 100));
        current.GetComponent<Rigidbody>().angularVelocity = spin;
    }

    void EnableScope() {
        if (!zoom) {
            transform.Translate(0f, -1f, 0f);
            firePoint.transform.Translate(-weaponOffset, 1f, 0f);
            scopeUI.alpha = 1f;
            mouseLook.scopeSensitivity = true;
            zoom = true;
        }
    }

    public void DisableScope() {
        if (zoom) {
            scopeUI.alpha = 0f;
            transform.Translate(0f, 1f, 0f);
            firePoint.transform.Translate(weaponOffset, -1, 0f);
            playerCam.fieldOfView = camFOV;
            mouseLook.scopeSensitivity = false;
            zoom = false;
        }
    }

    void Zoom() {
        if (playerCam.fieldOfView != zoomFOV) {
            playerCam.fieldOfView = Mathf.Lerp(playerCam.fieldOfView, zoomFOV, zoomSpeed * Time.deltaTime);
        }
    }

    // This function makes sure the oldest bullet is used for recycling
    void CycleIndex(Bullet.BulletType type) {
        int i = (int)type;
        bulletIndicies[i]++;
        if (bulletIndicies[i] >= bulletLists[i].Count) {
            bulletIndicies[i] = 0;
        }
    }

    // Apply recoil to gun and arms
    void Recoil() {
        if (recoilReturn) {
            transform.localPosition = Vector3.Lerp(transform.localPosition, gunPos, 5f * Time.deltaTime);
            if (armPivot.localEulerAngles.x > 180f) {
                armPivot.localRotation = Quaternion.Lerp(armPivot.localRotation, Quaternion.identity, 5f * Time.deltaTime);
            }
        } else {
            transform.localPosition = Vector3.Lerp(transform.localPosition, gunPos - Vector3.forward * recoilOffset, 100f * Time.deltaTime);
            // Check if gun is aiming forward
            if (armPivot.localEulerAngles.x > 180 || armPivot.localEulerAngles.x < 1f) {
                armPivot.localRotation = Quaternion.Lerp(armPivot.localRotation, Quaternion.Euler(-recoilAngleOffset, 0f, 0f), 100f * Time.deltaTime);
            }
            if (transform.localPosition == gunPos - Vector3.forward * recoilOffset) {
                recoilReturn = true;
            }
        }

        if (transform.localPosition == gunPos) {
            recoil = false;
            recoilReturn = false;
        }
    }

    IEnumerator Cooldown(float time) {
        cooldown = true;
        yield return new WaitForSeconds(time);
        cooldown = false;
    }
}
