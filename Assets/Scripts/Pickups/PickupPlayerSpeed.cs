
public class PickupPlayerSpeed : Pickup {
  protected override void Action(Player player) {
    player.IncreasePlayerSpeed();
  }

  protected override PickupType Type() {
    return PickupType.PlayerSpeed;
  }
}
