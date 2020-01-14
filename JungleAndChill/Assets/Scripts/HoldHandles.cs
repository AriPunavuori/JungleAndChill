using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

[RequireComponent(typeof(Interactable))]
public class HoldHandles : MonoBehaviour {
  [Tooltip("How much angular difference affects the calculation of closest position. The calculated distance to a position is multiplied by: 1 - (180 / angleDifference  * " + nameof(angleWeight) + ")")]
  public float angleWeight = 1;

  private Transform[] handles;
  private Interactable inter;
  private bool attached = false;

  private void Start() {
    inter = GetComponent<Interactable>();
    inter.onAttachedToHand += OnAttachedToHand;
    var handleGos = GetComponentsInChildren<Handle>();
    handles = handleGos.Map((v) => v.transform);
  }

  private void OnAttachedToHand(Hand hand) {
    if (attached) return;
    var handPos = hand.transform.position;

    // Find closest pos
    Transform selected = null;
    float minDist = float.PositiveInfinity;
    foreach (var pos in handles) {
      var dist = Vector3.Distance(pos.position, handPos);
      // Factor in angle difference
      if (angleWeight != 0)
        dist = dist * (1 - (180 / Vector3.Angle(pos.rotation.eulerAngles, hand.transform.rotation.eulerAngles) * angleWeight));
      if (dist < minDist) {
        selected = pos;
        minDist = dist;
      }
    }
    // Move to selected position
    if (selected != null) {
      var flags = hand.currentAttachedObjectInfo.Value.attachmentFlags;
      var grabType = hand.currentAttachedObjectInfo.Value.grabbedWithType;

      attached = true;
      hand.DetachObject(gameObject);
      // Set angle so that handle matches the hand angle
      var rot = hand.transform.rotation.eulerAngles - selected.transform.rotation.eulerAngles;
      transform.Rotate(-rot);

      // Set position so that handle matches the hand position
      var move = handPos - selected.transform.position;
      transform.Translate(move);

      hand.AttachObject(gameObject, grabType, flags);
      attached = false;
    }
  }
}
