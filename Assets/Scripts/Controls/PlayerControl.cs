using System.Collections;
using UnityEngine;
using UnityEngine.UI;

///
/// PlayerControl.cs
///
/// Author: Eric Thompson (Dead Battery Games)
/// Purpose: Allows player movement and viewmodel animations
///

[RequireComponent(typeof(Rigidbody))]
//[RequireComponent(typeof(CapsuleCollider))]

public class PlayerControl : MonoBehaviour {

    // Public
    public MultiCannon multiCannon;
    public GravClaw gravClaw;

    [HideInInspector] public bool canSprint = true;
    [HideInInspector] public Image crosshair;
    [HideInInspector] public CanvasGroup hitSplash;
    [HideInInspector] public CanvasGroup healSplash;

    // Private
    [SerializeField] private float baseSpeed = 6.0f;
    [SerializeField] private float maxMoveForce = 1.0f;
    [SerializeField] private float jumpForce = 10.0f;
    [SerializeField] private float airSpeed = 0.5f;
    [SerializeField] private Transform armPivot;
    [SerializeField] private float sprintPivotAngle = 20;
    [SerializeField] private float hidePivotAngle = 35f;

    private Rigidbody playerRB;

    private float maxHealth;
    private float currentHealth;
    private Text heathText;
    private Image healthFill;
    private Image healthHit;
    private bool hitDrain = false;

    private float currentSpeed;
    private bool grounded = false;

    private float horizontal;
    private float vertical;
    private Vector3 pivotPos;
    private float currentTime = 0f;
    private bool hideGun;

    void Start() {
        playerRB = GetComponent<Rigidbody>();

        currentSpeed = baseSpeed;

        hitSplash = GameObject.Find("HitSplash").GetComponent<CanvasGroup>();
        healSplash = GameObject.Find("HealSplash").GetComponent<CanvasGroup>();
        heathText = GameObject.Find("HealthText").GetComponent<Text>();
        healthFill = GameObject.Find("HealthFill").GetComponent<Image>();
        healthHit = GameObject.Find("HealthHit").GetComponent<Image>();
        crosshair = GameObject.Find("Crosshair").GetComponent<Image>();

        maxHealth = PlayerStats.maxPlayerHealth;
        currentHealth = PlayerStats.currentPlayerHealth;

        heathText.text = currentHealth.ToString("F0");
        healthFill.fillAmount = currentHealth / maxHealth;
        healthHit.fillAmount = healthFill.fillAmount;

        pivotPos = armPivot.localPosition;
        armPivot.localEulerAngles = new Vector3 (90f, 45f, 0f);

        if (multiCannon.isActiveAndEnabled && gravClaw.isActiveAndEnabled) {
            gravClaw.gameObject.SetActive(false);
        }
    }

    void Update() {
        // Jump
        if (grounded && Input.GetButtonDown("Jump")) {
            playerRB.AddForce(transform.up * jumpForce, ForceMode.VelocityChange);
        }

        // Sprint
        if (grounded && Input.GetButton("Sprint") && vertical > 0f && canSprint) {
            currentSpeed = baseSpeed * 2f;
            if (multiCannon.isActiveAndEnabled) {
                multiCannon.DisableScope();
                multiCannon.canFire = false;
            } else if (gravClaw.isActiveAndEnabled) {
                gravClaw.canGrab = false;
            }
            crosshair.enabled = false;
            if (!hideGun) LowerWeapon(sprintPivotAngle);
        } else {
            if (!hideGun) {
                if (multiCannon.isActiveAndEnabled) {
                    multiCannon.canFire = true;
                    crosshair.enabled = true;
                } else if (gravClaw.isActiveAndEnabled && !gravClaw.holdingModule) {
                    gravClaw.canGrab = true;
                    crosshair.enabled = true;
                }
                if (armPivot.localRotation != Quaternion.identity && armPivot.localEulerAngles.x < 180f) RaiseWeapon();
            }
            currentSpeed = baseSpeed;
        }

        // Wobble
        if (grounded && (horizontal != 0 || vertical != 0) && !hideGun) {
            if (Input.GetButton("Sprint") && vertical > 0f && canSprint) ArmWobble(1.5f);
            else ArmWobble();
        }

        // Hide weapon
        if (hideGun) {
            if (gravClaw.isActiveAndEnabled && !gravClaw.holdingModule) {
                gravClaw.canGrab = false;
                crosshair.enabled = false;
                LowerWeapon(hidePivotAngle);
            } else if (multiCannon.isActiveAndEnabled) {
                LowerWeapon(hidePivotAngle);
                crosshair.enabled = false;
            }
        }

        // Weapon types
        if (Input.GetButtonDown("PulseBeam")
        || Input.GetButtonDown("FlameThrower")
        ||  Input.GetButtonDown("IceBreaker")
        || Input.GetButtonDown("GelCaster")) {
            SwitchWeaponTypes();
        }


        // GravClaw
        if (Input.GetButtonDown("GravClaw")) {
            if (gravClaw.isActiveAndEnabled && !gravClaw.holdingModule) {
                SwitchTool(0);
            } else if (multiCannon.isActiveAndEnabled) {
                SwitchTool(1);
            }
        }

        // Reset weapon
        if (!grounded || (horizontal == 0 && vertical == 0)) {
            if (armPivot.localPosition != pivotPos) ResetWeapon();
        }

        if (healthHit.fillAmount != healthFill.fillAmount && hitDrain) {
            healthHit.fillAmount -= 0.5f * Time.deltaTime;
            if (healthHit.fillAmount < healthFill.fillAmount) {
                healthHit.fillAmount = healthFill.fillAmount;
            }
        }

        if (hitSplash.alpha > 0) {
            hitSplash.alpha -= Time.deltaTime;
        }
        if (healSplash.alpha > 0) {
            healSplash.alpha -= Time.deltaTime;
        }

    }

    void SwitchTool(int tool) {
        if (tool == 0) {
            gravClaw.gameObject.SetActive(false);
            multiCannon.gameObject.SetActive(true);
            armPivot.localEulerAngles = new Vector3(90f, 45f, 0f);
            FindObjectOfType<GunHide>().MoveTrigger(0);
        } else if (tool == 1) {
            multiCannon.DisableScope();
            gravClaw.gameObject.SetActive(true);
            multiCannon.gameObject.SetActive(false);
            FindObjectOfType<GunHide>().MoveTrigger(1);
            armPivot.localEulerAngles = new Vector3(90f, 45f, 0f);
        }
    }

    void SwitchWeaponTypes() {
        if (!multiCannon.isActiveAndEnabled) SwitchTool(0);

        // Types
        if (Input.GetButtonDown("PulseBeam")) {
            multiCannon.bulletType = Bullet.BulletType.Pulse;
        } else if (Input.GetButtonDown("FlameThrower")) {
            multiCannon.bulletType = Bullet.BulletType.Fire;
        } else if (Input.GetButtonDown("IceBreaker")) {
            multiCannon.bulletType = Bullet.BulletType.Ice;
        } else if (Input.GetButtonDown("GelCaster")) {
            multiCannon.bulletType = Bullet.BulletType.Gel;
        }
    }

    void FixedUpdate() {
        // Calculate target speed
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
        Vector3 targetVelocity = (transform.right * horizontal + transform.forward * vertical).normalized;
        targetVelocity *= currentSpeed;

        // Calculate force vector
        Vector3 velocity = playerRB.velocity;
        Vector3 moveForce = (targetVelocity - velocity);
        moveForce = Vector3.ClampMagnitude(moveForce, maxMoveForce);

        if (grounded) playerRB.AddForce(moveForce, ForceMode.VelocityChange);
        else playerRB.AddForce(targetVelocity * airSpeed);
    }

    public void SetGrounded(bool set) {
        grounded = set;
    }

    public void DamagePlayer(float damage) {
        currentHealth -= damage;
        hitSplash.alpha = 0.5f;

        if (currentHealth <= 0f) {
            

            StopCoroutine("KillPlayer"); StartCoroutine("KillPlayer");

            enabled = false;
        }

        StopCoroutine("UpdateHealthBar"); StartCoroutine("UpdateHealthBar");
    }

    public void HealPlayer(float amount) {
        currentHealth += amount;

        if (currentHealth > maxHealth) {
            currentHealth = maxHealth;
        }

        healSplash.alpha = 0.5f;

        StopCoroutine("UpdateHealthBar");
        healthFill.fillAmount = currentHealth / maxHealth;
        healthHit.fillAmount = healthFill.fillAmount;
        heathText.text = Mathf.CeilToInt(currentHealth).ToString();
    }

    public float GetCurrentHealth() {
        return currentHealth;
    }

    IEnumerator UpdateHealthBar() {
        healthFill.fillAmount = currentHealth / maxHealth;
        heathText.text = Mathf.CeilToInt(currentHealth).ToString();

        hitDrain = false;
        yield return new WaitForSeconds(1f);
        hitDrain = true;
    }

    IEnumerator KillPlayer() {
        currentHealth = 0f;
        GameObject.Find("HealthBar").GetComponent<CanvasGroup>().alpha = 0f;
        crosshair.enabled = false;

        FindObjectOfType<MouseLook>().enabled = false;
        GetComponent<PlanetSnap>().enabled = false;

        if (multiCannon) Destroy(multiCannon.transform.gameObject);
        if (gravClaw) Destroy(gravClaw.transform.gameObject);

        playerRB.freezeRotation = false;
        playerRB.drag = 0f;

        yield return new WaitForSeconds(2f);

        playerRB.drag = 10f;

        yield return new WaitForSeconds(1f);

        while (hitSplash.alpha < 1f) {
            yield return new WaitForEndOfFrame();
            hitSplash.alpha += 0.5f * Time.deltaTime;
        }
        FindObjectOfType<MenuManager>().RestartLevel();
    }

    public void HideGun(bool set) {
        if (set) {
            hideGun = true;
            multiCannon.canFire = false;
            crosshair.enabled = false;
        } else {
            hideGun = false;
        }
    }

    // Adds wobble effect to weapon & arms
    void ArmWobble(float speedMult = 1f) {
        Vector3 newPos = new Vector3(0.01f * speedMult * Mathf.Sin(currentTime * 6f * speedMult) + pivotPos.x, -0.02f  * speedMult * Mathf.Sin(currentTime * 12f * speedMult) + pivotPos.y, pivotPos.z);
        armPivot.localPosition = newPos;
        currentTime += Time.deltaTime;
    }

    // Animation functions
    void LowerWeapon(float angle) {
        armPivot.localRotation = Quaternion.Lerp(armPivot.localRotation, Quaternion.Euler(angle, angle, 0f), 8f * Time.deltaTime);
    }

    // Resets weapon rotation
    void RaiseWeapon() {
        armPivot.localRotation = Quaternion.Lerp(armPivot.localRotation, Quaternion.identity, 8f * Time.deltaTime);
    }

    // Resets weapon position
    void ResetWeapon() {
        currentTime = 0f;
        armPivot.localPosition = Vector3.Lerp (armPivot.localPosition, pivotPos, 8f * Time.deltaTime);
    }
}
