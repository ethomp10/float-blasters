using UnityEngine;
using UnityEngine.PostProcessing;

///
/// PostSwitch.cs
///
/// Author: Eric Thompson (Dead Battery Games)
/// Purpose: Post processing switch for slower computers
///

public class PostSwitch : MonoBehaviour {

	PostProcessingBehaviour postProcessing;

	void Start () {
		postProcessing = GetComponent<PostProcessingBehaviour>();

        Set();
	}
	
	void Update () {
		if (Input.GetButtonDown("PostProcessing")) {
            if (GameObject.FindGameObjectWithTag("Player")) {
                if (transform.parent.CompareTag("Player")) {
                    Switch();
                }
            } else {
                Switch();
            }
		}
	}

    void Switch() {
        if (PlayerStats.postProcessing) {
            postProcessing.enabled = false;
            PlayerStats.postProcessing = false;
        } else {
            postProcessing.enabled = true;
            PlayerStats.postProcessing = true;
        }
    }

    public void Set() {
        postProcessing.enabled = PlayerStats.postProcessing;
    }
}
