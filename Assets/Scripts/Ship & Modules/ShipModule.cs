using UnityEngine;

///
/// ShipComponent.cs
///
/// Author: Eric Thompson (Dead Battery Games)
/// Purpose: Generic script for any component that can be attached/detached from a ship
///

public class ShipModule : MonoBehaviour {


    // Protected
    [SerializeField] protected float mass = 5f;
    [SerializeField] protected Vector3 localShipPos;
    protected ShipController shipController = null;
    protected bool connected = false;
    protected bool active = false;

    protected virtual void Start() {
        if (transform.parent != null && transform.parent.CompareTag("PlayerShip")) {
            shipController = transform.parent.GetComponent<ShipController>();
            ConnectShip(transform.parent);
        }
    }

    protected virtual void FixedUpdate() {
        if (connected) {
            if (transform.localPosition != localShipPos) {
                transform.localPosition = Vector3.Lerp(transform.localPosition, localShipPos, 10f * Time.deltaTime);
            }
            if (transform.localRotation != Quaternion.identity) {
                transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.identity, 10f * Time.deltaTime);
            }
        }
    }

    // Connect component to player ship
    public virtual void ConnectShip(Transform ship) {
        // Change parent & orient with ship
        transform.SetParent(ship);

        // Disable rigidbody
        if (GetComponent<GravBody>()) Destroy(GetComponent<GravBody>());
        Rigidbody compRB = GetComponent<Rigidbody>();
        if (compRB) {
            Destroy(compRB);
        }

        // Update status variables
        connected = true;
        shipController = transform.parent.GetComponent<ShipController>();
        shipController.RefreshModules();
    }

    // Disconnect component from player ship
    public virtual void DisconnectShip() {
        // Deactivate before disconnecting
        Deactivate();

        // Change parent
        transform.SetParent(null);

        // Enable rigidbody
        Rigidbody compRB = GetComponent<Rigidbody>();
        if (!compRB) {
            compRB = gameObject.AddComponent<Rigidbody>();
            compRB.mass = 5f;
        }
        if (!GetComponent<GravBody>()) gameObject.AddComponent<GravBody>();

        // Update status variables
        connected = false;
        if (shipController) shipController.RefreshModules();
        shipController = null;
    }

    // Enables component
    public virtual void Activate() {
        active = true;
    }

    // Disables component
    public virtual void Deactivate() {
        active = false;
    }

    // Accessor methods
    public bool IsActive() {
        return active;
    }

    public bool IsConnected() {
        return connected;
    }
}
