using UnityEngine;

public class PickupSpecialSkill : Pickup {
  [Header("Special Skill Properties")]
  public SkillObject skillPrefab;

  protected override void Action(Player player) {
    player.SetSecondarySkill(skillPrefab);
  }

  protected override PickupType Type() {
    return PickupType.SpecialSkill;
  }
}
