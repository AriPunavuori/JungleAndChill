using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeParentOnAwake : MonoBehaviour {
  [Tooltip("Can be null for scene root")]
  public Transform parent;
  // Start is called before the first frame update
  void Awake() {
    transform.parent = parent;
  }
}
