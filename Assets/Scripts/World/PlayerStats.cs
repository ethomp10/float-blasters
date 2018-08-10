using UnityEngine;

///
/// PlayerStats.cs
///
/// Author: Eric Thompson (Dead Battery Games)
/// Purpose: Keeps track of player stats
///

public class PlayerStats : MonoBehaviour {

    public static float maxPlayerHealth = 100f;
    [HideInInspector] public static float currentPlayerHealth;
    public static bool postProcessing = true;

    void Start() {
        currentPlayerHealth = maxPlayerHealth;
    }
}
