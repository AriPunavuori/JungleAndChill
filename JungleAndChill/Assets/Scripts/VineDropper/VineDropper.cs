using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VineDropper : MonoBehaviour
{
    public GameObject vine;
    Vector3 target;
    float dropTime = 5f;
    public bool first;
    public bool second;
    public bool third;
    bool startVineDropping;

    void Start() {
        //vine = GameObject.Find("Vine");
        target = vine.transform.position + Vector3.down * 3;
    }

    private void OnTriggerEnter(Collider other) {
        print("osui");
        if(other.tag == "FirstVineObject")
            first = true;
        if(other.tag == "SecondVineObject")
            second = true;
        if(other.tag == "ThirdVineObject")
            third = true;
        if(first && second && third) {
            StartCoroutine("MoveToPosition");
            print("Start Dropping");
        }
    }
    public IEnumerator MoveToPosition() {
        var currentPos = vine.transform.position;
        var t = 0f;
        while(t < 1) {
            t += Time.deltaTime / dropTime;
            vine.transform.position = Vector3.Lerp(currentPos, target, t);
            yield return null;
        }
    }
}
