using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySimple : Enemy {

  protected override void Update() {
    base.Update();

    _rigidbody.velocity = lastMoveDir * moveSpeed;
  }

  protected override void OnPlayerCollision() {
    player.GetDamage(meleeDamage);
    Die();
  }

  public override EnemyType Type() {
    return EnemyType.Simple;
  }
}
