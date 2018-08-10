using UnityEngine;
using UnityEngine.SceneManagement;

///
/// MenuManager.cs
///
/// Author: Eric Thompson (Dead Battery Games)
/// Purpose: Handles menu transitions
///

public class MenuManager : MonoBehaviour {

    // Update is called once per frame
    void Update() {
        if (Input.GetButtonDown("Menu")) {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Menu();
        }
	}

    public void RestartLevel() {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    public void Menu() {
        SceneManager.LoadScene("Menu");
    }

    public void StartChallenge() {
        SceneManager.LoadScene("Challenge");
    }

    public void StartSandbox() {
        SceneManager.LoadScene("Sandbox");
    }

    public void StartGorbonia() {
        SceneManager.LoadScene("Gorbonia");
    }

    public void ExitGame() {
        Application.Quit();
    }
}
