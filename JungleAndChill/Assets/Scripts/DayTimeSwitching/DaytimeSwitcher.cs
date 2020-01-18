using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class DaytimeSwitcher : MonoBehaviour {
  public enum dayNite { Day, Night, DayToNight, NightToDay };
  public dayNite dn;
  public float Daylight;
  public float switchTime = 5;

  private Material mat;

  private void Start() {
    mat = GetComponent<Renderer>().material;

  }

  private void Update() {
    if (dn == dayNite.DayToNight) {
      Daylight -= (Time.deltaTime / switchTime);
      mat.SetFloat("Fraction", 1 - Daylight);
    }
    if (dn == dayNite.NightToDay) {
      Daylight += (Time.deltaTime / switchTime);
      mat.SetFloat("Fraction", Daylight);
    }
    if (Daylight <= 0 && dn != dayNite.Night) {
      dn = dayNite.Night;
      Daylight = 0;
      mat.SetInt("DayOuter", 1);
      mat.SetFloat("Fraction", 0);
      return;
    }
    if (Daylight >= 1 && dn != dayNite.Day) {
      dn = dayNite.Day;
      Daylight = 1;
      mat.SetInt("DayOuter", 0);
      mat.SetFloat("Fraction", 0);
      return;
    }
  }

  public void SetSource(Vector2 source) {
    mat.SetVector("Source", source);
  }

  public void SwitchDaytime() {
    if (dn == dayNite.Day)
      dn = dayNite.DayToNight;
    if (dn == dayNite.Night)
      dn = dayNite.NightToDay;
  }
}
