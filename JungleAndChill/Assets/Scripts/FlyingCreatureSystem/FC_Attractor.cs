using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

[RequireComponent(typeof(Rigidbody))]
public class FC_Attractor : MonoBehaviour {

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
  private Vector3 prevInbounds;



  // Start is called before the first frame update
  void Start() {
    rb = GetComponent<Rigidbody>();
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

}
