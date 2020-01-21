using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class BranchComponent : MonoBehaviour {
  public bool SitPoint = false;
  [MyBox.ReadOnly]
  public bool Dummy = false;

  [HideInInspector] public Vector3 prevPos;
  [HideInInspector] public PathTree tree;


  [MyBox.ButtonMethod]
  void AttachToNearestNode() {
    var transforms = tree.GetComponentsInChildren<Transform>();

    Transform minTransform = null;
    var minDist = float.PositiveInfinity;

    foreach (var t in transforms) {
      if (t == transform || t == tree.transform || t.position == transform.position) continue;
      var dist = Vector3.Distance(transform.position, t.position);
      if (dist < minDist) {
        minTransform = t;
        minDist = dist;
      }
    }
    if (minTransform != null) {
      Undo.RecordObject(transform, "Move node to closest");
      transform.position = minTransform.position;
    }
  }

  void OnValidate() {
    prevPos = transform.position;
  }

  void OnDrawGizmos() {
    if (Dummy) {
      Gizmos.DrawIcon(transform.position, "Branch Dummy", true, Color.white * 0.1f);
    } else if (SitPoint) {
      Gizmos.DrawIcon(transform.position, "Branch Sit Point", true, Color.white * 0.1f);
    } else {
      Gizmos.DrawIcon(transform.position, "Branch Normal", true, Color.white * 0.1f);
    }
  }

  void Update() {
    if (transform.position != prevPos) {
      prevPos = transform.position;
      if (tree != null)
        tree.ReCalculate();
    }
  }
}
