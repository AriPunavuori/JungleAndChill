// ---------------------------------------------------------------------------- 
// Author: Richard Fine
// Source: https://bitbucket.org/richardfine/scriptableobjectdemo
// ----------------------------------------------------------------------------

using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

#endif

namespace MyBox {
  public class MinMaxRangeAttribute : PropertyAttribute {
    public MinMaxRangeAttribute(float min, float max) {
      this.min = min;
      this.max = max;
    }

    public readonly float min;
    public readonly float max;
  }

  [Serializable]
  public struct RangedFloat {
    public float min;
    public float max;

    public RangedFloat(float min, float max) {
      this.min = min;
      this.max = max;
    }
  }

  [Serializable]
  public struct RangedInt {
    public int min;
    public int max;

    public RangedInt(int min, int max) {
      this.min = min;
      this.max = max;
    }
  }

  public static class RangedExtensions {
    public static float LerpFromRange(this RangedFloat ranged, float t) {
      return Mathf.Lerp(ranged.min, ranged.max, t);
    }

    public static float LerpFromRangeUnclamped(this RangedFloat ranged, float t) {
      return Mathf.LerpUnclamped(ranged.min, ranged.max, t);
    }

    public static float LerpFromRange(this RangedInt ranged, float t) {
      return Mathf.Lerp(ranged.min, ranged.max, t);
    }

    public static float LerpFromRangeUnclamped(this RangedInt ranged, float t) {
      return Mathf.LerpUnclamped(ranged.min, ranged.max, t);
    }
  }
}

#if UNITY_EDITOR
namespace MyBox.Internal {
  [CustomPropertyDrawer(typeof(MinMaxRangeAttribute))]
  public class MinMaxRangeIntAttributeDrawer : PropertyDrawer {
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
      SerializedProperty minProp = property.FindPropertyRelative("min");
      SerializedProperty maxProp = property.FindPropertyRelative("max");
      if (minProp == null || maxProp == null) {
        WarningsPool.Log("MinMaxRangeAttribute used on <color=brown>" +
                         property.name +
                         "</color>. Must be used on types with min and max fields",
          property.serializedObject.targetObject);

        return;
      }

      var minValid = minProp.propertyType == SerializedPropertyType.Integer || minProp.propertyType == SerializedPropertyType.Float;
      var maxValid = maxProp.propertyType == SerializedPropertyType.Integer || maxProp.propertyType == SerializedPropertyType.Float;
      if (!maxValid || !minValid || minProp.propertyType != maxProp.propertyType) {
        WarningsPool.Log("MinMaxRangeAttribute used on <color=brown>" +
                         property.name +
                         "</color>. min and max fields must be of int or float type",
          property.serializedObject.targetObject);

        return;
      }

      MinMaxRangeAttribute rangeAttribute = (MinMaxRangeAttribute)attribute;

      label = EditorGUI.BeginProperty(position, label, property);
      position = EditorGUI.PrefixLabel(position, label);

      bool isInt = minProp.propertyType == SerializedPropertyType.Integer;

      float minValue = isInt ? minProp.intValue : minProp.floatValue;
      float maxValue = isInt ? maxProp.intValue : maxProp.floatValue;
      float rangeMin = rangeAttribute.min;
      float rangeMax = rangeAttribute.max;


      const float rangeBoundsLabelWidth = 40f;

      var rangeBoundsLabel1Rect = new Rect(position);
      rangeBoundsLabel1Rect.width = rangeBoundsLabelWidth;
      GUI.Label(rangeBoundsLabel1Rect, new GUIContent(minValue.ToString(isInt ? "F0" : "F2")));
      position.xMin += rangeBoundsLabelWidth;

      var rangeBoundsLabel2Rect = new Rect(position);
      rangeBoundsLabel2Rect.xMin = rangeBoundsLabel2Rect.xMax - rangeBoundsLabelWidth;
      GUI.Label(rangeBoundsLabel2Rect, new GUIContent(maxValue.ToString(isInt ? "F0" : "F2")));
      position.xMax -= rangeBoundsLabelWidth;

      EditorGUI.BeginChangeCheck();
      EditorGUI.MinMaxSlider(position, ref minValue, ref maxValue, rangeMin, rangeMax);

      if (EditorGUI.EndChangeCheck()) {
        if (isInt) {
          minProp.intValue = Mathf.RoundToInt(minValue);
          maxProp.intValue = Mathf.RoundToInt(maxValue);
        } else {
          minProp.floatValue = minValue;
          maxProp.floatValue = maxValue;
        }
      }

      EditorGUI.EndProperty();
    }
  }
}
#endif