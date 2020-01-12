using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

[RequireComponent(typeof(Interactable))]
public class HoldHandles : MonoBehaviour {
  [Tooltip("How much angular difference affects the calculation of closest position. The calculated distance to a position is multiplied by: 1 - (180 / angleDifference  * " + nameof(angleWeight) + ")")]
  public float angleWeight = 1;
  public Transform[] handles;

  private void OnAttachedToHand(Hand hand) {
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
      // Set position so that selected handle is at hand position
      transform.position = hand.transform.position + (transform.position - selected.position);
      // Set angle so that selected angle matches hand angle
      transform.rotation = selected.rotation;
    }
  }
}
