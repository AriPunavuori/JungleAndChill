using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestingSpot : MonoBehaviour {
  void OnDrawGizmosSelected() {
    Gizmos.color = Color.cyan;
    Gizmos.DrawSphere(transform.position, 0.02f);
    DrawArrow.Gizmo(transform.position, transform.forward * 0.333f, Color.magenta, 0.1f);
  }
}
