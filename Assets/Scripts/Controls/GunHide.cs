using UnityEngine;

///
/// GunHide.cs
///
/// Author: Eric Thompson (Dead Battery Games)
/// Purpose: Hides player weapon when colliding with objects
///

public class GunHide : MonoBehaviour {

    private PlayerControl playerMove;
    private int collisionCount = 0;
    private Vector3 multiCannonPos;
    private Vector3 gravClawPos = new Vector3(-0.239f, -0.486f, 1.065f);

	void Awake () {
        multiCannonPos = transform.localPosition;
        playerMove = transform.parent.parent.GetComponent<PlayerControl>();
	}

    void OnTriggerEnter(Collider other) {
        collisionCount++;
        if (collisionCount > 0) {
            playerMove.HideGun(true);
        }
    }

    void OnTriggerExit(Collider other) {
        collisionCount--;
        if (collisionCount <= 0) {
            playerMove.HideGun(false);
        }
    }

    public void MoveTrigger(int weaponNum) {
        if (weaponNum == 0) {
            transform.localPosition = multiCannonPos;
        } else if (weaponNum == 1) {
            transform.localPosition = gravClawPos;
        }
    }
}
