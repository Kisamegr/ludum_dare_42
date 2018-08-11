using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(Rigidbody2D))]
public class TopDownCharacterController : MonoBehaviour
{

	public float maxSpeed;

    public GameObject bulletPrefab;

    public float shootInterval = 0.1f;

    public float bulletSpeed = 10f;

    private float lastShootTime;

    private Transform bulletSpawnOffset;


	Rigidbody2D body;



	// Use this for initialization
	void Start ()
	{
		body = GetComponent<Rigidbody2D> ();
        lastShootTime = Time.time;
        bulletSpawnOffset = transform.Find("BulletSpawnOffset");
	}
	
	// Update is called once per frame
	void Update ()
	{
		Move ();

        Rotate();

        Shoot();

	}

	void Move ()
	{
		Vector2 velocity;
		velocity.x = Input.GetAxis ("Horizontal") * maxSpeed;
		velocity.y = Input.GetAxis ("Vertical") * maxSpeed;

		body.velocity = velocity;
	
	}

	void Rotate ()
	{
        Vector2 dir = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position; 
        
        transform.rotation = Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.right, dir));
    }

    void Shoot()
    {
        if (Input.GetAxis("Fire1") > 0 && Time.time - lastShootTime > shootInterval)
        {
            GameObject bullet = Instantiate(bulletPrefab, position: bulletSpawnOffset.position, rotation: transform.rotation);

            bullet.GetComponent<Bullet>().Shoot(bulletSpeed);
            
            lastShootTime = Time.time;
        }
    }



    public void GetDamage(Enemy enemy)
    {
        //Decrease screen size
    }


}
