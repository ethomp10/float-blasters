using System.Collections;
using UnityEngine;

///
/// HealthOrb.cs
///
/// Author: Eric Thompson (Dead Battery Games)
/// Purpose: Health pick-up script
///

public class HealthOrb : MonoBehaviour {

    // Public
    public float healAmount = 10f;
    public float lifeSpan = 10f;

    // Private
    private MeshRenderer orb;
    private bool dying;

	// Use this for initialization
	void Start () {
        orb = GetComponentInChildren<MeshRenderer>();
        StartCoroutine("LifeSpan");
	}
	
	// Update is called once per frame
	void Update () {
        float orbScale;

        if (!dying) {
            orbScale = (Mathf.Sin(3f * Time.time) / 8f) + 0.5f;
            orb.transform.localScale = new Vector3(orbScale, orbScale, orbScale);
        } else {
            orbScale = Mathf.Lerp(orb.transform.localScale.x, 0f, 3f * Time.deltaTime);
        }

        orb.transform.localScale = new Vector3(orbScale, orbScale, orbScale);

    }

    void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            other.GetComponent<PlayerControl>().HealPlayer(healAmount);
            StopCoroutine("LifeSpan");
            Destroy(gameObject);
        }
    }

    IEnumerator LifeSpan() {
        yield return new WaitForSeconds(lifeSpan);

        dying = true;
        GetComponentInChildren<ParticleSystem>().Stop();
        yield return new WaitForSeconds(1f);

        Destroy(gameObject);
    }
}
