using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New {layerLevel", menuName = "Player Levels")]
public class PlayerLevelsObject : ScriptableObject {

  public float[] playerSpeedLevels;
  public float[] bulletSpeedLevels;
  public float[] bulletShootIntervalLevels;
  public float[] bulletSizeLevels;
  public float[] bulletDamageLevels;

}
