using UnityEngine;

///
/// Bullet.cs
///
/// Author: Eric Thompson (Dead Battery Games)
/// Purpose: Handles bullet behaviour & collision
///

public class Bullet : MonoBehaviour {

    // Public
    public enum BulletType { Pulse, Fire, Ice, Gel }

    public BulletType type; // Set in inspector to match prefab
    public float baseDamage = 20f;

    // Private
    private ParticleSystem explosion;
    private MeshRenderer bulletMesh;
    private Collider bulletCollider;
    private Rigidbody bulletRB;

    void Start() {
        explosion = GetComponentInChildren<ParticleSystem>();
        bulletMesh = GetComponentInChildren<MeshRenderer>();
        bulletCollider = GetComponentInChildren<Collider>();
        bulletRB = GetComponent<Rigidbody>();
    }

    void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.tag != ("Player") && !collision.collider.CompareTag("Shield")) {
            Explode();
        }

        if (collision.collider.CompareTag("Shield")) {
            collision.collider.GetComponent<Shield>().Impact(this);
        }

        if (collision.collider.CompareTag("Enemy") && collision.gameObject.GetComponent<GorbonAI>()) {
            float damage = baseDamage;
            if (type != BulletType.Pulse) damage /= 10f;
            collision.gameObject.GetComponent<GorbonAI>().DamageGorbon(damage);
        }
    }

    // Explosion particle effects on bullet contact. Also hides mesh and disables collider for recycling purposes.
    public void Explode() {
        bulletRB.velocity = Vector3.zero;
        bulletRB.angularVelocity = Vector3.zero;
        bulletMesh.enabled = false;
        bulletCollider.enabled = false;
        explosion.gameObject.SetActive(true);
        explosion.Play();
    }

    // Resets bullet to the gun's firepoint. 
    public void ResetBullet(Transform resetPosition, Vector3 resetVelocity) {
        explosion.gameObject.SetActive(false);
        transform.position = resetPosition.position;
        bulletMesh.enabled = true;
        bulletCollider.enabled = true;
        bulletRB.velocity = resetVelocity;
    }
}
