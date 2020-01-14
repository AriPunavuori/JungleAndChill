using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FC_BoundingBox : MonoBehaviour {
  public Vector3 bounds;

  public bool Inside(Vector3 point) {
    if (point.x < transform.position.x - bounds.x / 2 || point.x > transform.position.x + bounds.x / 2) return false;
    if (point.y < transform.position.y - bounds.y / 2 || point.y > transform.position.y + bounds.y / 2) return false;
    if (point.z < transform.position.z - bounds.z / 2 || point.z > transform.position.z + bounds.z / 2) return false;
    return true;
  }

  void OnDrawGizmos() {
    Gizmos.color = new Color(1, 0, 1, 0.2f);
    Gizmos.DrawCube(transform.position, bounds);
    Gizmos.color = Color.magenta;
    Gizmos.DrawWireCube(transform.position, bounds);
  }
}
