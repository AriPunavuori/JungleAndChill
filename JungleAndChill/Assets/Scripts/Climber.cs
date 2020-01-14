using UnityEngine;
using System.Collections.Generic;
using Valve.VR;

[RequireComponent(typeof(Rigidbody))]
public class Climber : MonoBehaviour {
    LayerMask mask;
    Rigidbody rb;
    float rayLength = 0.1f;
    public ClimberHand RightHand;
    public ClimberHand LeftHand;
    public SteamVR_Action_Boolean ToggleGripButton;
    public ConfigurableJoint ClimberHandle;
    public List<ClimberHand> grabbedHands = new List<ClimberHand>(); // first in list is active

    private void Awake() {
        rb = GetComponent<Rigidbody>();
        mask = LayerMask.NameToLayer("Floor");
    }
    bool IsClimbing() {
        return grabbedHands.Count > 0;
    }
    ClimberHand ActiveHand() {
        return grabbedHands[0];
    }

    void Update() {
        UpdateHand(RightHand);
        UpdateHand(LeftHand);
        if(IsClimbing()) {
            ClimberHandle.targetPosition = -ActiveHand().transform.localPosition;//update collider for hand movment
        } else if(Physics.Raycast(transform.position, Vector3.down, rayLength, mask)) {
            rb.useGravity = false;
        } else {
            rb.useGravity = true;
        }
    }

    void UpdateHand(ClimberHand hand) {
        if (grabbedHands.Contains(hand)) {
            if (ToggleGripButton.GetStateUp(hand.hand)) {
                grabbedHands.Remove(hand);
                if (!IsClimbing()) {
                    rb.useGravity = true;
                    ClimberHandle.connectedBody = null;
                }
            }
        } else {
            if (hand.touchedCount > 0 && ToggleGripButton.GetStateDown(hand.hand)) {
                grabbedHands.Insert(0, hand);
                print("adding hand " + hand.name + " to list idx 0");
                ClimberHandle.transform.position = hand.transform.position;
                rb.useGravity = false;
                ClimberHandle.connectedBody = GetComponent<Rigidbody>();
            }
        }
    }
}