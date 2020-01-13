
using UnityEngine;
using System.Collections;
using Valve.VR;
using Valve.VR.InteractionSystem;

//-------------------------------------------------------------------------
[RequireComponent(typeof(Throwable))]
public class Boomerang : MonoBehaviour {
  public AnimationCurve rotationCurve = new AnimationCurve();
  public float curveDuration = 3;
  [Tooltip("Percentage of angular rotation converted to upwards(local) motion")]
  public float spinLift = 1;
  public float maxRotation = 180;
  public Vector3 testVelocity = new Vector3(1, 0, 0);

  private bool boomeranging = false;
  private float prevAngle = 0;
  private float boomerangStart = 0;
  private Rigidbody rb;

  [MyBox.ButtonMethod]
  public void TestThrow() {
    rb.velocity = testVelocity;
    StartBoomeranging();
  }

  private void Start() {
    rb = GetComponent<Rigidbody>();
  }

  //-------------------------------------------------
  // Called when this GameObject becomes attached to the hand
  //-------------------------------------------------
  private void OnAttachedToHand(Hand hand) {
    StopBoomeranging();
  }



  //-------------------------------------------------
  // Called when this GameObject is detached from the hand
  //-------------------------------------------------
  private void OnDetachedFromHand(Hand hand) {
    StartBoomeranging();
  }

  //-------------------------------------------------
  // Called when this GameObject is detached from the hand
  //-------------------------------------------------
  private void OnCollisionEnter(Collision col) {
    StopBoomeranging();
  }

  private void StartBoomeranging() {
    boomeranging = true;
    prevAngle = 0;
    boomerangStart = Time.time;
  }
  private void StopBoomeranging() {
    boomeranging = false;
    boomerangStart = 0;
  }

  //-------------------------------------------------
  // Called every Update() while this GameObject is attached to the hand
  //-------------------------------------------------
  private void Update() {
    if (boomeranging) {
      rb.AddForce(transform.up * (rb.angularVelocity.y * spinLift));

      var time = Time.time;
      var fract = (time - boomerangStart) / curveDuration;
      var angle = rotationCurve.Evaluate(fract) * maxRotation;
      var angleDiff = angle - prevAngle;
      prevAngle = angle;

      Quaternion rot = Quaternion.AngleAxis(angleDiff, Vector3.up);
      rb.velocity = rot * rb.velocity;

      if (fract > 1) {
        StopBoomeranging();
      }
    }
  }
}