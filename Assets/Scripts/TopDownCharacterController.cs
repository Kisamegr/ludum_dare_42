using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class TopDownCharacterController : MonoBehaviour {

  public float maxSpeed;
  public float speedDamping = 0.12f;
  public GameObject primaryBulletPrefab;
  public float shootInterval = 0.1f;
  public float bulletSpeed = 10f;
  public Status currentStatus;
  public SpecialWeaponObject specialWeapon;

  private float lastShootTime = 0;
  private float lastSpecialShootTime = 0;
  private Transform bulletSpawnOffset;

  Rigidbody2D body;

  private float rootDuration = 0;
  private float rootStartTime = 0;

  private float invunerableDuration = 0;
  private float invunerableStartTime = 0;

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
      float damageAmount = 5f; //TODO: make it a class variable
      Bullet bullet = CreateBullet(primaryBulletPrefab);
      bullet.Shoot(bulletSpeed);

      lastShootTime = Time.time;
    }

  }

  private Bullet CreateBullet(GameObject bulletPrefab) {
    return Instantiate(bulletPrefab, position: bulletSpawnOffset.position, rotation: transform.rotation).GetComponent<Bullet>();
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
            rocket.Shoot(bulletSpeed);
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

    }
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
