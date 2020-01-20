using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;
//[RequireComponent(typeof(FixedJoint))]
[RequireComponent(typeof(Interactable))]
public class DetachOnPickUp : MonoBehaviour
{
    Interactable inter;
    FixedJoint joint;

    void Start()
    {
        inter= GetComponent<Interactable>();
        inter.onAttachedToHand += OnAttach;
        joint = GetComponent<FixedJoint>();
    }

    void OnAttach(Hand hand)
    {
        Destroy(joint);
        //joint.connectedBody = null;
    }
}
