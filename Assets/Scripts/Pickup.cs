using System;
using UnityEngine;

[Flags]
public enum PickupType {
  Score,
  PlayerSpeed,
  BulletSpeed,
  BulletPower,
  SecondaryWeapon,
  SpecialSkill,
  Mana
}

public abstract class Pickup : MonoBehaviour {

  [Header("Pickup Properties")]
  public float lifetime = 10f;
  public int score = 10;
  public float flickerDuration = 3f;
  public float magnetRadius = 1f;
  public float magneticForce = 10;
  public bool requiresButtonPress = false;

  [HideInInspector]
  public Player player;

  protected float creationTime;
  protected Rigidbody2D _rigidbody;
  protected bool attached;
  protected SpriteRenderer _spriteRenderer;

  private void Awake() {
    creationTime = Time.time;
    attached = false;
    _spriteRenderer = GetComponent<SpriteRenderer>();
  }

  protected virtual void Start() {
    _rigidbody = GetComponent<Rigidbody2D>();
  }

  protected abstract void Action(Player player);

  protected abstract PickupType Type();

  protected virtual void Update() {
    if (Time.time - creationTime > lifetime - flickerDuration) {
      float t = (Time.time - (creationTime + lifetime - flickerDuration)) / flickerDuration;
      float r = UnityEngine.Random.value;

      Color c = _spriteRenderer.color;
      c.a = r < t ? 0 : 1;
      _spriteRenderer.color = c;
    }

    if (Time.time - creationTime > lifetime) {
      Disappear();
    }
  }

  private void FixedUpdate() {
    Vector2 diff = player.transform.position - transform.position;
    if (!requiresButtonPress && diff.magnitude < magnetRadius || attached) {
      _rigidbody.velocity = diff.normalized * magneticForce * Time.smoothDeltaTime;
      attached = true;
    }
  }

  private void Disappear() {
    PlayPickupSound();
    Destroy(gameObject);
  }

  void OnTriggerEnter2D(Collider2D collision) {
    if (collision.gameObject.CompareTag("Player")) {
      GAME.Instance().IncreaseScore(score);
      Action(player);
      Disappear();
    }
  }

  private void PlayPickupSound() {
    Transform audioChild = transform.Find("Audio");
    if (audioChild != null) {
      audioChild.parent = null;
      audioChild.GetComponent<AudioSource>().Play();
      Destroy(audioChild.gameObject, 2f);
    }
  }


}

//[CreateAssetMenu(fileName = "PickupTable", menuName = "Pickup Table")]
[Serializable]
public class PickupTable {
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
