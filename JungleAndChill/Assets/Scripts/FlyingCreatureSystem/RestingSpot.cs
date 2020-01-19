using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestingSpot : MonoBehaviour {
  public int maxResters = 1;
  private List<Transform> resters = new List<Transform>();

  [HideInInspector]
  public Vector3 prevPos;

  void LateUpdate() {
    prevPos = transform.position;
  }

  public void AddRester(Transform t) {
    if (!resters.Contains(t))
      resters.Add(t);
  }

  public void RemoveRester(Transform t) {
    if (resters.Contains(t))
      resters.Remove(t);
  }

  public bool IsFull() {
    return resters.Count >= maxResters;
  }

  void OnDrawGizmosSelected() {
    Gizmos.color = Color.cyan;
    Gizmos.DrawSphere(transform.position, 0.02f);
    DrawArrow.Gizmo(transform.position, transform.forward * 0.333f, Color.magenta, 0.1f);
  }
}
