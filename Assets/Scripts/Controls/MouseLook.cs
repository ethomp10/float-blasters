using UnityEngine;

///
/// MouseLook.cs
///
/// Author: Eric Thompson (Dead Battery Games)
/// Purpose: Script to move camera based on mouse input
///

public class MouseLook : MonoBehaviour {

    // Public
    public float sensitivityX = 10f;
    public float sensitivityY = 10f;
    public float scopeMultiplier = 0.5f;
    public float recoilRecoverySpeed = 5f;

    [HideInInspector] public bool yLock = false;
    [HideInInspector] public float yOffset = 0f;
    [HideInInspector] public bool scopeSensitivity = false;

    // Private
    [SerializeField] private float minY = -60f;
    [SerializeField] private float maxY = 60f;
    [SerializeField] private float minYGravClaw = -10f;

	private float rotX = 0f;
    private float rotY = 0f;
    private float setY = 0f;

    void Start() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update () {
        rotX = Input.GetAxisRaw("Mouse X") * sensitivityX / 10f;
        rotY = Input.GetAxisRaw("Mouse Y") * sensitivityY / 10f;

        if (scopeSensitivity) {
            rotX *= scopeMultiplier;
            rotY *= scopeMultiplier;

            yOffset = Mathf.Lerp(yOffset, 0f, recoilRecoverySpeed * Time.deltaTime);
        } else {
            yOffset = 0f;
        }

        if (yLock) {
            if (setY < minYGravClaw) {
                setY = Mathf.Lerp(setY, minYGravClaw, 5f * Time.deltaTime);
                if (rotY < 0) {
                    rotY = 0;
                }
            }
        }

        setY += rotY;
        setY = Mathf.Clamp(setY, minY, maxY);
		
		// Rotate camera
		transform.localEulerAngles = new Vector3 (-setY - yOffset, 0f, 0f);

        // Turn player
        transform.parent.Rotate(transform.parent.up, rotX, Space.World);
	}
}
