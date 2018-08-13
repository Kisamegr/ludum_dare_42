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
  }

  public virtual void HitTarget(GameObject target) {
    //if simple type1 
    if (target.CompareTag(targetTag)) {
      target.SendMessage("GetDamage", damageAmount);
      PlayDestroySound();
      Destroy(gameObject);
    }

    if (target.CompareTag("Wall")) {
      UiManager.Instance().MakeExplosion(transform.position, 6, GetComponent<SpriteRenderer>().color, speedMultiplier: 0.5f);
      PlayDestroySound();
      Destroy(gameObject);
    }
  }

  protected void PlayDestroySound()
  {
    Transform audioChild = transform.Find("Audio");
    if (audioChild != null)
    {
      audioChild.parent = null;
      audioChild.GetComponent<AudioSource>().Play();
      Destroy(audioChild.gameObject, 2f);
    }
  }

  protected virtual void OnTriggerEnter2D(Collider2D collision) {
    HitTarget(collision.gameObject);
  }

}
