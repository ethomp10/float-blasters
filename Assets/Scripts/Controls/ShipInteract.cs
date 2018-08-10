using UnityEngine;

///
/// ShipInteract.cs
///
/// Author: Eric Thompson (Dead Battery Games)
/// Purpose: Listener for player proximity
///

[RequireComponent(typeof(SphereCollider))]

public class ShipInteract : MonoBehaviour {

    void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            other.GetComponentInParent<PlayerInteract>().SetNearShip(true);
        }
    }

    void OnTriggerExit(Collider other) {
        if (other.CompareTag("Player")) {
            other.GetComponentInParent<PlayerInteract>().SetNearShip(false);
        }
    }
}
