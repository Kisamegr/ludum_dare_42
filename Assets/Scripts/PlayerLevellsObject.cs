using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//[CreateAssetMenu(fileName = "New {layerLevel", menuName = "Player Levels")]
[Serializable]
public class PlayerLevelsObject {

  public float[] playerSpeedLevels;
  public float[] bulletSpeedLevels;
  public float[] bulletShootIntervalLevels;
  public int[] bulletCountLevels;

}
