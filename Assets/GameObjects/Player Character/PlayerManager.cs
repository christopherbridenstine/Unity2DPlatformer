using UnityEngine;
using System.Collections;

public class PlayerManager : MonoBehaviour {
	// Keyboard controls
	public KeyCode left;
	public KeyCode right;
	public KeyCode jump;

	// Player rigidbody
	private Rigidbody2D rb;
	public float maxRunSpeed;
	
	// Movemement state machine
	private int moveState;
	private int stop = 0;
	private int moveRight = 1;
	private int moveLeft = -1;
	private bool jumpState;

	// Colliders
	private BoxCollider2D eCollider;
	private int environment;


	// Use this for initialization
	void Start () {
		left = KeyCode.LeftArrow;
		right = KeyCode.RightArrow;
		jump = KeyCode.Space;
		maxRunSpeed = 10.0f;
		moveState = 0;
		jumpState = false;

		rb = GetComponent<Rigidbody2D> ();

		eCollider = GetComponent<BoxCollider2D> ();
		environment = LayerMask.GetMask("Environment Colliders");	}
	
	// Update is called once per frame
	void Update () {
		GetUserInput ();
		MoveCharacter ();

	}

	// Set current moveState based on Keyboard input
	void GetUserInput() {
		bool lKey = Input.GetKey (left);
		bool rKey = Input.GetKey (right);
		bool jKey = Input.GetKey (jump);

		// Set move left or right state
		if (lKey && !rKey) {
			moveState = moveLeft;
		} else if (!lKey && rKey) {
			moveState = moveRight;
		} else if (!lKey && !rKey) {
			moveState = stop;
		}

		// Set jumpState
		if (jKey) {
			jumpState = true;
		} else {
			jumpState = false;
		}
	}

	// Move Character based on movestate
	void MoveCharacter(){
		AddXForce ();
		AddJumpForce ();

	}

	// Determine if an X force should be added this frame and add it
	void AddXForce(){
		// 
		float newVelocityX;
		if (moveState == 1) {
			newVelocityX = maxRunSpeed;
		} else if (moveState == -1) {
			newVelocityX = -maxRunSpeed;
		} else {
			newVelocityX = 0.0f;
		}
		Vector2 newVelocity = new Vector2(newVelocityX, rb.velocity.y);
		Vector2 newForce = getForce (newVelocity);
		rb.AddForce (newForce);
	}

	// Determine if a jump force should be added this frame and adds it
	void AddJumpForce(){
		if (jumpState == true && IsOnGround ()) {
			rb.AddForce (new Vector2 (0.0f, 8.0f), ForceMode2D.Impulse);
		} else if (jumpState == true && !IsOnGround ()) {

		}
	}

	// Determine the force necessary to change velocity to newVelocity
	Vector2 getForce(Vector2 newVelocity){
		if (newVelocity.x >= maxRunSpeed) {
			newVelocity.x = maxRunSpeed;
		}
		Vector2 diffVelocity = newVelocity - rb.velocity;
		Vector2 neededAccel = diffVelocity / Time.deltaTime;
		if (!IsOnGround ()) {
			neededAccel = neededAccel * 0.75f;
		}
		Vector2 neededForce = neededAccel * rb.mass;
		return neededForce;
	}


	bool IsOnGround(){
		return(eCollider.IsTouchingLayers(environment) && (rb.velocity.normalized.y < .1f));
	}
}
