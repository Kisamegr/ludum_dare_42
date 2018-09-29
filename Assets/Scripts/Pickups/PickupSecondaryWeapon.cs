
public class PickupSecondaryWeapon : Pickup {
  protected override void Action(Player player) {
    throw new System.NotImplementedException();
  }

  protected override PickupType Type() {
    return PickupType.SecondaryWeapon;
  }
}
