using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class SkillObject : ScriptableObject {

    public string weaponName;

    [SerializeField]
    protected float cooldown;
    [SerializeField]
    protected float manaCost;


    public virtual float Cooldown { get { return cooldown; } }

    public virtual float ManaCost { get { return manaCost; } }

    public Sprite icon;

    public Color icon_color;

    public abstract void Cast(Player player);

}
