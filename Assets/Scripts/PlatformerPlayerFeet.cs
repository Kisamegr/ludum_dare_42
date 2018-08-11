using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformerPlayerFeet : MonoBehaviour {

  public bool grounded = false;

  private void OnTriggerEnter(Collider other) {
    grounded = true;
  }

  private void OnTriggerExit2D(Collider2D collision) {
    grounded = false;
  }

  private void OnTriggerStay2D(Collider2D collision) {
    grounded = true;
  }
}
