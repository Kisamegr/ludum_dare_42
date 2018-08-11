using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BulletType { Simple };


public class Bullet : MonoBehaviour {

    public BulletType bulletType = BulletType.Simple;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	    
	}

    public void Shoot(float bulletSpeed)
    {
        float bulletAngle = Mathf.Deg2Rad * transform.rotation.eulerAngles.z;
        Vector2 bulletDir = new Vector2(Mathf.Cos(bulletAngle), Mathf.Sin(bulletAngle));
        GetComponent<Rigidbody2D>().velocity = bulletDir * bulletSpeed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Enemy")
        {
            collision.GetComponent<Enemy>().GetDamage(this);
        }
    }

}
