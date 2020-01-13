﻿using UnityEngine;
using System.Collections.Generic;
using Valve.VR;

[RequireComponent(typeof(Rigidbody))]
public class Climber : MonoBehaviour {
    public ClimberHand RightHand;
    public ClimberHand LeftHand;
    public SteamVR_Action_Boolean ToggleGripButton;
    //public SteamVR_Action_Pose position;
    public ConfigurableJoint ClimberHandle;

    //    private bool Climbing;
    public List<ClimberHand> grabbedHands = new List<ClimberHand>(); // first in list is active

    bool IsClimbing() {
        return grabbedHands.Count > 0;
    }
    ClimberHand ActiveHand() {
        return grabbedHands[0];
    }

    void Update() {
        UpdateHand(RightHand);
        UpdateHand(LeftHand);
        if (IsClimbing()) {
            ClimberHandle.targetPosition = -ActiveHand().transform.localPosition;//update collider for hand movment
        }
    }

    void UpdateHand(ClimberHand hand) {
        if (grabbedHands.Contains(hand)) {
            if (ToggleGripButton.GetStateUp(hand.hand)) {
                grabbedHands.Remove(hand);
                if (!IsClimbing()) {
                    GetComponent<Rigidbody>().useGravity = true;
                    ClimberHandle.connectedBody = null;
                } else {
                    // position?
                }

            }
        } else {
            if (hand.touchedCount > 0 && ToggleGripButton.GetStateDown(hand.hand)) {
                grabbedHands.Insert(0, hand);
                print("adding hand " + hand.name + " to list idx 0");
                ClimberHandle.transform.position = hand.transform.position;
                GetComponent<Rigidbody>().useGravity = false;
                ClimberHandle.connectedBody = GetComponent<Rigidbody>();
            }
        }
    }
}


//using UnityEngine;
//using Valve.VR;

//[RequireComponent(typeof(Rigidbody))]
//public class Climber : MonoBehaviour {
//    public ClimberHand RightHand;
//    public ClimberHand LeftHand;
//    public SteamVR_Action_Boolean ToggleGripButton;
//    public SteamVR_Action_Pose position;
//    public ConfigurableJoint ClimberHandle;

//    private bool Climbing;
//    private ClimberHand ActiveHand;

//    void Update() {
//        updateHand(RightHand);
//        updateHand(LeftHand);
//        if (Climbing) {
//            ClimberHandle.targetPosition = -ActiveHand.transform.localPosition;//update collider for hand movment
//        }
//    }

//    void updateHand(ClimberHand Hand) {
//        if (Climbing && Hand == ActiveHand)//if is the hand used for climbing check if we are letting go.
//        {
//            if (ToggleGripButton.GetStateUp(Hand.hand)) {
//                ClimberHandle.connectedBody = null;
//                Climbing = false;

//                GetComponent<Rigidbody>().useGravity = true;
//            }
//        } else {
//            if (ToggleGripButton.GetStateDown(Hand.hand) || Hand.grabbing) {
//                Hand.grabbing = true;
//                if (Hand.touchedCount > 0) {
//                    ActiveHand = Hand;
//                    Climbing = true;
//                    ClimberHandle.transform.position = Hand.transform.position;
//                    GetComponent<Rigidbody>().useGravity = false;
//                    ClimberHandle.connectedBody = GetComponent<Rigidbody>();
//                    Hand.grabbing = false;
//                }
//            }
//        }
//    }
//}