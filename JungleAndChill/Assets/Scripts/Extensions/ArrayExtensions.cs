using System.Collections;
using System.Collections.Generic;

static class ArrayExtensions {
  /// <summary> Returns true if `array` has `value` </summary>
  public static bool Includes<T>(this T[] array, T value) {
    int length = array.Length;
    for (int i = 0; i < length; i++)
      if (array[i].Equals(value)) return true;
    return false;
  }
  /// <summary> Returns true if `array` has `value` and passes the found position with `index` which is -1 when not found </summary>
  public static bool Includes<T>(this T[] array, T value, out int index) {
    index = -1;
    int length = array.Length;
    for (int i = 0; i < length; i++)
      if (array[i].Equals(value)) {
        index = i;
        return true;
      }
    return false;
  }

  /// <summary> Reverses the array or part of it in place </summary>
  public static void Reverse<T>(this T[] array, int index, int length) => System.Array.Reverse(array, index, length);
  public static void Reverse<T>(this T[] array) => System.Array.Reverse(array);

  public delegate R MapCallback1<R, T>(T current);
  public delegate R MapCallback2<R, T>(T current, int index);
  public delegate R MapCallback3<R, T>(T current, int index, T[] array);

  /// <summary> Returns the resulting array if func is ran on each element </summary>
  public static R[] Map<T, R>(this T[] array, MapCallback1<R, T> callback) {
    R[] res = new R[array.Length];
    for (int i = 0; i < array.Length; i++) {
      res[i] = callback(array[i]);
    }
    return res;
  }
  /// <summary> Returns the resulting array if func is ran on each element </summary>
  public static R[] Map<T, R>(this T[] array, MapCallback2<R, T> callback) {
    R[] res = new R[array.Length];
    for (int i = 0; i < array.Length; i++) {
      res[i] = callback(array[i], i);
    }
    return res;
  }
  /// <summary> Returns the resulting array if func is ran on each element </summary>
  public static R[] Map<T, R>(this T[] array, MapCallback3<R, T> callback) {
    R[] res = new R[array.Length];
    for (int i = 0; i < array.Length; i++) {
      res[i] = callback(array[i], i, array);
    }
    return res;
  }
  /*
  
callback
  Function that produces an element of the new Array, taking three arguments:
currentValue
  The current element being processed in the array.
indexOptional
  The index of the current element being processed in the array.
arrayOptional
  The array map was called upon.
thisArgOptional
  Value to use as this when executing callback.
*/

}
