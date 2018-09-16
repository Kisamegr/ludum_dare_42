using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{

    public float invunerableDamage = 5f; 
    public GameObject primaryBulletPrefab;
    public Status currentStatus;
    public PlayerLevelsObject playerLevels;
    public float bulletDamage = 1f;

    private float lastShootTime = 0;
    private float lastSpecialShootTime = 0;
    private Transform bulletSpawnOffset;


    private int currentSpeedLevel = 0;
    private float maxSpeed;

    private int currentBulletSpeedLevel = 0;
    private float bulletSpeed;
    private float shootInterval;

    private int currentBulletPowerLevel = 0;
    private int noBullets;
    private bool bulletsPenetrate;

    Rigidbody2D body;

    private float dashDuration = 0;
    private float dashStartTime = 0;
    public float dashDamping = 0.12f;

    private float afterDashDuration = 0;
    private float afterDashStartTime = 0;

    private float invunerableDuration = 0;
    private float invunerableStartTime = 0;
    private GAME _GAME;
     

    private Vector2 dashDirection;

    [SerializeField]
    private float maxMana;
    [SerializeField]
    private float manaRegen;

    private float currentMana;

    [SerializeField]
    private DashSkillObject dashSkill;



    private SkillObject activeSecodarySkill;
    private SkillObject secondarySkillPrefab;

    [Flags]
    public enum Status
    {
        None = 0,
        Dashing = 1,
        AfterDashing = 2,
        Invunerable = 4
    }

    // Use this for initialization
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        bulletSpawnOffset = transform.Find("BulletSpawnOffset");
        _GAME = GAME.Instance();

        maxSpeed = playerLevels.playerSpeedLevels[0];
        bulletSpeed = playerLevels.bulletSpeedLevels[0];
        shootInterval = playerLevels.bulletShootIntervalLevels[0];
        noBullets = playerLevels.bulletCountLevels[0];  
        currentMana = maxMana;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        Rotate();
        PrimaryShoot();
        SpecialShoot(); 
        Dash();

        CheckExpiredStatus();


        currentMana = Mathf.Min(maxMana, currentMana + manaRegen * Time.deltaTime);
        UiManager.Instance().UpdateMana(1.0f * currentMana / maxMana);
    }

    void Move()
    {
        Vector2 velocity;
        velocity.x = Input.GetAxis("Horizontal") * maxSpeed;
        velocity.y = Input.GetAxis("Vertical") * maxSpeed;

        if (!HasStatus(Status.Dashing) && !HasStatus(Status.AfterDashing))
        {
            body.velocity = velocity;
        }
        else if (HasStatus(Status.AfterDashing))
        {
            body.velocity = Vector2.Lerp(body.velocity, velocity, Time.deltaTime / dashDamping);
        }
    }

    void Rotate()
    {
        if (InputHandler.Instance().GetInputType == InputType.MOUSE_KEYBOARD)
        {
            Vector2 dir = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
            transform.rotation = Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.right, dir));
        }
        if (InputHandler.Instance().GetInputType == InputType.JOYSTICK)
        {
            Vector2 dir = new Vector2(Input.GetAxis("Horizontal2"), Input.GetAxis("Vertical2")).normalized;
            if (dir != Vector2.zero)
                transform.rotation = Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.right, dir));
        }
    }

    private bool ShouldShoot()
    {

        if(Time.time - lastShootTime > shootInterval)
        { 
            if (Input.GetButton("Primary"))
                return true;
            
            //Controller input
            float stickPressure = new Vector2(Input.GetAxis("Horizontal2"), Input.GetAxis("Vertical2")).magnitude;
            float shootThreshold = 0.1f;
            if(stickPressure > shootThreshold)
                return true;
            
        }

        return false;
    }




    void PrimaryShoot()
    {

        if (ShouldShoot())
        {

            Vector2 perpendicularVector = Vector2.Perpendicular(bulletSpawnOffset.position - transform.position).normalized;

            List<Bullet> bullets = new List<Bullet>();
            for (int i = 0; i < noBullets; i++)
            {
                float angleOffset = (i - (noBullets - 1) / 2) * 3f;
                Bullet bullet = CreateBullet(primaryBulletPrefab, rotationOffset: angleOffset);
                bullets.Add(bullet);
            }
            float singleBulletSpace = bullets[0].GetComponent<BoxCollider2D>().bounds.extents.y + 0.05f;
            float bulletSpace = singleBulletSpace * noBullets;

            for (int i = 0; i < noBullets; i++)
            {
                Bullet bullet = bullets[i];
                Vector2 spawnLocation = bulletSpawnOffset.position + (-bulletSpace / 2 + singleBulletSpace / 2 + i * bulletSpace / noBullets) * (Vector3)perpendicularVector;
                bullet.transform.position = spawnLocation;
                bullet.damageAmount = bulletDamage;
                bullet.CanPenetrate = bulletsPenetrate;
                bullet.Shoot(bulletSpeed);
            }
            lastShootTime = Time.time;
        }

    }

    public Bullet CreateBullet(GameObject bulletPrefab, Vector3? spawnLocation = null, float rotationOffset = 0f)
    {
        if (spawnLocation == null)
        {
            spawnLocation = bulletSpawnOffset.position;
        }
        return Instantiate(bulletPrefab, position: spawnLocation.Value, rotation: Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z + rotationOffset)).GetComponent<Bullet>();
    }

    public void IncreaseBulletPower()
    {
        currentBulletPowerLevel += 1;
        currentBulletPowerLevel = Mathf.Min(currentBulletPowerLevel, playerLevels.bulletCountLevels.Length - 1);
        noBullets = playerLevels.bulletCountLevels[currentBulletPowerLevel];
        bulletsPenetrate = playerLevels.bulletPenetrationLevels[currentBulletPowerLevel];
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
        if (activeSecodarySkill != null)
        {
            if (Input.GetButtonDown("Secondary")
                && lastSpecialShootTime + activeSecodarySkill.Cooldown < Time.time
                && currentMana >= activeSecodarySkill.ManaCost){
                 
                currentMana -= activeSecodarySkill.ManaCost;
                activeSecodarySkill.Cast(this);
                lastSpecialShootTime = Time.time;    
            }
        }
    }


    void Dash()
    {
        if (Input.GetButtonDown("Dash")
            && lastSpecialShootTime + activeSecodarySkill.Cooldown < Time.time
            && currentMana >= activeSecodarySkill.ManaCost)
        {
            currentMana -= dashSkill.ManaCost;
            dashSkill.Cast(this);
            lastSpecialShootTime = Time.time;
        }
    }





    public void SetSecondarySkill(SkillObject skillPrefab)
    {
        secondarySkillPrefab = skillPrefab;
        activeSecodarySkill = Instantiate(skillPrefab);
        lastSpecialShootTime = 0;
        currentMana = maxMana;

        UiManager.Instance().SetSecondarySkill(secondarySkillPrefab);
    }

    public void GetPushed(float pushAmount)
    {

    }

    public void FillMana(float manaAmount)
    {
        currentMana = Mathf.Min(maxMana, currentMana + manaAmount);
    }

    public void GetDamage(float damageAmount)
    {
        if (!HasStatus(Status.Invunerable))
        {
            _GAME.ChangeWallBorders(false, -damageAmount);

            var vcam = GameObject.Find("Follow Cam").GetComponent<Cinemachine.CinemachineVirtualCamera>();
            var noise = vcam.GetCinemachineComponent<Cinemachine.CinemachineBasicMultiChannelPerlin>();

            noise.m_AmplitudeGain = 3;
            noise.m_FrequencyGain = 5;

            Invoke("StopShaking", 0.5f);
        }
    }

    private void StopShaking()
    {
        var vcam = GameObject.Find("Follow Cam").GetComponent<Cinemachine.CinemachineVirtualCamera>();
        var noise = vcam.GetCinemachineComponent<Cinemachine.CinemachineBasicMultiChannelPerlin>();

        noise.m_AmplitudeGain = 0;
        noise.m_FrequencyGain = 0;
    }


     

    public void SetStatus(Status status, float timeAmount = 0)
    {
        if ((status & Status.Dashing) == Status.Dashing)
        {
            dashDuration = timeAmount;
            dashStartTime = Time.time;

            GetComponent<TrailRenderer>().enabled = true;
            transform.Find("Shield").gameObject.SetActive(true);
            GetComponent<AudioSource>().Play();
        }

        if ((status & Status.AfterDashing) == Status.AfterDashing)
        {
            afterDashDuration = timeAmount;
            afterDashStartTime = Time.time;

            GetComponent<TrailRenderer>().enabled = false;
            transform.Find("Shield").gameObject.SetActive(false);
        }

        if ((status & Status.Invunerable) == Status.Invunerable)
        {
            invunerableDuration = timeAmount;
            invunerableStartTime = Time.time;
        }

        currentStatus = currentStatus | status;
        currentStatus = currentStatus | status;
    }

    public void ResetStatus()
    {
        dashDuration = 0;
        dashStartTime = 0;
        afterDashDuration = 0;
        afterDashStartTime = 0;
        invunerableDuration = 0;
        invunerableStartTime = 0;
        currentStatus = Status.None;

    }
    public bool HasStatus(Status status)
    {
        return (currentStatus & status) == status;
    }

    private void CheckExpiredStatus()
    {
        if (dashDuration > 0 && Time.time > dashStartTime + dashDuration)
        {
            dashDuration = 0;
            currentStatus = currentStatus & ~Status.Dashing;

            SetStatus(Status.AfterDashing, 1);
        }

        if (afterDashDuration > 0 && Time.time > afterDashStartTime + afterDashDuration)
        {
            afterDashDuration = 0;
            currentStatus = currentStatus & ~Status.AfterDashing;
        }

        if (invunerableDuration > 0 && Time.time > invunerableStartTime + invunerableDuration)
        {
            invunerableDuration = 0;
            currentStatus = currentStatus & ~Status.Invunerable;
        }
    }





}
