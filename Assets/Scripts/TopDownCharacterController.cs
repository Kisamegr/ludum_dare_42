using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class TopDownCharacterController : MonoBehaviour {

  public float maxSpeed;


  Rigidbody2D body;

  // Use this for initialization
  void Start () {
    body = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {
    CalculateVelocity();

	}

  void CalculateVelocity() {
    Vector2 velocity;
    velocity.x = Input.GetAxis("Horizontal") * maxSpeed;
    velocity.y = Input.GetAxis("Vertical") * maxSpeed;

    body.velocity = velocity;
  }
}
