using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class FoxController : MonoBehaviour {
  public PathTree network;

  [Tooltip("Time until fox comes to check out player")]
  public float awakeTimer = 5;

  [MyBox.Foldout("Animation")]
  [Tooltip("Time until fox comes to check out player")]
  public float restAnimDuration = 1;
  [MyBox.Foldout("Animation")]
  [Tooltip("Time until fox comes to check out player")]
  public float unrestAnimDuration = 1;

  [MyBox.Foldout("Animation")]
  public Vector3 unrestRotation;
  [MyBox.Foldout("Animation")]
  public AnimationCurve unrestRotationCurve;

  public float walkingSpeed = 1;
  public float runningSpeed = 2;


  [Tooltip("Run away if spooky thing at closer than this")]
  public float spookyDistance = 0.25f;
  public GameObject[] spookyThings;

  [Tooltip("Add objects from anywhere here that you want to be disabled when running away (eg. look constraint!)")]
  public Disabler disables;


  float sleepTime;
  float restStartTime;
  float restEndTime;

  [SerializeField]
  State state;
  enum State {
    sleep,
    walking,
    stopping,
    restingStart,
    resting,
    restingEnd,
    running,
  }

  Vector3 dir;
  [SerializeField]
  float velocity;
  [SerializeField]
  float fraction;
  [SerializeField]
  float overshoot;

  [SerializeField]
  PathTree.Branch source;
  [SerializeField]
  PathTree.Branch target;


  void Start() {
    Reset();
  }

  void Reset() {
    sleepTime = Time.time;
    state = State.sleep;
    source = network.network.branches[0];
    target = source.GetRandomNeighbor();
    transform.position = source.root.position;
    velocity = 0;
    fraction = 0;
    overshoot = 0;
    disables.EnableComponents();
  }

  void Update() {

    dir = target.root.position - source.root.position;
    transform.rotation = Quaternion.LookRotation(dir.normalized);

    switch (state) {
      case State.sleep:
        if (Time.time > sleepTime + awakeTimer)
          state = State.walking;
        break;

      case State.walking:
        velocity = Mathf.Lerp(velocity, walkingSpeed, Time.deltaTime);
        transform.position = source.MoveTowards(target, fraction, velocity * Time.deltaTime + overshoot, out overshoot, out fraction);
        if (fraction > 1) {
          source = target;
          target = source.GetRandomNeighbor();
          fraction = 0;
          transform.position = source.MoveTowards(target, fraction, overshoot, out overshoot, out fraction);

          if (target.root.GetComponent<FoxStopPoint>()) {
            state = State.stopping;
          }
        }
        break;

      case State.stopping:
        transform.position = source.MoveTowards(target, fraction, velocity * Time.deltaTime + overshoot, out overshoot, out fraction);
        if (fraction > 1) {
          StartRest();
        }
        break;

      case State.restingStart: {
          var fract = (Time.time - restStartTime) / restAnimDuration;
          if (fract >= 1)
            state = State.resting;
          break;
        }

      case State.resting:
        if (spookyThings.Any((g) => Vector3.Distance(transform.position, g.transform.position) < spookyDistance))
          Spook();
        break;

      case State.restingEnd: {
          var fract = (Time.time - restEndTime) / unrestAnimDuration;
          if (fract >= 1) {
            state = State.running;
          } else {
            transform.rotation *= Quaternion.Euler(unrestRotation * unrestRotationCurve.Evaluate(fract));
          }
          break;
        }

      case State.running:
        velocity = Mathf.Lerp(velocity, runningSpeed, Time.deltaTime);
        transform.position = source.MoveTowards(target, fraction, velocity * Time.deltaTime + overshoot, out overshoot, out fraction);
        if (fraction > 1) {
          source = target;
          target = source.GetRandomNeighbor();
          if (target == null) {
            Reset();
            return;
          }
          fraction = 0;
          transform.position = source.MoveTowards(target, fraction, overshoot, out overshoot, out fraction);
        }
        break;

      default:
        break;
    }

  }

  void StartRest() {
    state = State.restingStart;
    restStartTime = Time.time;
    source = target;
    target = source.GetRandomNeighbor();
    fraction = 0;
    overshoot = 0;
  }

  void Spook() {
    velocity = 0;
    state = State.restingEnd;
    restEndTime = Time.time;
    disables.DisableComponents();
  }
}
