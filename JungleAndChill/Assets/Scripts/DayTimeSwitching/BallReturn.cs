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
    float autoReturn = 10f;
    public float returnTimer = 10f;
    bool ballThrown;
    bool ballHit;

    private void Start() {
        ds = GameObject.Find("DaytimeSwitcher").GetComponent<DaytimeSwitcher>();
        returnTime = ds.switchTime;
        returnPoint = transform.position;
        rb = GetComponent<Rigidbody>();
    }

    private void Update() {
        if (ballThrown) {
            returnTimer -= Time.deltaTime;
            if (!ballHit && Vector3.Distance(throwPoint, transform.position) > triggerDistance) {
                ballHit = true;
                ds.SwitchDaytime();
                returnTimer = returnTime;
            }
            if (returnTimer < 0) {
                ReturnBall();
            }
        }
    }

    public void BallThrown() {
        throwPoint = transform.position;
        ballThrown = true;
    }

    void ReturnBall() {
        returnTimer = autoReturn;
        rb.MovePosition(returnPoint);
        rb.useGravity = true;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        ballThrown = false;
        ballHit = false;
    }
}
