
public class PickupBulletSpeed : Pickup {
  protected override void Action(Player player) {
    player.IncreaseBulletSpeed();
  }

  protected override PickupType Type() {
    return PickupType.BulletSpeed;
  }
}
