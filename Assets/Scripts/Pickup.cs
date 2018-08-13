using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Flags]
public enum PickupType
{
  Score = 0,
  IncreasePlayerSpeed = 1,
  IncreaseBulletSpeed = 2,
  IncreaseBulletPower = 3,
  SecondaryWeapon,
  SpecialSkill
}


public class Pickup: MonoBehaviour {


  public PickupType type = PickupType.Score;
  public float lifetime = 10f;
  public int score = 10;
  public float flickerDuration = 3f;
  public float magnetRadius = 1f;
  public float magneticForce = 10;
  public ScriptableObject scriptableObject;

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

    if(type == PickupType.Score)
      _rigidbody.AddTorque((UnityEngine.Random.value - 0.5f) * 300);
    
  }

  private void Update()
  {
    if(Time.time - creationTime > lifetime - flickerDuration)
    {
      float t = (Time.time - (creationTime + lifetime - flickerDuration)) / flickerDuration;
      float r = UnityEngine.Random.value;
      
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
    PlayPickupSound();
    Destroy(gameObject);
  }

  void OnTriggerEnter2D(Collider2D collision)
  { 
    if (collision.gameObject.CompareTag("Player"))
    {
      Player player = GameObject.Find("Player").GetComponent<Player>();

      GAME.Instance().IncreaseScore(score);
      switch (type)
      {
        case PickupType.Score:

          //GAME.Instance().IncreaseScore(score);
          break;
        case PickupType.IncreaseBulletSpeed:
          player.IncreaseBulletSpeed();
          break;
        case PickupType.IncreaseBulletPower:
          player.IncreaseBulletPower();
          break;
        case PickupType.IncreasePlayerSpeed:
          player.IncreasePlayerSpeed();
          break;
        case PickupType.SpecialSkill:
          player.SetSpecialWeapon((SpecialWeaponObject) scriptableObject);
          break;
        default:
          break;
      }
      Disappear();
    }
  }

  private void PlayPickupSound()
  {
    Transform audioChild = transform.Find("Audio");
    if (audioChild != null) {
      audioChild.parent = null;
      audioChild.GetComponent<AudioSource>().Play();
      Destroy(audioChild.gameObject, 2f);
    }
  }


}

[CreateAssetMenu(fileName = "PickupTable", menuName = "Pickup Table")]
public class PickupTable : ScriptableObject {
  public PickupInfo[] pickupInfos;

  public GameObject ChoosePickup() {
    float totalWeight = 0;
    foreach (PickupInfo info in pickupInfos)
      totalWeight += info.weight;

    float r = UnityEngine.Random.Range(0, totalWeight);

    totalWeight = 0;
    foreach (PickupInfo info in pickupInfos) {
      totalWeight += info.weight;

      if (r < totalWeight)
        return info.pickupPrefab;
    }

    return null;
  }
}

[Serializable]
public class PickupInfo {
  public GameObject pickupPrefab;
  public float weight = 1;
}
