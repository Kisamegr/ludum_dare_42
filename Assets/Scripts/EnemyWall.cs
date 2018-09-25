using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWall : Enemy {

  protected override void Update() {
    base.Update();

    Rect currentMapSize = GAME.Instance().CurrentMapSize();


    if (!currentMapSize.Contains(GetComponent<SpriteRenderer>().bounds.min) || !currentMapSize.Contains(GetComponent<SpriteRenderer>().bounds.max)) {
      Vector2 tp = transform.position;
      lastMoveDir = (currentMapSize.center - tp).normalized;
      _rigidbody.velocity = lastMoveDir * moveSpeed;
    }
    else {
      _rigidbody.velocity = Vector2.zero;
    }
  }

  protected override void OnPlayerCollision() {
    // Do nothing...
  }

  public override EnemyType Type() {
    return EnemyType.Wall;
  }
}
