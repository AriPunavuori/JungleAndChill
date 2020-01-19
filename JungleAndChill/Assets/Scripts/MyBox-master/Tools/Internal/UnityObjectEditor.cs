#if UNITY_EDITOR
namespace MyBox.Internal {
  using UnityEditor;
  using UnityEngine;

  [CustomEditor(typeof(Object), true), CanEditMultipleObjects]
  public class UnityObjectEditor : Editor {
    private FoldoutAttributeHandler _foldout;
    private ButtonMethodHandler _buttonMethod;

    private void OnEnable() {
      try {
        _foldout = new FoldoutAttributeHandler(target, serializedObject);
        _buttonMethod = new ButtonMethodHandler(target);
      } catch (System.Exception) {
      }
    }

    private void OnDisable() {
      try {
        _foldout.OnDisable();
      } catch (System.Exception) {
      }
    }

    public override void OnInspectorGUI() {
      _foldout.Update();
      if (!_foldout.OverrideInspector) base.OnInspectorGUI();
      else _foldout.OnInspectorGUI();

      _buttonMethod.OnInspectorGUI();
    }
  }
}
#endif