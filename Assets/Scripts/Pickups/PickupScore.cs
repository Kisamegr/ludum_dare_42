
public class PickupScore : Pickup {

  public float spinMultiplier = 300f;

  protected override void Start() {
    base.Start();
    _rigidbody.AddTorque((UnityEngine.Random.value - 0.5f) * spinMultiplier);
  }

  protected override void Action(Player player) {
    // Do nothing, the score is added in OnCollision
  }

  protected override PickupType Type() {
    return PickupType.Score;
  }

}
