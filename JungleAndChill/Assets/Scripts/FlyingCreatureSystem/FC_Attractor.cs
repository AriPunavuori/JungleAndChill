using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

[RequireComponent(typeof(Rigidbody))]
public class FC_Attractor : MonoBehaviour {

  [Tooltip("Spooky objects")]
  public SpookyObject[] spooks;

  [System.Serializable]
  public struct SpookyObject {
    public Transform transform;
    [Tooltip("Add this much of a spooks velocity to own velocity (depends on distance values)")]
    public float spookiness;
  }

  [Tooltip("Avoid spooks which reach this velocity (fixed delta scaled distance from old position)")]
  public float spookVelocity = 0.2f;
  [Tooltip("Full spookiness below this distance")]
  public float minSpookyDist = 0.2f;
  [Tooltip("No spookiness above this distance")]
  public float maxSpookyDist = 1f;

  [Tooltip("Automatically search for bounds in children of parent")]
  public bool FindBoundsInSiblings = true;
  [Tooltip("Manually find bounding boxes in children of parent")]
  public FC_BoundingBox[] boundingBoxes;
  [Tooltip("Manually find bounding spheres in children of parent")]
  public FC_BoundingSphere[] boundingSpheres;
  [PositiveValueOnly]
  [Tooltip("Amount of velocity converted to force towards previously in bounds position")]
  public float inboundsForce;


  private Rigidbody rb;
  private Vector3[] oldPositions;
  private Vector3 prevInbounds;

  // Start is called before the first frame update
  void Start() {
    rb = GetComponent<Rigidbody>();
    oldPositions = spooks.Map(s => s.transform.position);
    prevInbounds = transform.position;
    if (FindBoundsInSiblings) {
      var foundBoxes = transform.parent.GetComponentsInChildren<FC_BoundingBox>();
      var foundSpheres = transform.parent.GetComponentsInChildren<FC_BoundingSphere>();

      boundingBoxes = boundingBoxes.Concat(foundBoxes);
      boundingSpheres = boundingSpheres.Concat(foundSpheres);
    }
  }

  // Update is called once per frame
  void FixedUpdate() {
    CheckSpooks();
    UpdateVelocity();
    ApplyBounds();
  }


  void UpdateVelocity() {
    if (rb.velocity == Vector3.zero) {
      rb.velocity = Vector3.one * 0.01f;
    }
  }

  bool ApplyBounds() {
    if (IsInBounds()) {
      prevInbounds = transform.position;
      return false;
    } else {
      rb.velocity = rb.velocity + (prevInbounds - transform.position) * inboundsForce * Time.deltaTime;
      return true;
    }
  }

  bool IsInBounds() {
    foreach (var box in boundingBoxes)
      if (!box.isActiveAndEnabled || box.Inside(transform.position)) return true;

    foreach (var sphere in boundingSpheres)
      if (!sphere.isActiveAndEnabled || sphere.Inside(transform.position)) return true;

    return false;
  }

  void CheckSpooks() {
    for (int i = 0; i < spooks.Length; i++) {
      var distance = Vector3.Distance(spooks[i].transform.position, transform.position);
      if (distance > maxSpookyDist) continue;

      var oldPos = oldPositions[i];
      oldPositions[i] = spooks[i].transform.position;
      var velocity = (spooks[i].transform.position - oldPos) / Time.deltaTime;
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
  }

  void GetSpooked(Vector3 sourceOldPos, Vector3 sourceNewPos, float spookiness) {
    var vel = (sourceOldPos - sourceNewPos) * spookiness;
    rb.AddForce(vel);
  }
}
