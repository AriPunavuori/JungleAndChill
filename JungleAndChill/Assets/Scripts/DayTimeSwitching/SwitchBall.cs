using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class SwitchBall : MonoBehaviour {

  public float triggerDistance = 5f;

  public AnimationCurve deScaleCurve;
  public float deScaleDuration = 0.2f;
  public float deScaleSpeedDivisor = 5;

  public AnimationCurve reScaleCurve;
  public float reScaleDuration = 0.5f;

  public GameObject createOnHit;

  GameObject created;
  Vector3 spawnPoint;
  Vector3 throwPoint;
  DaytimeSwitcher ds;
  Rigidbody rb;
  bool ballThrown;
  bool ballHit;
  Transform cam;

  Vector3 scale;

  bool deScale = false;
  float deScaleTime;

  bool reScale = false;
  float reScaleTime;

  private void Start() {
    spawnPoint = transform.position;
    ds = GameObject.FindObjectOfType<DaytimeSwitcher>();
    rb = GetComponent<Rigidbody>();

    var inter = GetComponent<Interactable>();
    inter.onAttachedToHand += OnPickUp;
    inter.onDetachedFromHand += OnDetach;
    cam = Camera.main.transform;
    scale = transform.localScale;
    ReSpawn();
  }

  private void Update() {

    if (deScale) {
      var fraction = Mathf.Clamp01((Time.time - deScaleTime) / deScaleDuration);
      transform.localScale = deScaleCurve.Evaluate(fraction) * scale;
      rb.velocity = rb.velocity / deScaleSpeedDivisor;
      if (fraction == 1) {
        ds.SetSource(cam.position - transform.position);
        ds.SwitchDaytime();
        if (createOnHit != null) created = Instantiate(createOnHit, transform.position, Quaternion.identity);
        deScale = false;
      }
    } else if (reScale) {
      var fraction = Mathf.Clamp01((Time.time - reScaleTime) / reScaleDuration);
      transform.localScale = reScaleCurve.Evaluate(fraction) * scale;
      if (fraction == 1) reScale = false;
    }

    if (ballHit && Time.time >= deScaleTime + ds.switchTime + deScaleDuration) {
      ReSpawn();
    }

    if (ballThrown) {
      if (!ballHit && Vector3.Distance(throwPoint, transform.position) > triggerDistance) {
        ballHit = true;
        DeSpawn();
      }
    }
  }

  void DeSpawn() {
    deScale = true;
    deScaleTime = Time.time;
  }

  void ReSpawn() {
    rb.MovePosition(spawnPoint);
    rb.velocity = Vector3.zero;
    rb.angularVelocity = Vector3.zero;

    if (created != null) Destroy(created);
    transform.localScale = Vector3.zero;
    reScale = true;
    reScaleTime = Time.time;

    ballThrown = false;
    ballHit = false;
  }

  void OnPickUp(Hand hand) {
    ballThrown = false;
  }

  void OnDetach(Hand hand) {
    throwPoint = transform.position;
    ballThrown = true;
    ballHit = false;
  }

}
