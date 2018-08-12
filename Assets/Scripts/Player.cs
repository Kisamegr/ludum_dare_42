using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour {
   
  public float speedDamping = 0.12f;
  public GameObject primaryBulletPrefab; 
  public Status currentStatus;
  public SpecialWeaponObject specialWeapon;
  public PlayerLevelsObject playerLevels;

  private float lastShootTime = 0;
  private float lastSpecialShootTime = 0;
  private Transform bulletSpawnOffset;


  private int currentSpeedLevel = 0;
  private float maxSpeed;

  private int currentBulletSpeedLevel = 0;
  private float bulletSpeed;
  private float shootInterval;

  private int currentBulletPowerLevel = 0;
  private float bulletDamage;
  private float bulletSize;

  Rigidbody2D body;

  private float rootDuration = 0;
  private float rootStartTime = 0;

  private float invunerableDuration = 0;
  private float invunerableStartTime = 0;
  private GAME _GAME;

  private int currentAmmo = 100;

  [Flags]
  public enum Status {
    None = 0,
    Rooted = 1,
    Invunerable = 2
  }
  // Use this for initialization
  void Start() {
    body = GetComponent<Rigidbody2D>();
    bulletSpawnOffset = transform.Find("BulletSpawnOffset");
    _GAME = GAME.Instance();

    maxSpeed = playerLevels.playerSpeedLevels[0];
    bulletDamage = playerLevels.bulletDamageLevels[0];
    bulletSize = playerLevels.bulletSizeLevels[0];
    bulletSpeed = playerLevels.bulletSpeedLevels[0];
    shootInterval = playerLevels.bulletShootIntervalLevels[0];
  }

// Update is called once per frame
void Update() {
    if (!HasStatus(Status.Rooted))
      Move();
    Rotate();
    PrimaryShoot();
    SpecialShoot();

    CheckExpiredStatus();
  }

  void Move() {
    Vector2 velocity;
    velocity.x = Input.GetAxis("Horizontal") * maxSpeed;
    velocity.y = Input.GetAxis("Vertical") * maxSpeed;

    body.velocity = Vector2.Lerp(body.velocity, velocity, Time.deltaTime / speedDamping);
  }

  void Rotate() {
    Vector2 dir = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
    transform.rotation = Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.right, dir));
  }

  void PrimaryShoot() {
    if (Input.GetAxis("Fire1") > 0 && Time.time - lastShootTime > shootInterval) {
      Bullet bullet = CreateBullet(primaryBulletPrefab);
      bullet.damageAmount = bulletDamage;
      bullet.transform.localScale = new Vector3(bulletSize, bulletSize, 1);
      
      bullet.Shoot(playerLevels.bulletSpeedLevels[currentBulletSpeedLevel]);

      lastShootTime = Time.time;
    }

  }

  private Bullet CreateBullet(GameObject bulletPrefab) {
    return Instantiate(bulletPrefab, position: bulletSpawnOffset.position, rotation: transform.rotation).GetComponent<Bullet>();
  }

  public void IncreaseBulletPower()
  {
    currentBulletPowerLevel += 1;
    currentBulletPowerLevel = Mathf.Min(currentBulletPowerLevel, playerLevels.bulletDamageLevels.Length);
    bulletDamage = playerLevels.bulletDamageLevels[currentBulletPowerLevel];
    bulletSize = playerLevels.bulletSizeLevels[currentBulletPowerLevel]; 
  }

  public void IncreaseBulletSpeed()
  {
    currentBulletSpeedLevel += 1;  
    currentBulletSpeedLevel = Mathf.Min(currentBulletSpeedLevel, playerLevels.bulletSpeedLevels.Length);
    bulletSpeed = playerLevels.bulletSpeedLevels[currentBulletSpeedLevel];
    shootInterval = playerLevels.bulletShootIntervalLevels[currentBulletSpeedLevel];
  }

  public void IncreasePlayerSpeed()
  {
    currentSpeedLevel += 1;  
    currentSpeedLevel = Mathf.Min(currentSpeedLevel, playerLevels.playerSpeedLevels.Length);
    maxSpeed = playerLevels.playerSpeedLevels[currentSpeedLevel]; 
  }


  void SpecialShoot() {
    if (specialWeapon != null) {

      if (Input.GetAxis("Fire2") > 0 && lastSpecialShootTime + specialWeapon.cooldown < Time.time) {
        switch (specialWeapon.type) {
          case SpecialWeaponObject.WeaponType.Dash:
            SetStatus(Status.Rooted | Status.Invunerable, 1);
            body.AddForce(transform.right * 75, ForceMode2D.Impulse);
            break;
          case SpecialWeaponObject.WeaponType.Rocket:
            RocketBullet rocket = (RocketBullet) CreateBullet(specialWeapon.bullet);
            rocket.Shoot(10/2); //TODO have a rocket speed variable
            break;
          case SpecialWeaponObject.WeaponType.Homing:
            break;
        }

        //specialWeapon.ammo--;
        currentAmmo--;
        lastSpecialShootTime = Time.time;

        if (currentAmmo == 0)
          specialWeapon = null;
      }

    }
  }


  public void GetDamage(int damageAmount) {
    if (!HasStatus(Status.Invunerable)) {
      _GAME.ChangeWallBorders(false, -damageAmount);

      var vcam = GameObject.Find("Follow Cam").GetComponent<Cinemachine.CinemachineVirtualCamera>();
      var noise = vcam.GetCinemachineComponent<Cinemachine.CinemachineBasicMultiChannelPerlin>();

      noise.m_AmplitudeGain = 2;
      noise.m_FrequencyGain = 4;

      Invoke("StopShaking", 0.4f);
    }
  }

  private void StopShaking()
  {
    var vcam = GameObject.Find("Follow Cam").GetComponent<Cinemachine.CinemachineVirtualCamera>();
    var noise = vcam.GetCinemachineComponent<Cinemachine.CinemachineBasicMultiChannelPerlin>();

    noise.m_AmplitudeGain = 0;
    noise.m_FrequencyGain = 0;
  }



  public void GetPushed(float force) {

  }

  public void SetStatus(Status status, float timeAmount = 0) {
    switch (status) {
      case Status.None:
        ResetStatus();
        break;
      case Status.Rooted:
        rootDuration = timeAmount;
        rootStartTime = Time.time;
        break;
      case Status.Invunerable:
        invunerableDuration = timeAmount;
        invunerableStartTime = Time.time;
        break;
    }

    currentStatus = currentStatus & status;
  }

  public void ResetStatus() {
    rootDuration = 0;
    rootStartTime = 0;
    invunerableDuration = 0;
    invunerableStartTime = 0;
    currentStatus = Status.None;
  }
  public bool HasStatus(Status status) {
    return (currentStatus & status) == status;
  }

  private void CheckExpiredStatus() {
    if(rootDuration > 0 && Time.time > rootStartTime + rootDuration) {
      rootDuration = 0;
      currentStatus = currentStatus & ~Status.Rooted;
    }

    if (invunerableDuration > 0 && Time.time > invunerableStartTime + invunerableDuration) {
      invunerableDuration = 0;
      currentStatus = currentStatus & ~Status.Invunerable;
    }
  }





}
