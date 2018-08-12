using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BulletType { Simple };


public class Bullet : MonoBehaviour {

  public BulletType bulletType = BulletType.Simple;
  public float bulletSpeed;
  public float damageAmount;
  public float pushForce;
  public string targetTag;
  public Color explosionColor;


  public void Shoot() {
    Shoot(bulletSpeed);
  }

  public void Shoot(float bulletSpeed) {
    float bulletAngle = Mathf.Deg2Rad * transform.rotation.eulerAngles.z;
    Vector2 bulletDir = new Vector2(Mathf.Cos(bulletAngle), Mathf.Sin(bulletAngle));
    GetComponent<Rigidbody2D>().velocity = bulletDir * bulletSpeed;
  }

  public virtual void HitTarget(GameObject target) {
    //if simple type1 
    if (target.CompareTag(targetTag)) {
      target.SendMessage("GetDamage", damageAmount);


      //TODO - penetration powerup/skill
      if (true)
        Destroy(gameObject);
    }

    if (target.CompareTag("Wall")) {
      UiManager.Instance().MakeExplosion(transform.position, 6, GetComponent<SpriteRenderer>().color, speedMultiplier: 0.5f);
      Destroy(gameObject);
    }
  }

  protected virtual void OnTriggerEnter2D(Collider2D collision) {
    HitTarget(collision.gameObject);
  }

}
