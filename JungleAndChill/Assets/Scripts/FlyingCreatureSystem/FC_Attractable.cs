using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

[RequireComponent(typeof(Rigidbody))]
public class FC_Attractable : MonoBehaviour {
  private Rigidbody rb;
  [MustBeAssigned]
  public FC_Attractor att;
  [PositiveValueOnly]
  public float minStrength = 0.1f;
  [PositiveValueOnly]
  public float maxStrength = 1;
  [Tooltip("Maximum attraction strength is applied at distances lower than this")]
  public float minDistance = 0.1f;
  [Tooltip("Minimum attraction strength is applied at distances higher than this")]
  public float maxDistance = 1;
  [Tooltip("Velocity added over time to prevent hugging attractor")]
  public float velocityIncreaseOverTime = 0.1f;
  // Start is called before the first frame update
  void Start() {
    if (rb != null) Debug.LogError("TEST CONCLUDED AND AUTOPROPERTY WORKS");
    rb = GetComponent<Rigidbody>();
  }

  // Update is called once per frame
  void Update() {
    var dist = Vector3.Distance(att.transform.position, transform.position);
    dist = Mathf.Clamp(dist, minDistance, maxDistance);
    var strength = dist.Remap(minDistance, maxDistance, maxStrength, minStrength);

    rb.AddForce((att.transform.position - transform.position).SetLenSafe(strength));
    rb.velocity = rb.velocity * velocityIncreaseOverTime;
  }
}
