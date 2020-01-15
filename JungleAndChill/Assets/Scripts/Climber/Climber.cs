using UnityEngine;
using System.Collections.Generic;
using Valve.VR;

[RequireComponent(typeof(Rigidbody))]
public class Climber : MonoBehaviour {
    public LayerMask mask;
    public float moveSpeed;
    Rigidbody rb;
    GameObject player;
    float rayLength = 0.5f;
    public ClimberHand RightHand;
    public ClimberHand LeftHand;
    public SteamVR_Action_Boolean ToggleGripButton;
    public ConfigurableJoint ClimberHandle;
    public List<ClimberHand> grabbedHands = new List<ClimberHand>(); // first in list is active

    private void Awake() {
        rb = GetComponent<Rigidbody>();
        player = GameObject.Find("Player");
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
        } 
    }

    private void FixedUpdate() {
        if (Physics.Raycast(transform.position + Vector3.up * rayLength, Vector3.down, rayLength, mask)) { //If raycast hits ground disable gravity
            //print("Hit ground");
            rb.useGravity = false;
            rb.velocity = Vector3.zero;
        } else {
            rb.useGravity = true;
        }
        var targetPos = Vector3.up * transform.position.y; 
        rb.MovePosition(Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime));
        //Debug.DrawLine(transform.position + Vector3.up * rayLength, transform.position + Vector3.down * rayLength, Color.blue);
    }

    void UpdateHand(ClimberHand hand) {
        if (grabbedHands.Contains(hand)) {
            if (ToggleGripButton.GetStateUp(hand.hand)) { //Remove connection to ClimberHandle and return gravity
                grabbedHands.Remove(hand);
                if (!IsClimbing()) {
                    rb.useGravity = true;
                    ClimberHandle.connectedBody = null;
                }
            }
        } else {
            if (hand.touchedCount > 0 && ToggleGripButton.GetStateDown(hand.hand)) { //Attach this rigidbody to ClimberHandle and remove gravity
                grabbedHands.Insert(0, hand);
                print("adding hand " + hand.name + " to list idx 0");
                ClimberHandle.transform.position = hand.transform.position;
                rb.useGravity = false;
                ClimberHandle.connectedBody = GetComponent<Rigidbody>();
            }
        }
    }

}