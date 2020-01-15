using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DaytimeSwitcher : MonoBehaviour {
    public enum dayNite { Day, Night, DayToNight, NightToDay };
    public dayNite dn;
    public float Daylight;
    public float switchTime = 5;

    private void Update() {
        if(dn == dayNite.DayToNight) {
            Daylight -= (Time.deltaTime / switchTime);
        }
        if(dn == dayNite.NightToDay) {
            Daylight += (Time.deltaTime / switchTime);
        }
        if(Daylight <= 0) {
            dn = dayNite.Night;
            Daylight = 0;
        }
        if(Daylight >= 1) {
            dn = dayNite.Day;
            Daylight = 1;
        }
    }

    public void SwitchDaytime() {
        if(dn == dayNite.Day)
            dn = dayNite.DayToNight;
        if (dn == dayNite.Night)
            dn = dayNite.NightToDay;
    }
}
