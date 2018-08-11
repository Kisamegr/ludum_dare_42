using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScorePickup : MonoBehaviour {

  public float lifetime = 10f;

  public int score = 10;

  public float flickerDuration = 3f;

  private float creationTime;

  private void Awake()
  {
    creationTime = Time.time;
  }

  private void Update()
  {
    if(Time.time - creationTime > lifetime - flickerDuration)
    {
      //TODO - start flickering
    }

    if(Time.time - creationTime > lifetime)
    {
      Disappear();
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
