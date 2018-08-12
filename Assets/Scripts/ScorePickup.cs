using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScorePickup : MonoBehaviour {

  public float lifetime = 10f;
  public int score = 10;
  public float flickerDuration = 3f;
  public float magnetRadius = 1f;
  public float magneticForce = 10;

  private float creationTime;
  private Rigidbody2D _rigidbody;
  private Transform playerTrans;
  private bool attached;
  private SpriteRenderer _spriteRenderer;


  private void Awake()
  {
    creationTime = Time.time;
    attached = false;
    _spriteRenderer = GetComponent<SpriteRenderer>();
  }

  private void Start()
  {
    _rigidbody = GetComponent<Rigidbody2D>();
    playerTrans = GameObject.Find("Player").transform;
    _rigidbody.AddTorque((Random.value - 0.5f) * 500);
  }

  private void Update()
  {
    if(Time.time - creationTime > lifetime - flickerDuration)
    {
      float t = (Time.time - (creationTime + lifetime - flickerDuration)) / flickerDuration;
      float r = Random.value;
      
      Color c = _spriteRenderer.color;
      c.a = r < t ? 0 : 1;
      _spriteRenderer.color = c;
    }

    if(Time.time - creationTime > lifetime){
      Disappear();
    }
  }

  private void FixedUpdate()
  {
    Vector2 diff = playerTrans.position - transform.position;
    if (diff.magnitude < magnetRadius || attached) {
      _rigidbody.velocity = diff.normalized * magneticForce * Time.smoothDeltaTime;
      attached = true;
     }
  }

  private void Disappear()
  {
    Destroy(gameObject);
  }

  void OnTriggerEnter2D(Collider2D collision)
  { 
    if (collision.gameObject.CompareTag("Player"))
    {
      TopDownGame.Instance().IncreaseScore(score);
      Disappear();
    }
  }
   


}
