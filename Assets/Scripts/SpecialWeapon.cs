using System;
using UnityEngine;

//[CreateAssetMenu(fileName = "New Special Weapon", menuName = "Special Weapon")]
[Serializable]
public class SpecialWeaponObject {

  public enum WeaponType {
    Dash, Rocket, Homing
  }

  public string weaponName;
  public WeaponType type;
  public float cooldown;
  public int ammo;
  public GameObject bullet;
  public bool hasSecondActivation;
  public float count = 1;
}
