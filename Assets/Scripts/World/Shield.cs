using System.Collections;
using UnityEngine;

public class Shield : MonoBehaviour {

    public Bullet.BulletType type;
    public float maxHealth = 150f;
    public float startHealth = 100f;
    public float deploySpeed = 5f;

    private float currentHealth;
    private MeshRenderer meshRenderer;
    private ParticleSystem explosion;

	// Use this for initialization
	void Start () {
        currentHealth = startHealth;
        meshRenderer = GetComponent<MeshRenderer>();
        explosion = GetComponentInChildren<ParticleSystem>();
	}
	
	// Update is called once per frame
	void Update () {
        float shieldScale = currentHealth / 100f + 1.2f;

        if (transform.localScale.x != shieldScale) {
            transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(shieldScale, shieldScale, shieldScale), deploySpeed * Time.deltaTime);
        }
	}

    public void Impact(Bullet bullet) {
        if (type == bullet.type) {
            bullet.Explode();
            currentHealth += bullet.baseDamage;
        } else if ((type == Bullet.BulletType.Fire && bullet.type == Bullet.BulletType.Gel)
        || (type == Bullet.BulletType.Ice && bullet.type == Bullet.BulletType.Fire)
        || (type == Bullet.BulletType.Gel && bullet.type == Bullet.BulletType.Ice)) {
            bullet.Explode();

            currentHealth -= bullet.baseDamage;
            currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

            if (currentHealth == 0) {
                if (GetComponentInParent<GorbonAI>()) GetComponentInParent<GorbonAI>().DisableShield();
                StartCoroutine(DestroyShield());
            }
        }

        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
        if (currentHealth == 0) {
            StartCoroutine(DestroyShield());
        }
    }

    IEnumerator DestroyShield() {
        GetComponent<Collider>().enabled = false;
        meshRenderer.enabled = false;
        transform.parent = null;
        explosion.Play();
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }
}
