using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Rester : MonoBehaviour {

  [Tooltip("The resting points to use. If none are assigned find them in scene")]
  public RestingSpot[] spots;
  [Tooltip("Dont do nuffin before this duration")]
  public float minDuration = 10;
  [Tooltip("Max strength is reached at this point and duration resets to 0!")]
  public float wantDuration = 30;
  [Tooltip("Attach to a resting point when at this distance")]
  public float restDistance = 0.1f;
  [Tooltip("When resting, have this chance of unresting (per second on average)")]
  public float unrestChance = 0.25f;
  [Tooltip("Strength of deltaTime when applying rotation")]
  public float maxDirectionStrength = 1;
  [Tooltip("Additional strength added to TargetVelocity component")]
  public float maxTVDeltaMultInc;
  [Tooltip("Disable these things when resting")]
  public Disabler disables;

  private Rigidbody rb;
  private TargetVelocity tv;
  private float origStrength;
  private float unrestTime;
  private TransformData prevTrans;
  private Transform mainParent;
  private bool resting = false;

  public void StartResting(Transform t) {
    if (resting) return;
    disables.DisableComponents();
    resting = true;
    if (tv != null) tv.enabled = false;
    transform.parent = t;
    rb.MovePosition(t.position);
    rb.MoveRotation(t.rotation);
    rb.velocity = Vector3.zero;
    rb.angularVelocity = Vector3.zero;
  }

  public void StopResting() {
    if (!resting) return;
    disables.EnableComponents();
    resting = false;
    if (tv != null) tv.enabled = true;
    transform.parent = mainParent;
    unrestTime = Time.time;
  }


  // Start is called before the first frame update
  void Start() {
    mainParent = transform.parent;
    unrestTime = Time.time;
    rb = GetComponent<Rigidbody>();
    tv = GetComponent<TargetVelocity>();
    if (tv != null) origStrength = tv.strength;
    if (spots.Length == 0) {
      spots = GameObject.FindObjectsOfType<RestingSpot>();
    }
  }

  RestingSpot FindClosestSpot(out float distance) {
    distance = float.PositiveInfinity;
    RestingSpot minAtt = null;
    foreach (var spot in spots) {
      var dist = Vector3.Distance(transform.position, spot.transform.position);
      if (dist < distance) {
        distance = dist;
        minAtt = spot;
      }
    }
    return minAtt;
  }


  // Update is called once per frame
  void FixedUpdate() {

    if (resting) {
      var ran = Random.value;

      if (ran <= DeltaAdjustedProbability(unrestChance))
        StopResting();
      return;
    }

    var closest = FindClosestSpot(out var dist);
    if (closest == null) return;
    var fract = (Time.time - (unrestTime + minDuration)) / (wantDuration + minDuration);
    if (fract < 0) {
      tv.strength = origStrength;
      return;
    }
    if (tv != null) tv.strength = origStrength + fract * maxTVDeltaMultInc;

    var dirStrength = fract * maxDirectionStrength;
    if (fract >= 1) unrestTime = Time.time;
    var dir = closest.transform.position - transform.position;
    rb.velocity = Vector3.RotateTowards(rb.velocity, dir, 1 - Mathf.Pow(1 - Time.deltaTime, dirStrength), 0);

    if (dist < restDistance)
      StartResting(closest.transform);
  }

  // https://answers.unity.com/questions/1353041/deltatime-dependent-random-wander-math-problem.html
  public static float DeltaAdjustedProbability(float chance) {
    return 1 - Mathf.Pow(1 - chance, Time.deltaTime);
  }
}
