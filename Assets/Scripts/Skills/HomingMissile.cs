using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingMissile : Bullet {

  public float targetChooseDelay;
  public float searchRadius;
  public float rotationSpeed = 100;


  private GameObject target = null;

  bool searching = false;

  private void Update() {
    if(target != null) {
      Vector2 dir = target.transform.position - transform.position;

      float rotateAmount = Vector3.Cross(dir, transform.right).z;
      body.angularVelocity = -rotateAmount * rotationSpeed;
      body.velocity = transform.right * bulletSpeed;
    }
    else if(!searching) {
      InvokeRepeating("FindTarget", targetChooseDelay, 0.2f);
      searching = true;
    }
    
  }

  public void ShootOffset(float angleOffset) {
    float bulletAngle = Mathf.Deg2Rad * (transform.rotation.eulerAngles.z + angleOffset);
    Vector2 bulletDir = new Vector2(Mathf.Cos(bulletAngle), Mathf.Sin(bulletAngle));
    body.velocity = bulletDir * bulletSpeed;
  }

  private void FindTarget() {
    Collider2D[] collidersHit = Physics2D.OverlapCircleAll(this.transform.position, searchRadius, 1 << LayerMask.NameToLayer("Enemy"));

    if(collidersHit != null && collidersHit.Length > 0) {
      target = collidersHit[Random.Range(0, collidersHit.Length)].gameObject;
      CancelInvoke("FindTarget");
      searching = false;
    }

  }


}
