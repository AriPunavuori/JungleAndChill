using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallReturn : MonoBehaviour{
    Vector3 returnPoint;
    Rigidbody rb;
    float returnTime;
    public float returnTimer;
    bool ballThrown;
    private void Start() {
        returnTime = GameObject.Find("DaytimeSwitcher").GetComponent<DaytimeSwitcher>().switchTime;
        returnPoint = transform.position;
        returnTimer = returnTime;
        rb = GetComponent<Rigidbody>();
    }
    private void Update() {
        if(ballThrown)
            returnTimer -= Time.deltaTime;
        if (returnTimer < 0) {
            rb.MovePosition(returnPoint);
            ballThrown = false;
        }
    }
    public void BallThrown() {
        returnTimer = returnTime;
        ballThrown = true;
    }
}
