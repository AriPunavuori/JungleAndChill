using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FC_Attractor : MonoBehaviour {

  public FC_RestingSpot[] restingSpots;
  public bool resting = false;

  [Tooltip("If the resting object has a velocity bigger than this unrest")]
  public float unrestVelocity = 0.1f;

  [Tooltip("Spooky objects")]
  public SpookyObject[] spooks;

  [System.Serializable]
  public struct SpookyObject {
    public Transform transform;
    [Tooltip("Add this much of a spooks velocity to own velocity (depends on distance values)")]
    public float spookiness;
  }

  [Tooltip("Avoid spooks which reach this velocity (smoothDelta scaled distance from old position)")]
  public float spookVelocity = 0.2f;
  [Tooltip("Full spookiness below this distance")]
  public float minSpookyDist = 0.2f;
  [Tooltip("No spookiness above this distance")]
  public float maxSpookyDist = 1f;

  private Rigidbody rb;
  private Vector3[] oldPositions;

  // Start is called before the first frame update
  void Start() {
    rb = GetComponent<Rigidbody>();
    oldPositions = spooks.Map(s => s.transform.position);
  }

  // Update is called once per frame
  void Update() {
    for (int i = 0; i < spooks.Length; i++) {
      var distance = Vector3.Distance(spooks[i].transform.position, transform.position);
      if (distance > maxSpookyDist) continue;

      var oldPos = oldPositions[i];
      var velocity = (spooks[i].transform.position - oldPos) / Time.smoothDeltaTime;
      var speed = velocity.magnitude;
      if (speed >= spookVelocity) {
        if (distance < minSpookyDist) {
          rb.AddForce(velocity * spooks[i].spookiness);
          continue;
        }
        var spookiness = distance.Remap(minSpookyDist, maxSpookyDist, spooks[i].spookiness, 0);
        rb.AddForce(velocity * spookiness);

      }
    }
    foreach (var old in oldPositions) {
      var dist = Vector3.Distance(transform.position, old);
    }
  }

  void GetSpooked(Vector3 sourceOldPos, Vector3 sourceNewPos, float spookiness) {
    var vel = (sourceOldPos - sourceNewPos) * spookiness;
    rb.AddForce(vel);
  }

  void OnDrawGizmosSelected() {
    Gizmos.DrawSphere(transform.position, 0.1f);
  }
}
