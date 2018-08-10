using UnityEngine;

///
/// GroundCheck.cs
///
/// Author: Eric Thompson (Dead Battery Games)
/// Purpose: Checks for collision at feet of player
///

[RequireComponent(typeof(SphereCollider))]

public class GroundCheck : MonoBehaviour {

    // Private
    private PlayerControl playerControl;
    private int collisionCount = 0;

    void Start() {
        playerControl = GetComponentInParent<PlayerControl>();
    }

    void OnTriggerEnter(Collider other) {
        collisionCount++;
        if (collisionCount > 0) {
            playerControl.SetGrounded(true);
        }
    }

    void OnTriggerExit(Collider other) {
        collisionCount--;
        if (collisionCount <= 0) {
            playerControl.SetGrounded(false);
            if (collisionCount < 0) collisionCount = 0;
        }
    }
}
