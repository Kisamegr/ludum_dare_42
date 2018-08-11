using System;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Level", menuName = "Level")]
public class LevelObject : ScriptableObject {

  public int id;
  public EnemySpawnEvent[] enemySpawnEvents;
}

[Serializable]
public struct EnemySpawnEvent {
  public EnemySpawn[] enemySpawns;
}

[Serializable]
public struct EnemySpawn {
  public GameObject enemy;
  public int count;
}
