using System;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Level", menuName = "Level")]
public class LevelObject : ScriptableObject, ICloneable {

  public int id;
  public EnemySpawnEvent[] enemySpawnEvents;

  public object Clone()
  {
    LevelObject newLevel = new LevelObject();
    newLevel.enemySpawnEvents = new EnemySpawnEvent[this.enemySpawnEvents.Length];

    for (int i = 0; i < enemySpawnEvents.Length; i++)
    {
      var oldSpawnEvent = this.enemySpawnEvents[i];
      EnemySpawnEvent newSpawnEvent = new EnemySpawnEvent();
      newSpawnEvent.enemySpawns = new EnemySpawn[oldSpawnEvent.enemySpawns.Length];

      for (int j = 0; j < oldSpawnEvent.enemySpawns.Length; j++)
      {
        newSpawnEvent.enemySpawns[j].enemy = oldSpawnEvent.enemySpawns[j].enemy;
        newSpawnEvent.enemySpawns[j].count = oldSpawnEvent.enemySpawns[j].count;
      }

      newLevel.enemySpawnEvents[i] = newSpawnEvent;
    }
    return newLevel;
    //return this.MemberwiseClone();
  }
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
