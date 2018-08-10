using UnityEngine;

///
/// MouseLook.cs
///
/// Author: Eric Thompson (Dead Battery Games)
/// Purpose: Script to move ship camera based on mouse input
///

public class ShipCam : MonoBehaviour {

    // Private
    [SerializeField] private float sensitivityX = 10f;
    [SerializeField] private float sensitivityY = 10f;
    [SerializeField] private float resetSpeed = 5f;

    [SerializeField] private Camera cam;
    [SerializeField] private AudioListener listener;

    private ShipController shipController;
    private bool attached = true;
    private Vector3 detachedUpAxis;

	void Start () {
        shipController = GetComponentInParent<ShipController>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update () {
        if (Input.GetButton("RotateShipCam") && shipController.control) {
            float rotX = Input.GetAxisRaw("Mouse X") * sensitivityX / 10f;
            float rotY = Input.GetAxisRaw("Mouse Y") * sensitivityY / 10f;

            if (attached) {
                transform.Rotate(transform.parent.up, rotX, Space.World);
                transform.Rotate(transform.right, -rotY, Space.World);
            } else {
                transform.Rotate(detachedUpAxis, rotX, Space.World);
                transform.Rotate(transform.right, -rotY, Space.World);
                cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, 60, Time.deltaTime);
            }
        } else {
            if (attached) {
                transform.rotation = Quaternion.Slerp(transform.rotation, transform.parent.rotation, resetSpeed * Time.deltaTime);
            }
        }
    }

    public void Detach() {
        if (attached) {
            detachedUpAxis = transform.up;
            transform.SetParent(transform.parent.parent);
            attached = false;
        }
    }

    public void SetControl(bool set) {
        if (set) {
            cam.enabled = true;
            listener.enabled = true;
            GetComponentInChildren<PostSwitch>().Set();
        } else {
            cam.enabled = false;
            listener.enabled = false;
        }
    }
} 
