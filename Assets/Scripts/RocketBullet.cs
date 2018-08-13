using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketBullet : Bullet {

  public float explosionRadius;
  public SpriteRenderer radiusIndicator;

  private int layerMask;

  protected void Start() {
    Vector3 scale = new Vector3(
      explosionRadius*2 / (radiusIndicator.size.x * transform.localScale.x),
      explosionRadius*2 / (radiusIndicator.size.y * transform.localScale.y),
      1);

    radiusIndicator.transform.localScale = scale;

    layerMask = Physics2D.GetLayerCollisionMask(gameObject.layer);
  }

  public override void HitTarget(GameObject target) {
    Collider2D[] collidersHit = Physics2D.OverlapCircleAll(transform.position, explosionRadius, layerMask);

    foreach (Collider2D collider in collidersHit) {
      if (collider.CompareTag(targetTag)) {
        collider.gameObject.SendMessage("GetDamage", damageAmount);
      }
    }
    UiManager.Instance().MakeExplosion(transform.position, 200, Color.yellow, speedMultiplier: 4f);
    Destroy(gameObject);
  }

}
