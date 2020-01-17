using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class SwitchBall : MonoBehaviour {
  Vector3 returnPoint;
  Vector3 throwPoint;
  DaytimeSwitcher ds;
  public float triggerDistance = 5f;
  Rigidbody rb;
  float returnTime;
  float autoReturn = 10f;
  public float returnTimer = 10f;
  bool ballThrown;
  bool ballHit;
  public LayerMask rayMask;
  public Transform raySource;

  private void Start() {
    ds = GameObject.FindObjectOfType<DaytimeSwitcher>();
    returnTime = ds.switchTime;
    returnPoint = transform.position;
    rb = GetComponent<Rigidbody>();

    var inter = GetComponent<Interactable>();
    inter.onDetachedFromHand += BallThrown;
  }

  private void Update() {
    if (ballThrown) {
      returnTimer -= Time.deltaTime;
      if (!ballHit && Vector3.Distance(throwPoint, transform.position) > triggerDistance) {
        ballHit = true;
        if (Physics.Raycast(raySource.position, (transform.position - raySource.position).normalized, out var hit, float.PositiveInfinity, rayMask)) {
          ds.SetSource(hit.textureCoord);
        }
        ds.SwitchDaytime();
        returnTimer = returnTime;
      }
      if (returnTimer < 0) {
        ReturnBall();
      }
    }
  }

  public void BallThrown(Hand hand) {
    throwPoint = transform.position;
    ballThrown = true;
  }

  void ReturnBall() {
    returnTimer = autoReturn;
    rb.MovePosition(returnPoint);
    rb.useGravity = true;
    rb.velocity = Vector3.zero;
    rb.angularVelocity = Vector3.zero;
    ballThrown = false;
    ballHit = false;
  }
}
