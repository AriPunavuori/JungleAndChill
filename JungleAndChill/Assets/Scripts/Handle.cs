using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Handle : MonoBehaviour {
  void OnDrawGizmosSelected() {
    Gizmos.color = Color.cyan;
    Gizmos.DrawSphere(transform.position, 0.02f);
    DrawArrow.Gizmo(transform.position, transform.forward * 0.333f, Color.cyan, 0.1f);
  }
}
