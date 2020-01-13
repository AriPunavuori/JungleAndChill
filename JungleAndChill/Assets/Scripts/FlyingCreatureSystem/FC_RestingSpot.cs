using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FC_RestingSpot : MonoBehaviour {
  // Start is called before the first frame update
  void Start() {

  }

  // Update is called once per frame
  void Update() {

  }

  void OnDrawGizmosSelected() {
    Gizmos.DrawCube(transform.position, new Vector3(0.1f, -0.01f, 0.1f));
  }
}
