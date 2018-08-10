using UnityEngine;

///
/// GravBody.cs
///
/// Author: Eric Thompson (Dead Battery Games)
/// Purpose: Gravitates towards any GravSources in the world
///

[RequireComponent(typeof(Rigidbody))]

public class GravBody : MonoBehaviour {

    private GravSource[] sources;

    void Start() {
        sources = FindObjectsOfType<GravSource>();
    }

    void FixedUpdate() {
        // Call Pull function in each GravSource
        foreach (GravSource source in sources) {
            if (source != GetComponent<GravSource>()) {
                source.Pull(transform);
            }
        }
    }
}
