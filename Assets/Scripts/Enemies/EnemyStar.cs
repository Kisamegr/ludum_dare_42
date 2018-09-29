using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStar : Enemy {
  private float rotateSpeed = 2f;
  private Vector3 pivotPoint;

  protected override void Start() {
    base.Start();

    _rigidbody.angularVelocity = 700f;

    if (Random.value < 0.5) {
      rotateSpeed *= -1;
      _rigidbody.angularVelocity *= -1f;

    }
    float distFromCenter = Random.value;
    pivotPoint = distFromCenter * transform.position;
    pivotPoint.z = transform.position.z;
  }

  protected override void Update() {
    base.Update();

    transform.RotateAround(pivotPoint, new Vector3(0, 0, 1), rotateSpeed);

  }

  protected override void OnCollisionEnter2D(Collision2D collision) {
    base.OnCollisionEnter2D(collision);

    if (collision.collider.CompareTag("Wall")) {
      rotateSpeed *= -1f;
      _rigidbody.angularVelocity *= -1;
    }
  }

  protected override void OnPlayerCollision() {
    player.GetDamage(meleeDamage);
    Die();
  }

  public override EnemyType Type() {
    return EnemyType.Star;
  }
}
