using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FC_BoundingSphere : MonoBehaviour {
  public float dist;

  public bool Inside(Vector3 point) {
    return Vector3.Distance(point, transform.position) <= dist;
  }

  void OnDrawGizmos() {
    Gizmos.color = new Color(1, 0, 1, 0.2f);
    Gizmos.DrawSphere(transform.position, dist);
  }
}
