using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class TargetVelocity : MonoBehaviour {

  [Tooltip("Aproach this velocity")]
  public float targetVelocity = 1;
  [Tooltip("Multiply time.deltaTime with this value. Will increase speed of reaching target velocity")]
  public float deltaMultiplier = 1;

  private Rigidbody rb;

  // Start is called before the first frame update
  void Start() {
    rb = GetComponent<Rigidbody>();
  }

  // Update is called once per frame
  void FixedUpdate() {
    var mag = rb.velocity.magnitude;
    var mult = Mathf.Lerp(mag, targetVelocity, Time.deltaTime * deltaMultiplier);
    rb.velocity = rb.velocity.SetLenSafe(mult);
  }
}
