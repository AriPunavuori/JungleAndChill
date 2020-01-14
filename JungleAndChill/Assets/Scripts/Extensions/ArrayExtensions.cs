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

  public delegate R MapFunc1<R, T>(T current);
  public delegate R MapFunc2<R, T>(T current, int index);
  public delegate R MapFunc3<R, T>(T current, int index, T[] array);

  /// <summary> Returns the resulting array if `Func` is ran on each element </summary>
  public static R[] Map<T, R>(this T[] array, MapFunc1<R, T> func) {
    R[] res = new R[array.Length];
    for (int i = 0; i < array.Length; i++) {
      res[i] = func(array[i]);
    }
    return res;
  }
  /// <summary> Returns the resulting array if `Func` is ran on each element </summary>
  public static R[] Map<T, R>(this T[] array, MapFunc2<R, T> func) {
    R[] res = new R[array.Length];
    for (int i = 0; i < array.Length; i++) {
      res[i] = func(array[i], i);
    }
    return res;
  }
  /// <summary> Returns the resulting array if `Func` is ran on each element </summary>
  public static R[] Map<T, R>(this T[] array, MapFunc3<R, T> func) {
    R[] res = new R[array.Length];
    for (int i = 0; i < array.Length; i++) {
      res[i] = func(array[i], i, array);
    }
    return res;
  }


  public delegate bool AnyFunc1<R, T>(T current);
  public delegate bool AnyFunc2<R, T>(T current, int index);
  public delegate bool AnyFunc3<R, T>(T current, int index, T[] array);

  /// <summary> Returns true if `Func` returns true for any array element </summary>
  public static bool Any<T, R>(this T[] array, AnyFunc1<R, T> func) {
    for (int i = 0; i < array.Length; i++) {
      if (func(array[i])) return true;
    }
    return false;
  }
  /// <summary> Returns true if `Func` returns true for any array element </summary>
  public static bool Any<T, R>(this T[] array, AnyFunc2<R, T> func) {
    for (int i = 0; i < array.Length; i++) {
      if (func(array[i], i)) return true;
    }
    return false;
  }
  /// <summary> Returns true if `Func` returns true for any array element </summary>
  public static bool Any<T, R>(this T[] array, AnyFunc3<R, T> func) {
    for (int i = 0; i < array.Length; i++) {
      if (func(array[i], i, array)) return true;
    }
    return false;
  }

}
