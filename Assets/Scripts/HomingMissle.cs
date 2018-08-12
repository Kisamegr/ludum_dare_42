using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingMissle : Bullet {

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

  private void FindTarget() {
    Collider2D colliderHit = Physics2D.OverlapCircle(this.transform.position, searchRadius, 1 << LayerMask.NameToLayer("Enemy"));

    if(colliderHit != null) {
      target = colliderHit.gameObject;
      Debug.Log("FOUND  " + colliderHit.name);
      CancelInvoke("FindTarget");
      searching = false;
    }

  }


}
