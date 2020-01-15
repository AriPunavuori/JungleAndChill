
using UnityEngine;
using System.Collections;
using Valve.VR;
using Valve.VR.InteractionSystem;

//-------------------------------------------------------------------------
[RequireComponent(typeof(Throwable))]
public class Boomerang : MonoBehaviour {
  public AnimationCurve rotationCurve = new AnimationCurve();

  public float velocityToDuration = 1;

  [Tooltip("Percentage of angular rotation converted to upwards(local) motion. Wonky and hard to configure and use with gravity on")]
  public bool disableGravity = false;
  [MyBox.ConditionalField(nameof(disableGravity), true)]
  public float spinLift = 1;
  [MyBox.ConditionalField(nameof(disableGravity), true)]
  public float maxLift = 1;
  public float maxRotation = 240;
  public Vector3 testVelocity = new Vector3(1, 0, 0);

  private float curveDuration;
  private bool boomeranging = false;
  private float prevRot = 0;
  private float boomerangStart = 0;
  private Rigidbody rb;

  [MyBox.ButtonMethod]
  public void TestThrow() {
    rb.velocity = testVelocity;
    StartBoomeranging();
  }

  private void Start() {
    rb = GetComponent<Rigidbody>();
    var inter = GetComponent<Interactable>();
    inter.onDetachedFromHand += StartBoomeranging;
    inter.onAttachedToHand += StopBoomeranging;

    rb.velocity = testVelocity;
    StartBoomeranging();
  }

  private void OnCollisionEnter(Collision col) {
    StopBoomeranging();
  }

  private void StartBoomeranging(Hand hand = default(Hand)) {
    if (disableGravity)
      rb.useGravity = false;
    curveDuration = rb.velocity.magnitude * velocityToDuration;
    boomeranging = true;
    prevRot = 0;
    boomerangStart = Time.time;
  }
  private void StopBoomeranging(Hand hand = default(Hand)) {
    if (disableGravity)
      rb.useGravity = true;
    boomeranging = false;
    boomerangStart = 0;
  }

  private void Update() {
    if (boomeranging) {

      if (!disableGravity) {
        var upVel = Vector3.Dot(rb.velocity, transform.up);
        var newUpVel = Vector3.Dot(rb.velocity, transform.up);
        rb.AddForce(transform.up * (Mathf.Min(rb.angularVelocity.y * spinLift, maxLift)));
      }

      if (curveDuration == -1) {
        curveDuration = rb.velocity.magnitude * velocityToDuration;
      }
      var fract = (Time.time - boomerangStart) / (curveDuration == 0 ? -1 : curveDuration);
      var rot = rotationCurve.Evaluate(fract) * maxRotation;
      var rotDiff = rot - prevRot;
      prevRot = rot;

      Quaternion qtRot = Quaternion.AngleAxis(rotDiff, transform.up);
      if (!rb.velocity.Equals(Vector3.zero))
        rb.velocity = qtRot * rb.velocity;

      if (fract > 1) {
        StopBoomeranging();
      }
    }
  }
}