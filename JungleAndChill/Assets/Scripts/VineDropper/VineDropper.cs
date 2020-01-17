using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VineDropper : MonoBehaviour
{
    GameObject vine;
    Vector3 target;
    float dropSpeed;
    bool first;
    bool second;
    bool third;
    bool startVineDropping;

    void Start() {
        vine = GameObject.Find("Vine");
        target = vine.transform.position + Vector3.down * 5;
    }

    private void OnTriggerEnter(Collider other) {
        if(other.tag == "FirstVineObject")
            first = true;
        if(other.tag == "SecondVineObject")
            second = true;
        if(other.tag == "ThirdVineObject")
            third = true;
        if(first && second && third)
            StartCoroutine("MoveToPosition");
    }
    public IEnumerator MoveToPosition(Transform transform, Vector3 position, float timeToMove) {
        var currentPos = transform.position;
        var t = 0f;
        while(t < 1) {
            t += Time.deltaTime / timeToMove;
            transform.position = Vector3.Lerp(currentPos, position, t);
            yield return null;
        }
    }
}
