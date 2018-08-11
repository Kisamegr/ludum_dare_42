using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BulletType { Simple };


public class Bullet : MonoBehaviour {

  public BulletType bulletType = BulletType.Simple;
  public int damageAmount;
  public float pushForce;
  public string targetTag;

  public void Shoot(float bulletSpeed) {
    float bulletAngle = Mathf.Deg2Rad * transform.rotation.eulerAngles.z;
    Vector2 bulletDir = new Vector2(Mathf.Cos(bulletAngle), Mathf.Sin(bulletAngle));
    GetComponent<Rigidbody2D>().velocity = bulletDir * bulletSpeed;
  }

  protected virtual void HitTarget(GameObject target) {
    target.SendMessage("GetDamage", damageAmount);
  }

  protected virtual void OnTriggerEnter2D(Collider2D collision) {
    //if simple type1

    Debug.Log(collision.name);
    if (collision.CompareTag(targetTag)) {
      HitTarget(collision.gameObject);
    }
    Debug.Log("DESTROEES");
    Destroy(gameObject);
  }

}
