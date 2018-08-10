using UnityEngine;
using UnityEngine.UI;

///
/// UIKey.cs
///
/// Author: Eric Thompson (Dead Battery Games)
/// Purpose: Script to change ui key colour on input
///

public class UIKey : MonoBehaviour {

    // Public
    public KeyCode key;
    public Color pressedColour;

    // Private
    private Color originalColour;
    private Image background;

	// Use this for initialization
	void Start () {
        background = GetComponent<Image>();
        originalColour = background.color;
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(key)) {
            background.color = pressedColour;
        } else if (Input.GetKeyUp(key)) {
            background.color = originalColour;
        }
	}
}
