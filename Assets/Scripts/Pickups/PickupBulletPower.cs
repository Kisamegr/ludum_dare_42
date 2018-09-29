
public class PickupBulletPower : Pickup {
  protected override void Action(Player player) {
    player.IncreaseBulletPower();
  }

  protected override PickupType Type() {
    return PickupType.BulletPower;
  }
}
