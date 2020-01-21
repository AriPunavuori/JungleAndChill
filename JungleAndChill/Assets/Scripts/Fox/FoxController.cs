using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class FoxController : MonoBehaviour {
  public PathTree network;

  [Tooltip("Time until fox comes to check out player")]
  public float awakeTimer = 5;

  [MyBox.Foldout("Animation")]
  public Animator animator;
  [MyBox.Foldout("Animation")]
  [Tooltip("Time until fox comes to check out player")]
  public float restAnimDuration = 1;
  [MyBox.Foldout("Animation")]
  [Tooltip("Time until fox comes to check out player")]
  public float unrestAnimDuration = 1;
  [Tooltip("Time given for jump animation to finish")]
  public float disappearDuration = 1;

  [MyBox.Foldout("Animation")]
  public Vector3 unrestRotation;
  [MyBox.Foldout("Animation")]
  public AnimationCurve unrestRotationCurve;

  public float walkingSpeed = 1;
  public float runningSpeed = 2;
  [Header("The end of this curve should not be at value 0 or the fox will never reach the target!!!")]
  public AnimationCurve stopSpeedCurve = AnimationCurve.EaseInOut(0, 1, 1, 0.05f);

  [Tooltip("Movement will be smooth regardless of this value. Curve length and visuals may be affected though")]
  [Range(2, 50)]
  public int bezierSegments = 5;

  [Tooltip("Run away if spooky thing at closer than this")]
  public float spookyDistance = 0.50f;
  public GameObject[] spookyThings;

  [Tooltip("Add objects from anywhere here that you want to be disabled when running away (eg. look constraint!)")]
  public Disabler disables;

  State state;
  enum State {
    sleep,
    walking,
    stopping,
    restingStart,
    resting,
    restingEnd,
    running,
    vanish,
  }

  Vector3 dir;
  Vector3 prevPos;
  float velocity;
  float fraction;
  float overshoot;

  PathTree.Branch branch1;
  PathTree.Branch branch2;
  PathTree.Branch branch3;
  PathTree.Branch branch4;


  float sleepTime;
  float restStartTime;
  float restEndTime;
  float disappearTime;

  List<Vector3> points;
  float bezierLength;

  void Start() {
    if (animator == null)
      animator = GetComponent<Animator>();
    Reset();
  }

  void Reset() {
    branch1 = network.network.branches[0];
    branch2 = branch1.GetRandomNeighbor();
    branch3 = branch2.GetRandomNeighbor();
    branch4 = branch3.GetRandomNeighbor();
    points = GetPoints();

    velocity = 0;
    fraction = 0;
    overshoot = 0;
    sleepTime = Time.time;
    state = State.sleep;
    transform.position = branch1.root.position;
    prevPos = transform.position;

    SetAnimation(Anims.None);
    disables.EnableComponents();
  }

  void Update() {
    if (branch2 == null) {
      Reset();
      return;
    }
    dir = transform.position - prevPos;
    if (dir != Vector3.zero && state != State.vanish)
      transform.rotation = Quaternion.LookRotation(dir.normalized);
    prevPos = transform.position;

    animator.SetFloat("Speed", velocity);

    switch (state) {
      case State.sleep:
        if (Time.time > sleepTime + awakeTimer)
          state = State.walking;
        break;

      case State.walking:
        SetAnimation(Anims.WalkAway);
        velocity = Mathf.Lerp(velocity, walkingSpeed, Time.deltaTime);
        MoveAlongPoints(velocity * Time.deltaTime + overshoot);
        if (fraction > 1) {
          AdvanceTree();
          MoveAlongPoints(overshoot);

          var comp = branch2.root.GetComponent<BranchComponent>();
          if (comp != null && comp.SitPoint && !comp.Dummy) {
            state = State.stopping;
          }
        }
        break;

      case State.stopping:
        velocity = Mathf.Min(velocity, stopSpeedCurve.Evaluate(fraction) * walkingSpeed);
        MoveAlongPoints(velocity * Time.deltaTime + overshoot);
        if (fraction > 1) {
          StartRest();
        }
        break;

      case State.restingStart: {
          SetAnimation(Anims.Sit);
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
          SetAnimation(Anims.WalkAway);
          var fract = (Time.time - restEndTime) / unrestAnimDuration;
          if (fract >= 1) {
            state = State.running;
          } else {
            transform.rotation *= Quaternion.Euler(unrestRotation * unrestRotationCurve.Evaluate(fract));
          }
          break;
        }

      case State.running:
        SetAnimation(Anims.WalkAway);
        velocity = Mathf.Lerp(velocity, runningSpeed, Time.deltaTime);
        MoveAlongPoints(velocity * Time.deltaTime + overshoot);
        if (fraction > 1) {
          AdvanceTree();
          MoveAlongPoints(overshoot);
        }
        break;

      case State.vanish: {
          SetAnimation(Anims.Jump);
          var fract = (Time.time - disappearTime) / disappearDuration;
          if (fract >= 1) {
            Reset();
          }
          break;
        }
    }
  }

  void AdvanceTree() {
    branch1 = branch2;
    branch2 = branch1.GetRandomNeighbor();
    branch3 = branch2.GetRandomNeighbor();
    if (branch2 == null || branch3 == null) {
      disappearTime = Time.time;
      state = State.vanish;
      return;
    }
    branch4 = branch3.GetRandomNeighbor();
    points = GetPoints();

    fraction = 0;
  }

  List<Vector3> GetPoints() {
    if (branch1 == network.network.branches[0])
      return GetBezierPoints(branch1.position, branch2.position, branch3.position, out bezierLength, bezierSegments);
    if (branch4 == null)
      return GetBezierPoints(Vector3.Lerp(branch1.position, branch2.position, 0.5f), branch2.position, branch3.position, out bezierLength, bezierSegments);
    return GetBezierPoints(Vector3.Lerp(branch1.position, branch2.position, 0.5f), branch2.position, Vector3.Lerp(branch2.position, branch3.position, 0.5f), out bezierLength, bezierSegments);
  }

  public void MoveAlongPoints(float amount) {
    var preFrac = fraction;
    fraction = fraction + amount / bezierLength;
    transform.position = PointInBezier(fraction);
    // overshoot = Mathf.Max(0, (1 - fraction) * bezierLength);
  }

  static List<Vector3> GetBezierPoints(Vector3 start, Vector3 control, Vector3 end, out float length, int segments = 10) {
    var res = new List<Vector3>();
    length = 0;
    for (int i = 0; i < segments + 1; i++) {
      float t = i / (float)segments;
      res.Add(BezierPoint(start, control, end, t));
      if (i > 0) length += Vector3.Distance(res[i - 1], res[i]);
    }
    return res;
  }

  Vector3 PointInBezier(float t) {
    t = Mathf.Clamp01(t);
    if (branch1 == network.network.branches[0])
      return BezierPoint(branch1.position, branch2.position, branch3.position, t);
    if (branch4 == null)
      return BezierPoint(Vector3.Lerp(branch1.position, branch2.position, 0.5f), branch2.position, branch3.position, t);
    return BezierPoint(Vector3.Lerp(branch1.position, branch2.position, 0.5f), branch2.position, Vector3.Lerp(branch2.position, branch3.position, 0.5f), t);

  }

  static Vector3 BezierPoint(Vector3 start, Vector3 control, Vector3 end, float t) {
    Vector3 m1 = Vector3.Lerp(start, control, t);
    Vector3 m2 = Vector3.Lerp(control, end, t);
    return Vector3.Lerp(m1, m2, t);
  }

  void OnDrawGizmos() {
    if (network.network == null) return;

    Gizmos.color = Color.magenta;

    foreach (var branch in network.network.branches) {
      DrawBranch(branch);
    }

    void DrawBranch(PathTree.Branch branch1) { // Start
      bool isStart = branch1.Equals(network.network.branches[0]);
      foreach (var branch2 in branch1.neighbors) { // Control
        foreach (var branch3 in branch2.neighbors) { // Target
          if (branch3.neighbors.Length == 0)
            DrawPoints(
              GetBezierPoints(
                isStart ? branch1.position : Vector3.Lerp(branch1.position, branch2.position, 0.5f),
                branch2.position,
                branch3.position,
                out var l,
                bezierSegments
              )
            );
          else
            DrawPoints(
              GetBezierPoints(
                isStart ? branch1.position : Vector3.Lerp(branch1.position, branch2.position, 0.5f),
                branch2.position,
                Vector3.Lerp(branch2.position, branch3.position, 0.5f),
                out var l,
                bezierSegments
              )
            );
        }
      }
      void DrawPoints(List<Vector3> points) {
        var prevPoint = points[0];
        foreach (var point in points) {
          Gizmos.DrawLine(prevPoint, point);
          prevPoint = point;
        }
        DrawArrow.Gizmo(points[points.Count - 2], points[points.Count - 1] - points[points.Count - 2]);
      }
    }
  }

  void StartRest() {
    branch1 = branch2;
    branch2 = branch1.GetRandomNeighbor();
    branch3 = branch2.GetRandomNeighbor();
    branch4 = branch3.GetRandomNeighbor();
    points = GetPoints();

    state = State.restingStart;
    restStartTime = Time.time;
    fraction = 0;
    overshoot = 0;
  }

  void Spook() {
    velocity = 0;
    state = State.restingEnd;
    restEndTime = Time.time;
    disables.DisableComponents();
  }

  void SetAnimation(Anims anim) {
    animator.SetBool("WalkAway", anim == Anims.WalkAway);
    animator.SetBool("Sit", anim == Anims.Sit);
    animator.SetBool("Jump", anim == Anims.Jump);
  }
  private enum Anims {
    None,
    WalkAway,
    Sit,
    Jump,
  }

}
