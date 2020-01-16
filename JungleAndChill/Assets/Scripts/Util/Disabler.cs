using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Disabler {

  [Tooltip("Drop gameObject to add it to the list of disabled objects")]
  public GameObject addGameObject;
  [Tooltip("Drop behaviour to add it to the list of disabled objects")]
  public Behaviour addBehaviour;
  [Tooltip("Drop renderer to add it to the list of disabled objects")]
  public Renderer addRenderer;

  [Tooltip("Only Renderer, Behaviour and GameObject types are supported!")]
  public List<Object> objects;


  void OnValidate() {
    // !!! NOT CALLED
    if (addGameObject != null) objects.Add(addGameObject);
    addGameObject = null;

    if (addBehaviour != null) objects.Add(addBehaviour);
    addBehaviour = null;

    if (addRenderer != null) objects.Add(addRenderer);
    addRenderer = null;

    foreach (var obj in objects) {
      if (!(obj is Renderer) && !(obj is GameObject) && !(obj is Behaviour)) {
        objects.Remove(obj);
      }
    }
  }


  public void DisableComponents() {
    foreach (var obj in objects)
      Disable(obj);
  }

  public void EnableComponents() {
    foreach (var obj in objects)
      Enable(obj);
  }


  void Disable(Object obj) {
    var r = obj as Renderer;
    if (r != null) {
      r.enabled = false;
    } else {
      var b = obj as Behaviour;
      if (b != null) {
        b.enabled = false;
      } else {
        var g = obj as GameObject;
        if (g != null) {
          g.SetActive(false);
        }
      }
    }
  }

  void Enable(Object obj) {
    var r = obj as Renderer;
    if (r != null) {
      r.enabled = true;
    } else {
      var b = obj as Behaviour;
      if (b != null) {
        b.enabled = true;
      } else {
        var g = obj as GameObject;
        if (g != null) {
          g.SetActive(true);
        }
      }
    }
  }
}
