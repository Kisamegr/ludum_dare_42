using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Type1: Deal damage and Die on collision
/// Type2: Push on collision
/// Type3: Turret
/// </summary>
public enum EnemyType { Simple, Wall, Turret, Star, Type5, Type6 };

public abstract class Enemy : MonoBehaviour {


  [Header("Enemy Properties")]
  public float moveSpeed = 5f;
  public float meleeDamage = 1;
  public float hp = 1;
  public float wallPushAmount = 5f;
  public int pickupsDropCount = 1;
  public Transform scorePickupPrefab;
 
  [Header("Misc.")]
  public float colorHitDamping = 0.2f;
  public float colorHitWhite = 0.7f;

  [HideInInspector]
  public GameObject pickupDropPrefab;

  protected Player player;
  protected Rigidbody2D _rigidbody;
  protected float lastShootTime;
  protected float currentHp;
  protected SpriteRenderer spriteRenderer;
  protected Color originalColor;

  protected Vector2 lastMoveDir;

  // Use this for initialization
  protected virtual void Start() {
    player = GameObject.Find("Player").GetComponent<Player>();
    spriteRenderer = GetComponent<SpriteRenderer>();
    originalColor = spriteRenderer.color;
    _rigidbody = GetComponent<Rigidbody2D>();
    lastShootTime = Time.time;
    currentHp = hp;
  }

  // Update is called once per frame
  protected virtual void Update() {

    spriteRenderer.color = Color.Lerp(spriteRenderer.color, originalColor, Time.deltaTime / colorHitDamping);
    lastMoveDir = (player.transform.position - transform.position).normalized;
      //case EnemyType.Type5:
      //  _rigidbody.velocity = lastMoveDir * moveSpeed;
  }

  protected abstract void OnPlayerCollision();

  protected virtual void OnCollisionEnter2D(Collision2D collision)
  {
    if (collision.collider.CompareTag(player.tag))
    {
      player.GetPushed(meleeDamage);

      if(player.HasStatus(Player.Status.Invunerable)) {
        GetDamage(player.invunerableDamage);
      }
      else {
        OnPlayerCollision();
          //case EnemyType.Type5:
          //  //TODO use pushAmount variable?
          //  player.GetPushed(meleeDamage); 
      }
    }
  }

  private void OnCollisionStay2D(Collision2D collision)
  {
    if (collision.collider.CompareTag("Wall"))
    {
      Rect currentMapSize = GAME.Instance().CurrentMapSize();
      transform.position = new Vector3(currentMapSize.center.x, currentMapSize.center.y, 0);
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
    GAME.Instance().ChangeWallBorders(true, wallPushAmount);

    float pickupSize = scorePickupPrefab.GetComponent<SpriteRenderer>().bounds.size.x;
    for (int i = 0; i < pickupsDropCount; i++)
    {
      float x = Random.value * (pickupsDropCount - 1) * pickupSize;
      float y = Random.value * (pickupsDropCount - 1) * pickupSize;
      Vector3 offset = new Vector3(x, y, 0);
      Instantiate(scorePickupPrefab, transform.position + offset, Quaternion.identity);
    }
    UiManager.Instance().MakeExplosion(transform.position, 30, spriteRenderer.color);

    if (pickupDropPrefab != null) {
      Pickup pickup = Instantiate(pickupDropPrefab, transform.position, Quaternion.identity).GetComponent<Pickup>();
      pickup.player = player;
    }

    Destroy(gameObject);
  }

  public abstract EnemyType Type();
}
