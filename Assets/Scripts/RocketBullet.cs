using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketBullet : Bullet {

  public float explosionRadius;

  protected override void OnTriggerEnter2D(Collider2D collision) {
    Collider2D[] collidersHit = Physics2D.OverlapCircleAll(transform.position, explosionRadius);

    foreach(Collider2D collider in collidersHit) {
      if (collider.CompareTag(targetTag)) {
        HitTarget(collider.gameObject);
      }
    }

    Destroy(gameObject);
  }

  private void OnDrawGizmos() {
    Gizmos.DrawSphere(transform.position, explosionRadius);
  }
}
