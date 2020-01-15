using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FC_Restable : MonoBehaviour {

  [Tooltip("The resting points to use. If none are assigned find them in scene")]
  public FC_RestingSpot[] spots;
  [Tooltip("How much to force rb velocity direction towards closest resting point")]
  public float maxDirectionStrength = 1;
  [Tooltip("Dont do nuffin before this duration")]
  public float minDuration = 10;
  [Tooltip("Max strength is reached at this point and duration resets to 0!")]
  public float wantDuration = 30;
  [Tooltip("Attach to a resting point when at this distance")]
  public float restDistance = 0.1f;
  [Tooltip("When resting, have this chance of unresting (per second on average)")]
  public float unrestChance = 0.25f;

  private Rigidbody rb;
  private FC_Attractable atbl;
  private TargetVelocity tv;
  private float unrestTime;
  private TransformData prevTrans;
  private Transform prevParent;
  private bool resting = false;


  public void StartResting(Transform t) {
    resting = true;
    if (atbl != null) atbl.enabled = false;
    if (tv != null) tv.enabled = false;
    prevTrans = transform.Save();
    prevParent = transform.parent;
    transform.parent = t;
    transform.position = t.position;
    transform.rotation = t.rotation;
  }

  public void StopResting() {
    resting = false;
    if (atbl != null) atbl.enabled = true;
    if (tv != null) tv.enabled = true;
    transform.Load(prevTrans);
    transform.parent = prevParent;
    unrestTime = Time.time;
  }


  // Start is called before the first frame update
  void Start() {
    unrestTime = Time.time;
    rb = GetComponent<Rigidbody>();
    atbl = GetComponent<FC_Attractable>();
    tv = GetComponent<TargetVelocity>();
    if (spots.Length == 0) {
      spots = GameObject.FindObjectsOfType<FC_RestingSpot>();
    }
  }

  FC_RestingSpot FindClosestSpot(out float distance) {
    distance = float.PositiveInfinity;
    FC_RestingSpot minAtt = null;
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
    if (fract < 0) return;
    var strength = fract * maxDirectionStrength;
    if (fract >= 1) unrestTime = Time.time;
    rb.velocity = rb.velocity * (1 - maxDirectionStrength) + rb.velocity.SetDirSafe(closest.transform.position - transform.position) * maxDirectionStrength;

    if (dist < restDistance)
      StartResting(closest.transform);
  }

  // https://answers.unity.com/questions/1353041/deltatime-dependent-random-wander-math-problem.html
  public static float DeltaAdjustedProbability(float chance) {
    return 1 - Mathf.Pow(1 - chance, Time.deltaTime);
  }
}
