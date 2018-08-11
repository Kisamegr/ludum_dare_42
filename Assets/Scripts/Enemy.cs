using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyType { Type1 };


public class Enemy : MonoBehaviour {

  public EnemyType enemyType = EnemyType.Type1;
  public float moveSpeed = 5f;

  private TopDownCharacterController player;
  private Rigidbody2D _rigidbody;

  // Use this for initialization
  void Start() {
    player = GameObject.Find("Player").GetComponent<TopDownCharacterController>();
    _rigidbody = GetComponent<Rigidbody2D>();
  }

  // Update is called once per frame
  void Update() {
    Vector2 moveDir = (player.transform.position - transform.position).normalized;
    _rigidbody.velocity = moveDir * moveSpeed;
  }

  public void OnCollisionEnter2D(Collision2D collision) {
    if (collision.collider.CompareTag(player.tag)) {
      player.GetDamage(this);
      Die();
    }
  }

  public void GetDamage(Bullet bullet) {
    Die();
  }

  public void Die() {
    Destroy(gameObject);
  }


}
