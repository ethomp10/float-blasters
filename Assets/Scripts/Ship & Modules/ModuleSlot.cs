using UnityEngine;

///
/// ModuleSlot.cs
///
/// Author: Eric Thompson (Dead Battery Games)
/// Purpose: Allows player to connect modules
///

public class ModuleSlot : MonoBehaviour {

    // Public
    public Material guideMaterial;
    public Material connectMaterial;

    // Private
    private int collisionCount = 0;
    private MeshRenderer[] meshes;

    void Start() {
        meshes = GetComponentsInChildren<MeshRenderer>();
    }

    void OnTriggerEnter(Collider other) {
        if (other.transform.parent != null && other.transform.parent.name == name) {
            collisionCount++;
            if (collisionCount > 0 && other.GetComponentInParent<GravClaw>()) {
                other.GetComponentInParent<GravClaw>().canConnect = true;
                foreach (MeshRenderer mesh in meshes) {
                    mesh.material = connectMaterial;
                }
            }
        }
    }

    void OnTriggerExit(Collider other) {
        if (other.transform.parent != null && other.transform.parent.name == name) {
            collisionCount--;
            if (collisionCount == 0 && other.GetComponentInParent<GravClaw>()) {
                other.GetComponentInParent<GravClaw>().canConnect = false;
                foreach (MeshRenderer mesh in meshes) {
                    mesh.material = guideMaterial;
                }
            }
        }
    }

    public void ResetCollisionCount() {
        collisionCount = 0;
        foreach (MeshRenderer mesh in meshes) {
            mesh.material = guideMaterial;
        }
    }
}
