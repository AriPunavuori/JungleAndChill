using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PathTree : MonoBehaviour {

  public PathNetwork network { get => _network; }

  private PathNetwork _network;
  private int childCount;


  void Update() {
    if (!Application.isPlaying) {
      var prevCount = childCount;
      foreach (var child in transform)
        childCount++;
      if (prevCount != childCount)
        ReCalculate();
    }
  }

  [MyBox.ButtonMethod]
  void ReCalculate() {
    List<Line> lines = new List<Line>();
    Transform startTransform = default(Transform);

    int i = 0;
    Transform start = default(Transform);
    foreach (Transform child in transform) {
      if (i == 0) startTransform = child;
      if ((i / 2f) % 1 == 0) { // Flip flop
        start = child;
      } else {
        lines.Add(new Line(start, child));
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

    public PathNetwork(Transform start, List<Line> lines) {
      // Create neighborless branches
      foreach (var line in lines) {
        if (!branches.Exists(b => b.root == line.start))
          branches.Add(new Branch(line.start, new Branch[0]));
        // Naively add all ends as branches and remove later
        if (!lines.Exists(l => l.start == line.end))
          branches.Add(new Branch(line.end, new Branch[0]));
      }
      // Find unnecessary neighborless branches
      var remove = new List<Branch>();
      foreach (var branch in branches) {
        if (branch.neighbors.Length == 0 && branches.Exists(b => b.root == branch.root && branch != b))
          remove.Add(branch);
      }
      // Remove unnecessary branches
      foreach (var branch in remove) {
        branches.Remove(branch);
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
                break;
              }
            }
          }
        }
        branch.neighbors = neighbors.ToArray();
      }
    }
  }

  public class Branch {
    public Transform root;
    public Branch[] neighbors;

    public Branch(Transform root, Branch[] neighbors) {
      this.root = root;
      this.neighbors = neighbors;
    }
  }

  public struct Line {
    public Transform start;
    public Transform end;

    public Line(Transform start, Transform end) {
      this.start = start;
      this.end = end;
    }
  }
}
