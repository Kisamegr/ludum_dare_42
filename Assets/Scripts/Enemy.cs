using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Type1: Deal damage and Die on collision
/// Type2: Push on collision
/// Type3: Turret
/// </summary>
public enum EnemyType { Type1, Type2, Type3, Type4, Type5, Type6 };


public class Enemy : MonoBehaviour {

  public EnemyType enemyType = EnemyType.Type1;
  public float moveSpeed = 5f;
  public int meleeDamage = 1;
  public float hp = 1;
  //Only if type 3
  public float shootInterval = 0.2f;
  public GameObject bulletPrefab;
  public Transform bulletSpawnOffset;
  public Transform scorePickupPrefab;
  public int pickupsDropCount = 1;
  public GameObject pickupDropPrefab;
  public float colorHitDamping = 0.2f;
  public float colorHitWhite = 0.7f;

  private Player player;
  private Rigidbody2D _rigidbody;
  private float lastShootTime;
  private float currentHp;
  private SpriteRenderer spriteRenderer;
  private Color originalColor;

  #region Type4 vars
  private float rotateSpeed = 2f;
  private Vector3 pivotPoint;
  #endregion


  // Use this for initialization
  void Start() {
    player = GameObject.Find("Player").GetComponent<Player>();
    spriteRenderer = GetComponent<SpriteRenderer>();
    originalColor = spriteRenderer.color;
    _rigidbody = GetComponent<Rigidbody2D>();
    lastShootTime = Time.time;
    bulletSpawnOffset = transform.Find("BulletSpawnOffset");
    currentHp = hp;

    if(enemyType == EnemyType.Type4)
    {
      _rigidbody.angularVelocity = 700f;

      if (Random.value < 0.5)
      {
        rotateSpeed *= -1;
         _rigidbody.angularVelocity *= -1f;
        
      }
      float distFromCenter = Random.value;
      pivotPoint = distFromCenter * transform.position;
      pivotPoint.z = transform.position.z;
    }
  }

  // Update is called once per frame
  void Update() {

    spriteRenderer.color = Color.Lerp(spriteRenderer.color, originalColor, Time.deltaTime / colorHitDamping);
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
        float angle = Vector2.SignedAngle(Vector2.right, moveDir);
        transform.rotation = Quaternion.Euler(0, 0, angle);
        Shoot();
        break;
      case EnemyType.Type4: 
        transform.RotateAround(pivotPoint, new Vector3(0, 0, 1), rotateSpeed);
        break;
    }
    
  }

  public void OnCollisionEnter2D(Collision2D collision)
  {
    if (collision.collider.CompareTag(player.tag))
    {
          player.GetPushed(meleeDamage);

      if(player.HasStatus(Player.Status.Invunerable)) {
        GetDamage(player.invunerableDamage);
      }
      else {
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

    if (collision.collider.CompareTag("Wall"))
    {
      rotateSpeed *= -1f;
      _rigidbody.angularVelocity *= -1;
    }
  }


    public void Shoot()
  {
    if (Time.time - lastShootTime > shootInterval)
    {
      GameObject bulletGO = Instantiate(bulletPrefab, position: bulletSpawnOffset.position, rotation: transform.rotation);
      Bullet bullet = bulletGO.GetComponent<Bullet>();
      bullet.Shoot();

      lastShootTime = Time.time;
    }
  }

  public void GetDamage(float damageAmount) {
    currentHp -= damageAmount;
    if (currentHp <= 0)
      Die();
    else
      spriteRenderer.color = Color.Lerp(spriteRenderer.color, Color.white, colorHitWhite);
    
  }

  public void Die() {
    GAME.Instance().EnemyDied();
    GAME.Instance().ChangeWallBorders(true, 5);

    float pickupSize = scorePickupPrefab.GetComponent<SpriteRenderer>().bounds.size.x;
    for (int i = 0; i < pickupsDropCount; i++)
    {
      float x = Random.value * (pickupsDropCount - 1) * pickupSize;
      float y = Random.value * (pickupsDropCount - 1) * pickupSize;
      Vector3 offset = new Vector3(x, y, 0);
      Instantiate(scorePickupPrefab, transform.position + offset, Quaternion.identity);
    }
    UiManager.Instance().MakeExplosion(transform.position, 30, spriteRenderer.color);

    if (pickupDropPrefab != null)
      Instantiate(pickupDropPrefab, transform.position, Quaternion.identity);

    Destroy(gameObject);
  }



}
