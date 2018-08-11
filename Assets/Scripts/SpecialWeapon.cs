using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Special Weapon", menuName = "Special Weapon")]
public class SpecialWeaponObject : ScriptableObject {

  public enum WeaponType {
    Dash, Rocket, Homing
  }

  public string weaponName;
  public WeaponType type;
  public float cooldown;
  public int ammo;
  public GameObject bullet;
  public bool hasSecondActivation;
}
