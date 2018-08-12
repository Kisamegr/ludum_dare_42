using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour {
   
  public float invunerableDamage = 5f;
  public float dashSpeed = 10;
  public float dashTime = 0.5f;
  public float dashDamping = 0.12f;
  public GameObject primaryBulletPrefab; 
  public Status currentStatus;
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

  private float dashDuration = 0;
  private float dashStartTime = 0;

  private float afterDashDuration = 0;
  private float afterDashStartTime = 0;

  private float invunerableDuration = 0;
  private float invunerableStartTime = 0;
  private GAME _GAME;

  private int currentAmmo = 100;

  private Bullet currentSpecialBullet = null;
  private SpecialWeaponObject specialWeapon;

  private Vector2 dashDirection;

  [Flags]
  public enum Status {
    None = 0,
    Dashing = 1,
    AfterDashing = 2,
    Invunerable = 4
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

    Debug.Log(currentStatus);
    if(HasStatus(Status.Dashing)) {
      body.velocity = dashDirection * dashSpeed;
    }
    else if (HasStatus(Status.AfterDashing)) {
      body.velocity = Vector2.Lerp(body.velocity, velocity, Time.deltaTime / dashDamping);
    }
    else
      body.velocity = velocity;
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
      
      bullet.Shoot(bulletSpeed);

      lastShootTime = Time.time;
    }

  }

  private Bullet CreateBullet(GameObject bulletPrefab) {
    return Instantiate(bulletPrefab, position: bulletSpawnOffset.position, rotation: transform.rotation).GetComponent<Bullet>();
  }

  public void IncreaseBulletPower()
  {
    currentBulletPowerLevel += 1;
    currentBulletPowerLevel = Mathf.Min(currentBulletPowerLevel, playerLevels.bulletDamageLevels.Length - 1);
    bulletDamage = playerLevels.bulletDamageLevels[currentBulletPowerLevel];
    bulletSize = playerLevels.bulletSizeLevels[currentBulletPowerLevel]; 
  }

  public void IncreaseBulletSpeed()
  {
    currentBulletSpeedLevel += 1;  
    currentBulletSpeedLevel = Mathf.Min(currentBulletSpeedLevel, playerLevels.bulletSpeedLevels.Length - 1);
    bulletSpeed = playerLevels.bulletSpeedLevels[currentBulletSpeedLevel];
    shootInterval = playerLevels.bulletShootIntervalLevels[currentBulletSpeedLevel];
  }

  public void IncreasePlayerSpeed()
  {
    currentSpeedLevel += 1;  
    currentSpeedLevel = Mathf.Min(currentSpeedLevel, playerLevels.playerSpeedLevels.Length - 1);
    maxSpeed = playerLevels.playerSpeedLevels[currentSpeedLevel]; 
  }


  void SpecialShoot()
  {
    if(Input.GetMouseButtonDown(1)){
      Debug.Log("special shoot 1 ");
    }

    if (specialWeapon != null) {

      if (Input.GetMouseButtonDown(1)) {
        Debug.Log("special shoot 2 ");
        if (specialWeapon.hasSecondActivation && currentSpecialBullet != null) {
          currentSpecialBullet.HitTarget(currentSpecialBullet.gameObject);
        }
        if (lastSpecialShootTime + specialWeapon.cooldown < Time.time) {
          switch (specialWeapon.type) {
            case SpecialWeaponObject.WeaponType.Dash:
              SetStatus(Status.Dashing | Status.Invunerable, dashTime);
              dashDirection = transform.right;
              break;
            case SpecialWeaponObject.WeaponType.Rocket:
              RocketBullet rocket = (RocketBullet) CreateBullet(specialWeapon.bullet);
              rocket.Shoot();
              currentSpecialBullet = rocket;
              break;
            case SpecialWeaponObject.WeaponType.Homing:
              float maxAngle = 45;
              float angleSpacing = maxAngle / specialWeapon.count;
              float currentAngle = -maxAngle/2;
              for(int i=0; i<specialWeapon.count; i++) {
                HomingMissle missle = (HomingMissle) CreateBullet(specialWeapon.bullet);
                missle.ShootOffset(currentAngle);
                currentAngle += angleSpacing;
              }
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
  }

  public void SetSpecialWeapon(SpecialWeaponObject weapon) {
    specialWeapon = weapon;
    currentSpecialBullet = null;
    lastSpecialShootTime = 0;
    currentAmmo = specialWeapon.ammo;
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

  private void StopShaking() {
    var vcam = GameObject.Find("Follow Cam").GetComponent<Cinemachine.CinemachineVirtualCamera>();
    var noise = vcam.GetCinemachineComponent<Cinemachine.CinemachineBasicMultiChannelPerlin>();

    noise.m_AmplitudeGain = 0;
    noise.m_FrequencyGain = 0;
  }



  public void GetPushed(float force) {

  }

  public void SetStatus(Status status, float timeAmount = 0) {
    if ((status & Status.Dashing) == Status.Dashing) {
      dashDuration = timeAmount;
      dashStartTime = Time.time;
    }

    if ((status & Status.AfterDashing) == Status.AfterDashing) {
      afterDashDuration = timeAmount;
      afterDashStartTime = Time.time;
    }

    if ((status & Status.Invunerable) == Status.Invunerable) {
      invunerableDuration = timeAmount;
      invunerableStartTime = Time.time;
    }

    currentStatus = currentStatus | status;
  }

  public void ResetStatus() {
    dashDuration = 0;
    dashStartTime = 0;
    afterDashDuration = 0;
    afterDashStartTime = 0;
    invunerableDuration = 0;
    invunerableStartTime = 0;
    currentStatus = Status.None;
  }
  public bool HasStatus(Status status) {
    return (currentStatus & status) == status;
  }

  private void CheckExpiredStatus() {
    if (dashDuration > 0 && Time.time > dashStartTime + dashDuration) {
      dashDuration = 0;
      currentStatus = currentStatus & ~Status.Dashing;

      SetStatus(Status.AfterDashing, 1);
    }

    if (afterDashDuration > 0 && Time.time > afterDashStartTime + afterDashDuration) {
      afterDashDuration = 0;
      currentStatus = currentStatus & ~Status.AfterDashing;
    }

    if (invunerableDuration > 0 && Time.time > invunerableStartTime + invunerableDuration) {
      invunerableDuration = 0;
      currentStatus = currentStatus & ~Status.Invunerable;
    }
  }





}
