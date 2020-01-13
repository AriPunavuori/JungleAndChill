using UnityEngine;

public static class IntExtensions {
  public static int RoundToNearest(this int integer, int nearest) => Mathf.RoundToInt(integer / nearest) * nearest;
}
