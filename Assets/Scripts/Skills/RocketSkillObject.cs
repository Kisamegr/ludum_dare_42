using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "RocketSkill", menuName = "Rocket skill")]
[Serializable]
public class RocketSkillObject : SkillObject
{
    [SerializeField]
    private GameObject rocketBullet;

    [SerializeField]
    private double explosionRadius = 1;

    private RocketBullet rocketSpawn = null;

    override
    public void Cast(Player player)
    {
        if(rocketSpawn == null)
        {
            rocketSpawn = (RocketBullet)player.CreateBullet(rocketBullet);
            rocketSpawn.Shoot();
        }
        else
        {
            rocketSpawn.HitTarget(rocketSpawn.gameObject);
            rocketSpawn = null;
        }
    }

    override
    public float Cooldown
    {
        get
        {
            return rocketSpawn == null ? cooldown : 0;
        }
    }


    override
    public float ManaCost
    {
        get
        {
            return rocketSpawn == null ? manaCost : 0;
        }
    }

}
