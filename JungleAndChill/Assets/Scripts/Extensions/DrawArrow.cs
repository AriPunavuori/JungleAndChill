using UnityEngine;
using System.Collections;

/**
https://forum.unity.com/threads/debug-drawarrow.85980/
*/
public static class DrawArrow {
  public static void Gizmo(Vector3 pos, Vector3 direction, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f) {
    Gizmos.DrawRay(pos, direction);

    Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * new Vector3(0, 0, 1);
    Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * new Vector3(0, 0, 1);
    Gizmos.DrawRay(pos + direction, right * arrowHeadLength);
    Gizmos.DrawRay(pos + direction, left * arrowHeadLength);
  }

  public static void Gizmo(Vector3 pos, Vector3 direction, Color color, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f) {
    Gizmos.color = color;
    Gizmos.DrawRay(pos, direction);

    Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * new Vector3(0, 0, 1);
    Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * new Vector3(0, 0, 1);
    Gizmos.DrawRay(pos + direction, right * arrowHeadLength);
    Gizmos.DrawRay(pos + direction, left * arrowHeadLength);
  }

  public static void Debug(Vector3 pos, Vector3 direction, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f) {
    UnityEngine.Debug.DrawRay(pos, direction);

    Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * new Vector3(0, 0, 1);
    Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * new Vector3(0, 0, 1);
    UnityEngine.Debug.DrawRay(pos + direction, right * arrowHeadLength);
    UnityEngine.Debug.DrawRay(pos + direction, left * arrowHeadLength);
  }
  public static void Debug(Vector3 pos, Vector3 direction, Color color, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f) {
    UnityEngine.Debug.DrawRay(pos, direction, color);

    Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * new Vector3(0, 0, 1);
    Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * new Vector3(0, 0, 1);
    UnityEngine.Debug.DrawRay(pos + direction, right * arrowHeadLength, color);
    UnityEngine.Debug.DrawRay(pos + direction, left * arrowHeadLength, color);
  }
}
