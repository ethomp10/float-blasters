using UnityEngine;

///
/// Spawner.cs
///
/// Author: Eric Thompson (Dead Battery Games)
/// Purpose: Handles player and NPC spawning
///

public class Spawner : MonoBehaviour {

    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform startSpawn;

    void Start() {
        // Disable spawn meshes
        foreach (GameObject mesh in GameObject.FindGameObjectsWithTag("Respawn")) {
            mesh.GetComponent<MeshRenderer>().enabled = false;
        }

        if (playerPrefab == null) Debug.LogWarning("Spawner: No player prefab; set in the inspector");
        if (startSpawn == null) Debug.LogWarning("Spawner: No start spawn; set in the inspector");

        SpawnPlayer(startSpawn);
    }

    public void SpawnPlayer(Transform spawnPoint) {
        GameObject currentPlayer = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);

        // Change camera background colour
        Camera playerCam = currentPlayer.GetComponentInChildren<Camera>();
        Sun sun = FindObjectOfType<Sun>();
        playerCam.backgroundColor = sun.GetSkyColour(spawnPoint.position);
        sun.ChangeCamera(playerCam);
    }
}
