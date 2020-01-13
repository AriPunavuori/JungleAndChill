using UnityEngine;

public static class FloatExtensions {
  public static float RoundToNearest(this float integer, float nearest) => Mathf.Round(integer / nearest) * nearest;
}