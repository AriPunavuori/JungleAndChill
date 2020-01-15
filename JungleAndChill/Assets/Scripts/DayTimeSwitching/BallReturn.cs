using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallReturn : MonoBehaviour{
    Vector3 returnPoint;
    Vector3 throwPoint;
    DaytimeSwitcher ds;
    public float triggerDistance = 5f;
    Rigidbody rb;
    float returnTime;
    public float returnTimer;
    bool ballThrown;
    private void Start() {
        ds = GameObject.Find("DaytimeSwitcher").GetComponent<DaytimeSwitcher>();
        returnTime = ds.switchTime;
        returnPoint = transform.position;
        returnTimer = returnTime;
        rb = GetComponent<Rigidbody>();
    }
    private void Update() {
        if (ballThrown) {
            returnTimer -= Time.deltaTime;
            if (Vector3.Distance(throwPoint, transform.position) > triggerDistance) {
                ds.SwitchDaytime();
            }
        }
  
        if (returnTimer < 0) {
            rb.velocity = Vector3.zero;
            rb.MovePosition(returnPoint);
            ballThrown = false;
        }
    }
    public void BallThrown() {
        returnTimer = returnTime;
        throwPoint = transform.position;
        ballThrown = true;
    }
}
