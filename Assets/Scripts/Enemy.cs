using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Type1: Deal damage and Die on collision
/// Type2: Push on collision
/// Type3: Turret
/// </summary>
public enum EnemyType { Type1, Type2, Type3 };


public class Enemy : MonoBehaviour {

  public EnemyType enemyType = EnemyType.Type1;
  public float moveSpeed = 5f;
  public int meleeDamage = 1;
  public int hp = 1;
  //Only if type 3
  public float shootInterval = 0.2f;
  public float bulletSpeed = 10f;
  public GameObject bulletPrefab;
  public Transform bulletSpawnOffset;
  public Transform scorePickupPrefab;

  private TopDownCharacterController player;
  private Rigidbody2D _rigidbody;
  private float lastShootTime;
  private int currentHp;

  // Use this for initialization
  void Start() {
    player = GameObject.Find("Player").GetComponent<TopDownCharacterController>();
    _rigidbody = GetComponent<Rigidbody2D>();
    lastShootTime = Time.time;
    bulletSpawnOffset = transform.Find("BulletSpawnOffset");
    currentHp = hp;
  }

  // Update is called once per frame
  void Update() {

    Vector2 moveDir = (player.transform.position - transform.position).normalized;

    switch (enemyType)
    {
      case EnemyType.Type1:
        _rigidbody.velocity = moveDir * moveSpeed;
        break;
      case EnemyType.Type2:
        _rigidbody.velocity = moveDir * moveSpeed; 
        break;
      case EnemyType.Type3:
        //Extrapolate maybe to predict future movement (+ noise)
        transform.rotation = Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.right, moveDir));
        Shoot();
        break;
    }
    
  }

  public void OnCollisionEnter2D(Collision2D collision) {
    if (collision.collider.CompareTag(player.tag)) {
      switch (enemyType)
      {
        case EnemyType.Type1:
          player.GetDamage(meleeDamage);
          Die();
          break;
        case EnemyType.Type2:
          //TODO use pushAmount variable?
          player.GetPushed(meleeDamage); 
          break;
        default:
          break;
      }
    }
  }


  public void Shoot()
  {
    if (Time.time - lastShootTime > shootInterval)
    {
      GameObject bulletGO = Instantiate(bulletPrefab, position: bulletSpawnOffset.position, rotation: transform.rotation);
      Bullet bullet = bulletGO.GetComponent<Bullet>();
      bullet.Shoot(bulletSpeed);

      lastShootTime = Time.time;
    }
  }

  public void GetDamage(int damageAmount) {
    currentHp -= damageAmount;
    if (currentHp <= 0)
      Die();
  }

  public void Die() {
    TopDownGame.Instance().EnemyDied();
    TopDownGame.Instance().ChangeWallBorders(true, 5);
    Destroy(gameObject);
    Instantiate(scorePickupPrefab, transform.position, Quaternion.identity);

  }



}
