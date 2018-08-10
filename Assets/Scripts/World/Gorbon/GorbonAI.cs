using System.Collections;
using UnityEngine;

///
/// GorbonAI.cs
///
/// Author: Eric Thompson (Dead Battery Games)
/// Purpose: Controls Gorbon bombs
///

public class GorbonAI : MonoBehaviour {

    // Public
    public float moveForce = 15f;
    public float patrolSpeed = 2f;
    public float lockedSpeed = 5f;
    public float jumpForce = 10f;
    public float attackRange = 8f;
    public float attackForce = 200f;
    public float attackDamage = 10f;
    public float attackRadius = 10f;
    public float visibility = 20f;
    public float maxHealth = 100f;
    public bool canSpawnShield = false;

    public Transform lights;
    public GameObject[] shieldPrefabs;
    public Material[] lightMaterials;
    public Material onMaterial;
    public Material offMaterial;
    public LayerMask visibilityMask;
    public ParticleSystem explosion;
    public GameObject healthOrb;
    public Rigidbody gorbonRB;

    [HideInInspector] public bool shieldDeployed = false;

    // Private
    private Transform target = null;

    private bool targetLocked = false;
    private bool grounded = false;
    private bool attacking = false;
    private float distanceToTarget;

    private float currentHealth;
    private GorbonOverlord overlord;

    void Start() {
        gorbonRB = GetComponent<Rigidbody>();
        overlord = GameObject.Find("GameManager").gameObject.GetComponent<GorbonOverlord>();
        if(overlord != null) overlord.AddGorbon(this);
        SetShieldLights(onMaterial);
        currentHealth = maxHealth;
        jumpForce = 10f;
    }

    void FixedUpdate() {
        if(overlord == null) GorbonUpdate(Vector3.zero);
    }

    public void GorbonUpdate(Vector3 flockingVector) {
        if (target) {
            distanceToTarget = (target.position - transform.position).magnitude;
        }

        Move(flockingVector);

        // if (target == null) {
        //     target = FindPlayer();
        // } else {
        //     if (!targetLocked && distanceToTarget < visibility) Lock();
        //     if (targetLocked) Seek(target.position);
        //     if (targetLocked && distanceToTarget < attackRange && !attacking) {
        //         StartCoroutine(Attack());
        //         attacking = true;
        //     }
        // }
    }

    void OnCollisionStay(Collision collision) {
        grounded = true;
    }

    void OnCollisionExit(Collision collision) {
        grounded = false;
    }


    Transform FindPlayer() {
        if (GameObject.FindGameObjectWithTag("Player")) {
            return FindObjectOfType<PlayerControl>().transform;
        } else {
            return null;
        }
    }

    public void TargetPlayer() {
        target = FindPlayer();
    }

    void Lock() {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, (target.position - transform.position).normalized, out hit, visibility, visibilityMask)) {
            if (hit.transform.CompareTag(target.tag)) {
                targetLocked = true;
                if (!shieldDeployed && canSpawnShield) DeployShield();
            }
        }
    }

    Vector3 Seek(Vector3 targetPos) {
        if (grounded) {
            return (targetPos - transform.position).normalized;
            // gorbonRB.AddForce(forceDir * moveForce, ForceMode.Acceleration);

            // if (gorbonRB.velocity.magnitude > maxSpeed) {
            //     gorbonRB.velocity = Vector3.Lerp(gorbonRB.velocity, gorbonRB.velocity.normalized * maxSpeed, 5f * Time.fixedDeltaTime);
            // }
        }
        return Vector3.zero;
    }

    void Move(Vector3 flockingVector){
        //Move towards player
        if (target == null) {
            target = FindPlayer();
        } else {
            if (!targetLocked && distanceToTarget < visibility) Lock();
            // if (targetLocked) gorbonRB.velocity += Seek(target.position) * Time.deltaTime;
            if (targetLocked && !attacking) gorbonRB.AddForce(Seek(target.position) * moveForce, ForceMode.Acceleration);
            if (targetLocked && distanceToTarget < attackRange && !attacking) {
                StartCoroutine(Attack());
                attacking = true;
            }
        }

        if(grounded){
            flockingVector = flockingVector.normalized;
            //gorbonRB.velocity += flockingVector * Time.deltaTime;
            gorbonRB.AddForce(flockingVector * moveForce, ForceMode.Acceleration);

            float maxSpeed = targetLocked ? lockedSpeed : patrolSpeed;

            //Normalize velocity
            if (gorbonRB.velocity.magnitude > maxSpeed) {
                gorbonRB.velocity = Vector3.Lerp(gorbonRB.velocity, gorbonRB.velocity.normalized * maxSpeed, (gorbonRB.velocity.magnitude - maxSpeed) * Time.fixedDeltaTime);
                //gorbonRB.velocity = gorbonRB.velocity.normalized * maxSpeed;
            }
        }
    }

    void DeployShield() {
        int shieldType = Random.Range(0, 3);

        Instantiate(shieldPrefabs[shieldType], transform);
        SetShieldLights(lightMaterials[shieldType]);
        gameObject.layer = LayerMask.NameToLayer("ShieldedGorbon");

        shieldDeployed = true;
    }

    void SetShieldLights(Material material) {
        foreach (MeshRenderer light in lights.GetComponentsInChildren<MeshRenderer>()) {
            light.material = material;
        }
    }

    public void DisableShield() {
        gameObject.layer = LayerMask.NameToLayer("Default");
        SetShieldLights(onMaterial);
    }

    public void DisableGorbon() {
        Debug.Log(Instantiate(healthOrb, transform.position, Quaternion.identity));
        
        SetShieldLights(offMaterial);
        if(overlord != null) overlord.RemoveGorbon(this);
        Destroy(this);
    }

    public void DamageGorbon(float damage) {
        currentHealth -= damage;
        if (currentHealth <= 0) {
            DisableGorbon();
        } else {
            targetLocked = true;
            if (!shieldDeployed && canSpawnShield) DeployShield();
        }
    }

    void Jump() {
        //gorbonRB.velocity = Vector3.zero;
        grounded = false;
        gorbonRB.AddForce(target.transform.up * jumpForce, ForceMode.VelocityChange);
    }

    IEnumerator Attack() {
        Jump();


        yield return new WaitForSeconds(1f);

        Vector3 explosionPos = transform.position;
        gorbonRB.isKinematic = true;
        gorbonRB.velocity = Vector3.zero;
        transform.position = explosionPos;

        Collider[] colliders = Physics.OverlapSphere(explosionPos, attackRadius);
        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();

            if (rb != null) {
                rb.AddExplosionForce(attackForce, explosionPos, attackRadius);

                RaycastHit attackHit;
                if (rb.CompareTag("Player") && Physics.Linecast(transform.position, target.position, out attackHit, visibilityMask)) {
                    if (attackHit.transform.CompareTag("Player")) {
                        float realDamage = attackDamage - 3f * (target.position - transform.position).magnitude;
                        realDamage = Mathf.Clamp(realDamage, 0f, attackDamage);
                        rb.GetComponent<PlayerControl>().DamagePlayer(realDamage);
                        Debug.LogWarning(attackDamage + " | " + (3f * (target.position - transform.position).magnitude) + " | " + realDamage);
                    }
                }

                if (rb.CompareTag("PlayerShip")) {
                    rb.GetComponent<ShipCollision>().BreakShip();
                    if (target == rb.transform) {
                        FindObjectOfType<ShipController>().Exit();
                    }
                }
            }
        }

        MeshRenderer[] meshes = GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer mesh in meshes) {
            mesh.enabled = false;
        }

        explosion.Play();

        yield return new WaitForSeconds(1f);

        if(overlord != null){
            overlord.RemoveGorbon(this);
            overlord = null;
        }

        Destroy(gameObject);
    }
}
