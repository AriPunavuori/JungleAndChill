using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

[RequireComponent(typeof(Rigidbody))]
public class FC_Attractable : MonoBehaviour {
  private Rigidbody rb;
  [Tooltip("The attractor to use. Defaults to closest one")]
  public FC_Attractor attractor;
  [PositiveValueOnly]
  public float farStrength = 0.1f;
  [PositiveValueOnly]
  public float nearStrength = 1;
  [Tooltip("Near attraction strength is applied at distances lower than this")]
  public float nearDistance = 0.1f;
  [Tooltip("Far attraction strength is applied at distances higher than this")]
  public float farDistance = 1;

  // Start is called before the first frame update
  void Start() {
    rb = GetComponent<Rigidbody>();
    if (attractor == null) attractor = FindClosestAttractor();
    if (attractor == null) throw new UnityException("No attractor found in scene!");
  }

  FC_Attractor FindClosestAttractor() {
    var minDist = float.PositiveInfinity;
    FC_Attractor minAtt = null;
    foreach (var att in GetComponents<FC_Attractor>()) {
      var dist = Vector3.Distance(transform.position, att.transform.position);
      if (dist < minDist) {
        minDist = dist;
        minAtt = att;
      }
    }
    return minAtt;
  }

  // Update is called once per frame
  void FixedUpdate() {
    var dist = Vector3.Distance(attractor.transform.position, transform.position);
    dist = Mathf.Clamp(dist, nearDistance, farDistance);
    var strength = dist.Remap(nearDistance, farDistance, nearStrength, farStrength);

    rb.AddForce((attractor.transform.position - transform.position).SetLenSafe(strength) * Time.deltaTime);
  }
}
