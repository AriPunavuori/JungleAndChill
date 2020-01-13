using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FC_Attractor : MonoBehaviour {

  public FC_RestingSpot[] restingSpots;
  public bool resting = false;

  // Start is called before the first frame update
  void Start() {

  }

  // Update is called once per frame
  void Update() {

  }

  void OnDrawGizmosSelected() {
    Gizmos.DrawSphere(transform.position, 0.1f);
  }
}
