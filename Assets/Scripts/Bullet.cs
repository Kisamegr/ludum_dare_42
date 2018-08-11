using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BulletType { Simple };


public class Bullet : MonoBehaviour {

  public BulletType bulletType = BulletType.Simple;
  public float damageAmount;
  public float pushForce;
  public string TargetTag { get; set; }

  public void Shoot(float bulletSpeed) {
    float bulletAngle = Mathf.Deg2Rad * transform.rotation.eulerAngles.z;
    Vector2 bulletDir = new Vector2(Mathf.Cos(bulletAngle), Mathf.Sin(bulletAngle));
    GetComponent<Rigidbody2D>().velocity = bulletDir * bulletSpeed;
  }

  private void OnTriggerEnter2D(Collider2D collision) {
    //if simple type1

    Debug.Log(TargetTag);
    if (collision.CompareTag(TargetTag)) {
      collision.gameObject.SendMessage("GetDamage", damageAmount);
      Destroy(gameObject);
    }
    else if (collision.CompareTag("Wall"))
    {
      Destroy(gameObject);
    }
  }

}
