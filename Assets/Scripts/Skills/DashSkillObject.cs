using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DashSkill", menuName = "Dash skill")]
public class DashSkillObject : SkillObject
{
    override
    public void Cast(Player player)
    {
        player.Dash();
    }
}
