using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Spookable : MonoBehaviour {

  [Tooltip("Avoid spooks which reach this velocity (fixed delta scaled distance from old position)")]
  public float spookVelocity = 0.2f;
  [Tooltip("Full spookiness below this distance")]
  public float minSpookyDist = 0.2f;
  [Tooltip("No spookiness above this distance")]
  public float maxSpookyDist = 1f;
  [Tooltip("Add at most this much of a spook's velocity to own velocity")]
  public float maxSpookiness;
  [Tooltip("Spooky objects")]
  public Transform[] spooks;


  private Rigidbody rb;
  private Rester rtbl;
  private Vector3[] oldPositions;


  // Start is called before the first frame update
  void Start() {
    rb = GetComponent<Rigidbody>();
    rtbl = GetComponent<Rester>();
    oldPositions = spooks.Map(s => s.transform.position);
  }

  void Update() {
    CheckSpooks();
  }

  void CheckSpooks() {
    for (int i = 0; i < spooks.Length; i++) {
      var distance = Vector3.Distance(spooks[i].position, transform.position);
      if (distance > maxSpookyDist) continue;

      var oldPos = oldPositions[i];
      var spookPos = spooks[i].position;
      oldPositions[i] = spookPos;
      var velocity = (spookPos - oldPos) / Time.smoothDeltaTime;
      var speed = velocity.magnitude;
      if (speed >= spookVelocity) {
        var angleMultiplier = 1 - Vector3.Angle(velocity, transform.position - spookPos) / 180;
        if (distance < minSpookyDist) {
          rb.AddForce(velocity.SetDirSafe(transform.position - spookPos) * maxSpookiness * angleMultiplier);
          continue;
        }
        var spookiness = distance.Remap(minSpookyDist, maxSpookyDist, maxSpookiness, 0);
        rb.AddForce(velocity.SetDirSafe(transform.position - spookPos) * spookiness * angleMultiplier);

      }
    }
  }

  void GetSpooked(Vector3 sourceOldPos, Vector3 sourceNewPos, float spookiness) {
    if (rtbl != null) rtbl.StopResting();
    var vel = (sourceOldPos - sourceNewPos) * spookiness;
    rb.AddForce(vel);
  }
}
