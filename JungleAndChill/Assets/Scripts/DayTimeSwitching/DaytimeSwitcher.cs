using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEditor;
using UnityEngine.Rendering;

[RequireComponent(typeof(Renderer))]
public class DaytimeSwitcher : MonoBehaviour {

  [Range(0, 1)]
  public float dayValue; // 0: night, 1: day
  public float switchTime = 5;

  public new Light light;
  public float dayIntensity = 3;
  public float nightIntensity = 1;

  public Volume ppvol;
  public AnimationCurve bloomCurve;
  public float dayBloomIntensity = 0;
  public float nightBloomIntensity = 3.5f;

  public UnityEvent onDay;
  public UnityEvent onNight;

  private Material mat;
  private UnityEngine.Rendering.Universal.Bloom bloomComp;
  private bool update;
  private float dir;
  private float prevDayValue;

  private void OnValidate() {
    if (Application.isPlaying)
      return;
    if (PrefabUtility.GetCorrespondingObjectFromSource(gameObject) == null && PrefabUtility.GetPrefabInstanceHandle(gameObject) != null)
      return;
    try {
      mat = GetComponent<Renderer>().sharedMaterial;
      if (prevDayValue < dayValue) {
        dir = 1;
        if (mat != null) mat.SetInt("ExpandsToDay", 1);
      }
      if (prevDayValue > dayValue) {
        dir = -1;
        if (mat != null) mat.SetInt("ExpandsToDay", 0);
      }
      bloomComp = FindBloomComponent();

      prevDayValue = dayValue;
      dayValue += dir * (Time.deltaTime / switchTime);
      update = true;
      Update();
    } catch (System.Exception) {
      return;
    }
  }


  private void Start() {
    mat = GetComponent<Renderer>().material;
    bloomComp = FindBloomComponent();
  }

  private UnityEngine.Rendering.Universal.Bloom FindBloomComponent() {
    var bloom = FindObjectOfType<UnityEngine.Rendering.Universal.Bloom>();
    foreach (var comp in ppvol.profile.components) {
      bloomComp = comp as UnityEngine.Rendering.Universal.Bloom;
      if (bloomComp != null)
        break;
    }
    return bloomComp;
  }

  private void Update() {
    if (!update) return;

    dayValue = Mathf.Clamp(dayValue + dir * (Time.deltaTime / switchTime), 0, 1);

    light.intensity = Mathf.Lerp(nightIntensity, dayIntensity, dayValue);
    bloomComp.intensity.value = Mathf.Lerp(nightBloomIntensity, dayBloomIntensity, bloomCurve.Evaluate(dayValue));

    if (dayValue % 1 == 0) {
      update = false;
      var day = dayValue == 1;
      mat.SetFloat("Fraction", 0);
      mat.SetInt("ExpandsToDay", dayValue > 0.5 ? 0 : 1);
    } else {
      mat.SetFloat("Fraction", (dir > 0 ? dayValue : 1 - dayValue));
    }
  }

  public void SetSource(Vector3 source) {
    mat.SetVector("Source", source.normalized);
  }

  public void SwitchDaytime() {
    if (dayValue == 0) {
      update = true;
      mat.SetInt("ExpandsToDay", 1);
      dir = 1;
      onNight.Invoke();
    } else if (dayValue == 1) {
      update = true;
      mat.SetInt("ExpandsToDay", 0);
      dir = -1;
      onDay.Invoke();
    }
  }
}
