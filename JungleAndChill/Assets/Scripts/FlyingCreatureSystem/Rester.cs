using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
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
  public float restDistance = 0.025f;
  [Tooltip("When resting, have this chance of unresting (per second on average)")]
  public float unrestChance = 0.25f;
  [Tooltip("When resting, unrest if resting spot reaches this velocity")]
  public float unrestVelocity = 0.5f;
  [Tooltip("Strength of deltaTime when applying rotation towards resting points")]
  public float maxDirectionStrength = 1;
  [Tooltip("Additional strength added to TargetVelocity component if it exists on this object")]
  public float maxTVDeltaMultInc;

  public UnityEvent onRest;
  public UnityEvent onUnrest;

  [Tooltip("Disable these things when resting")]
  public Disabler disables;

  private Rigidbody rb;
  private TargetVelocity tv;
  private float origStrength;
  private float unrestTime;
  private TransformData prevTrans;
  private Transform mainParent;
  private RestingSpot spot = null;

  public void StartResting(RestingSpot spot) {
    if (this.spot != null) return;
    this.spot = spot;
    spot.AddRester(transform);
    rb.isKinematic = true;
    disables.DisableComponents();
    if (tv != null) tv.enabled = false;
    transform.parent = spot.transform;
    transform.position = spot.transform.position;
    transform.rotation = spot.transform.rotation;
    rb.velocity = Vector3.zero;
    rb.angularVelocity = Vector3.zero;

    onRest.Invoke();
  }

  public void StopResting() {
    if (spot == null) return;
    spot.RemoveRester(transform);
    rb.isKinematic = false;
    disables.EnableComponents();
    if (tv != null) tv.enabled = true;
    transform.parent = mainParent;
    unrestTime = Time.time;

    onUnrest.Invoke();
    spot = null;
  }


  // Start is called before the first frame update
  void Start() {
    mainParent = transform.parent;
    unrestTime = Time.time - Random.Range(0, wantDuration);
    rb = GetComponent<Rigidbody>();
    tv = GetComponent<TargetVelocity>();
    if (tv != null) origStrength = tv.strength;
    if (spots.Length == 0) {
      spots = GameObject.FindObjectsOfType<RestingSpot>();
    }
  }

  RestingSpot FindClosestFreeSpot(out float distance) {
    distance = float.PositiveInfinity;
    RestingSpot minAtt = null;
    foreach (var spot in spots) {
      var dist = Vector3.Distance(transform.position, spot.transform.position);
      if (dist < distance && !spot.IsFull()) {
        distance = dist;
        minAtt = spot;
      }
    }
    return minAtt;
  }


  // Update is called once per frame
  void FixedUpdate() {

    if (spot != null) {
      var change = spot.transform.position - spot.prevPos;
      var spotVelocity = change.magnitude / Time.deltaTime;
      if (spotVelocity > unrestVelocity) {
        StopResting();
        return;
      }
      var ran = Random.value;
      if (ran <= DeltaAdjustedProbability(unrestChance))
        StopResting();
      return;
    }

    var free = FindClosestFreeSpot(out var dist);
    if (free == null) {
      // Nothing found. May as well giveup searching
      unrestTime = Time.time;
      return;
    }


    var fract = (Time.time - (unrestTime + minDuration)) / (wantDuration + minDuration);
    if (fract < 0) {
      if (tv != null) tv.strength = origStrength;
      return;
    }

    if (tv != null) tv.strength = origStrength + fract * maxTVDeltaMultInc;

    if (fract >= 1) unrestTime = Time.time;

    var dirStrength = fract * maxDirectionStrength;
    var dir = free.transform.position - transform.position;
    rb.velocity = Vector3.RotateTowards(rb.velocity, dir, 1 - Mathf.Pow(1 - Time.deltaTime, dirStrength), 0);

    if (dist < restDistance)
      StartResting(free);
  }

  // https://answers.unity.com/questions/1353041/deltatime-dependent-random-wander-math-problem.html
  public static float DeltaAdjustedProbability(float chance) {
    return 1 - Mathf.Pow(1 - chance, Time.deltaTime);
  }
}
