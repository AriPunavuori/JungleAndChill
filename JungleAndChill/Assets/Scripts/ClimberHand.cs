using UnityEngine;
using Valve.VR;
public class ClimberHand : MonoBehaviour {
    public SteamVR_Input_Sources hand;
    public int touchedCount;
    //public bool grabbing;

    void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Climbable")) {
            touchedCount++;
        }
    }
    void OnTriggerExit(Collider other) {
        if (other.CompareTag("Climbable")) {
            touchedCount--;
            if (touchedCount < 0)
                Debug.LogError("ei näin");
        }
    }
}