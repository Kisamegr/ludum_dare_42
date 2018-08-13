using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BulletType { Simple };

[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour {

  public BulletType bulletType = BulletType.Simple;
  public float bulletSpeed;
  public float damageAmount;
  public float pushForce;
  public string targetTag;
  public Color explosionColor;

  protected Rigidbody2D body;

  protected virtual void Awake() {
    body = GetComponent<Rigidbody2D>();
  }

  public void Shoot() {
    Shoot(bulletSpeed);
  }

  public void Shoot(float givenSpeed) {
    float bulletAngle = Mathf.Deg2Rad * transform.rotation.eulerAngles.z;
    Vector2 bulletDir = new Vector2(Mathf.Cos(bulletAngle), Mathf.Sin(bulletAngle));
    body.velocity = bulletDir * givenSpeed;

    if (GetComponent<AudioSource>() != null)
    {
      GetComponent<AudioSource>().Play();
    }
  }

  public virtual void HitTarget(GameObject target) {
    //if simple type1 
    if (target.CompareTag(targetTag)) {
      target.SendMessage("GetDamage", damageAmount);
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
