using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PathTree : MonoBehaviour {

  public PathNetwork network { get => _network; }

  private PathNetwork _network;
  private int childCount;


  void Awake() {
    ReCalculate();
  }

  void Update() {
    if (!Application.isPlaying) {
      var prevCount = childCount;
      childCount = 0;
      foreach (var child in transform)
        childCount++;
      if (prevCount != childCount)
        ReCalculate();
    }
  }

  [MyBox.ButtonMethod]
  void ReCalculate() {
    List<(Transform start, Transform end)> lines = new List<(Transform start, Transform end)>();
    Transform startTransform = default(Transform);

    int i = 0;
    Transform start = default(Transform);
    foreach (Transform child in transform) {
      if (i == 0) startTransform = child;
      if ((i / 2f) % 1 == 0) { // Flip flop
        start = child;
      } else {
        lines.Add((start, child));
      }
      i++;
    }

    _network = new PathNetwork(startTransform, lines);
  }

  void OnDrawGizmos() {
    if (_network == null) return;
    foreach (var branch in _network.branches) {
      foreach (var neighbor in branch.neighbors) {
        var dir = neighbor.root.position - branch.root.position;
        DrawArrow.Gizmo(branch.root.position, neighbor.root.position - branch.root.position, Mathf.Min(0.25f, dir.magnitude / 3));
      }
    }
  }

  public class PathNetwork {
    public List<Branch> branches = new List<Branch>();

    public PathNetwork(Transform start, List<(Transform start, Transform end)> lines) {
      // Create neighborless branches
      foreach (var line in lines) {
        if (!branches.Exists(b => b.root == line.start))
          branches.Add(new Branch(line.start, new Branch[0]));
        // Add dead ends as branches
        if (!lines.Exists(l => l.start.position == line.end.position))
          branches.Add(new Branch(line.end, new Branch[0]));
      }
      // Find neighbors for branches
      foreach (var branch in branches) {
        // Find line which starts from branch root
        var neighbors = new List<Branch>();
        foreach (var line in lines) {
          if (line.start == branch.root) {
            // Find branch with root at line end
            foreach (var branch2 in branches) {
              if (line.end.position == branch2.root.position) {
                neighbors.Add(branch2);
              }
            }
          }
        }
        branch.neighbors = neighbors.ToArray();
      }
    }
  }

  public static Vector3 GetPosition(Branch source, Branch target, float fraction, out float overshootFraction) {
    var dir = target.root.position - source.root.position;
    overshootFraction = Mathf.Clamp(fraction - 1, 0, float.PositiveInfinity);
    return source.root.position + dir * fraction;
  }

  public static Vector3 MoveTowards(Branch source, Branch target, float fraction, float length, out float overshootLength, out float newFraction) {
    var dir = target.root.position - source.root.position;
    var moveFraction = fraction + length / dir.magnitude;
    var pos = GetPosition(source, target, moveFraction, out var overshootFraction);
    overshootLength = overshootFraction * dir.magnitude;
    newFraction = (pos - source.root.position).magnitude / dir.magnitude;
    return pos;
  }

  [System.Serializable]
  public class Branch {
    public Transform root;
    public Branch[] neighbors;

    public Branch(Transform root, Branch[] neighbors) {
      this.root = root;
      this.neighbors = neighbors;
    }

    public Branch GetRandomNeighbor() {
      if (neighbors.Length == 0) return null;
      return neighbors[Random.Range(0, neighbors.Length - 1)];
    }

    public Vector3 GetPosition(Branch target, float fraction, out float overshootFraction) {
      return PathTree.GetPosition(this, target, fraction, out overshootFraction);
    }

    public Vector3 MoveTowards(Branch target, float fraction, float length, out float overshootLength, out float newFraction) {
      return PathTree.MoveTowards(this, target, fraction, length, out overshootLength, out newFraction);
    }
  }
}
