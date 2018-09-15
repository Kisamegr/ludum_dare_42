using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "MissileSkill", menuName = "Missile skill")]
[Serializable]
public class HomingMissileSkillObject : SkillObject {

    public GameObject missileBullet;
    public int numberOfMissiles;

    override
    public void Cast(Player player)
    {
        float maxAngle = 45;
        float angleSpacing = maxAngle / numberOfMissiles;
        float currentAngle = -maxAngle / 2;
        for (int i = 0; i < numberOfMissiles; i++)
        {
            HomingMissile missle = (HomingMissile)player.CreateBullet(missileBullet);
            missle.ShootOffset(currentAngle);
            currentAngle += angleSpacing;
        }
    }
}
