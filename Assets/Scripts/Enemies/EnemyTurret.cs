using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTurret : Enemy {

  [Header ("Turret Properties")]
  public float shootInterval = 0.2f;
  public GameObject bulletPrefab;
  public Transform bulletSpawnOffset;

  protected override void Start() {
    base.Start();

    bulletSpawnOffset = transform.Find("BulletSpawnOffset");
  }

  protected override void Update() {
    base.Update();

    //Extrapolate maybe to predict future movement (+ noise)
    float angle = Vector2.SignedAngle(Vector2.right, lastMoveDir);
    transform.rotation = Quaternion.Euler(0, 0, angle);
    Shoot();
  }


  protected override void OnPlayerCollision() {
    // Do nothing...
  }

  public void Shoot() {
    if (Time.time - lastShootTime > shootInterval) {
      GameObject bulletGO = Instantiate(bulletPrefab, position: bulletSpawnOffset.position, rotation: transform.rotation);
      Bullet bullet = bulletGO.GetComponent<Bullet>();
      bullet.Shoot();

      lastShootTime = Time.time;
    }
  }

  public override EnemyType Type() {
    return EnemyType.Turret;
  }
}
