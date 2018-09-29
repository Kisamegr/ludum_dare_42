using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupMana : Pickup {
  [Header("Mana Properties")]
  public float manaFillAmount;

  protected override void Action(Player player) {
    player.FillMana(manaFillAmount);
  }

  protected override PickupType Type() {
    return PickupType.Mana;
  }
}
