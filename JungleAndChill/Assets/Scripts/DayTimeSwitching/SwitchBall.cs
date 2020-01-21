using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class SwitchBall : MonoBehaviour {

  public float triggerDistance = 5f;

  public GameObject createOnHit;

  [MyBox.Foldout("Descale Settings", true)]
  public AnimationCurve descaleCurve;
  public float descaleDuration = 0.2f;
  public float descaleSpeedDivisor = 5;

  [MyBox.Foldout("Shader Settings", true)]
  public bool doTimeScale = true;
  public AnimationCurve shaderTimeScaleCurve;
  public bool doNoiseScale = true;
  public AnimationCurve shaderNoiseScaleCurve;
  public bool doNoiseMultRange = true;
  public AnimationCurve shaderNoiseMultRangeMin;
  public AnimationCurve shaderNoiseMultRangeMax;

  [MyBox.Foldout("Rescale Settings", true)]
  public AnimationCurve rescaleCurve;
  public float rescaleDuration = 0.5f;


  DaytimeSwitcher ds;
  Rigidbody rb;
  Material mat;
  float normTimeScale;
  Vector2 normNoiseMultRange;
  float normNoiseScale;
  Transform cam;
  GameObject created;
  Vector3 spawnPoint;
  Vector3 throwPoint;
  Vector3 scale;
  bool ballThrown;
  bool ballHit;


  bool descale = false;
  float descaleTime;

  bool rescale = false;
  float rescaleTime;

  private void Start() {
    spawnPoint = transform.position;
    ds = GameObject.FindObjectOfType<DaytimeSwitcher>();
    rb = GetComponent<Rigidbody>();

    var inter = GetComponent<Interactable>();
    inter.onAttachedToHand += OnPickUp;
    inter.onDetachedFromHand += OnDetach;
    cam = Camera.main.transform;
    scale = transform.localScale;

    if (doTimeScale || doNoiseScale || doNoiseMultRange)
      mat = GetComponent<Renderer>().material;

    if (doTimeScale)
      normTimeScale = mat.GetFloat("TimeScale");

    if (doNoiseScale)
      normNoiseScale = mat.GetFloat("NoiseScale");

    if (doNoiseMultRange)
      normNoiseMultRange = mat.GetVector("NoiseMultRange");

    ReSpawn();
  }

  private void Update() {

    if (descale) {
      var fraction = Mathf.Clamp01((Time.time - descaleTime) / descaleDuration);
      transform.localScale = descaleCurve.Evaluate(fraction) * scale;
      if (doTimeScale) mat.SetFloat("TimeScale", shaderTimeScaleCurve.Evaluate(fraction) * normTimeScale);
      if (doNoiseScale) mat.SetFloat("NoiseScale", shaderNoiseScaleCurve.Evaluate(fraction) * normNoiseScale);
      if (doNoiseMultRange) {
        mat.SetVector("NoiseMultRange", new Vector2(
          shaderTimeScaleCurve.Evaluate(fraction) * normNoiseMultRange.x,
          shaderTimeScaleCurve.Evaluate(fraction) * normNoiseMultRange.y
        ));
      }
      rb.velocity = rb.velocity / descaleSpeedDivisor;
      if (fraction == 1) {
        ds.SetSource(cam.position - transform.position);
        ds.SwitchDaytime();
        if (doTimeScale) mat.SetFloat("TimeScale", normTimeScale);
        if (doNoiseScale) mat.SetFloat("NoiseScale", 1);
        if (doNoiseMultRange) mat.SetVector("NoiseMultRange", Vector2.one);
        if (createOnHit != null) created = Instantiate(createOnHit, transform.position, Quaternion.identity);
        descale = false;
      }
    } else if (rescale) {
      var fraction = Mathf.Clamp01((Time.time - rescaleTime) / rescaleDuration);
      if (doNoiseScale) mat.SetFloat("NoiseScale", normNoiseScale * fraction);
      if (doNoiseMultRange) mat.SetVector("NoiseMultRange", normNoiseMultRange * fraction);
      transform.localScale = rescaleCurve.Evaluate(fraction) * scale;
      if (fraction == 1) rescale = false;
    }

    if (ballHit && Time.time >= descaleTime + ds.switchTime + descaleDuration) {
      ReSpawn();
    }

    if (ballThrown) {
      if (!ballHit && Vector3.Distance(throwPoint, transform.position) > triggerDistance) {
        ballHit = true;
        DeSpawn();
      }
    }
  }

  [MyBox.ButtonMethod]
  void DeSpawn() {
    descale = true;
    descaleTime = Time.time;
  }

  [MyBox.ButtonMethod]
  void ReSpawn() {
    rb.MovePosition(spawnPoint);
    rb.velocity = Vector3.zero;
    rb.angularVelocity = Vector3.zero;

    if (created != null) Destroy(created);
    transform.localScale = Vector3.zero;
    rescale = true;
    rescaleTime = Time.time;

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
