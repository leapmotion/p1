using UnityEngine;
using System.Collections;

/*
	ABOUT THIS SCRIPT
	
This script is used to simulate the behaviour a rotary dial found on old telephones.
You could use these techniques to create interesting GUIs or puzzles with spinning
interfaces for the player.

We achieve the result by first deciding which sector was clicked and then using the
sector and the gird's angle to determine the total angle for the rotation. The actual
game logic can be nicely encapsulated in OnReachStop, solating it from the rotation.
*/

[RequireComponent(typeof(GFPolarGrid))]
[RequireComponent(typeof(MeshCollider))]
public class RotaryDial : MonoBehaviour {
	
	public float rotationSpeed = 75.0f; // speed of the dial rotation
	public float boost = 2.0f; // speed boost factor when the dial is returning 
	public int dialOffset = 1; // how many sectors is the first number offset from the first sector
	public int lowerLimit = 1; // if the number clicked is lower than this do nothing
	public int upperLimit = 10; // if the number clicked is larger than this do nothing

	private bool enableClick = true; // dial becomes unclickable while it is rotating

	// grid for grid logic, collider for clicking and transfor for applying the rotation
	private GFPolarGrid grid;
	private MeshCollider mc;
	private Transform _transform;

	void Awake () {
		grid = GetComponent<GFPolarGrid> ();
		mc = GetComponent<MeshCollider> ();
		_transform = transform;
	}

	// if the dial is spinning ignore clicks, else handle input and rotate the dial
	void OnMouseDown () {
		if (!enableClick)
			return;

		int cell = HandleMouseInput ();
		RotateDial (cell);
	}

	// this method returns the number of the sector that was clicked
	private int HandleMouseInput () {
		// use raycasting to get the world coordinates where the player clicked
		RaycastHit hit;
		mc.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out hit, Mathf.Infinity);
		// the Y-coordinate is our sector; round it to the nearest integer
		return Mathf.FloorToInt (grid.NearestFaceG (hit.point).y) - (dialOffset - 1); // subtract the offset to get the number instead of the actual cell
	}

	// this is where we compute the rotation angle and tell the script to apply the rotation
	private void RotateDial (int cell) {
		// if the cell is outside our range we don't do anything and exit the method
		if (cell < lowerLimit || cell > upperLimit)
			return;
		// the index tells us how many sectos we need to rotate through, angleDeg is the degree of one sector, dialOffset is a fixed number of addition sectors
		float angle = (cell + dialOffset) * grid.angleDeg;
		// we apply the rotation over time via a custom coroutine. This is advanced stuff and not related to Grid Framework so don't worry if you don't understand it
		StartCoroutine (RotateOverTime (angle, cell));
	}

	// this method is called after the dial has finished rotating to, but before it starts rotating back. Put your game logic in here
	void OnReachStop (int cell) {
		Debug.Log ("*ring* " + (cell % 10));
	}

	// this function is for use in coroutines, it will rotate the object around it's own origin for a certain amount of degrees with fixed speed
	IEnumerator RotateOverTime (float angle, int cell = 0) {
		enableClick = false; // disable clicks, we don't want to interfere

		float startTime = Time.time; // the time when we started this coroutin
		float angleCovered = 0.0f; // how much we have moved at a given point in time
		float t = 0.0f; // the fraction of the full angle

		while (t < 1.0f) {
			angleCovered = (Time.time - startTime) * rotationSpeed; // the covered angle depends on the time passed and the speed
			t = angleCovered / angle; // t is the fraction of the full angle, update it
			_transform.rotation = Quaternion.identity; // reset the rotation
			_transform.RotateAround (_transform.position, -Vector3.forward, t * angle); // and apply the updated angle
			yield return null; // this loop will repeat as long as out fraction hasn't been filled
		}

		OnReachStop (cell); // now call the method for game logic

		startTime = Time.time; // it's time to rotate back, start the math over again
		angleCovered = 0.0f;
		t = 0;
		while (t < 1.0f) {
			angleCovered = (Time.time - startTime) * boost * rotationSpeed;
			t = angleCovered / angle;
			_transform.rotation = Quaternion.identity;
			_transform.RotateAround (_transform.position, -Vector3.forward, (1 - t) * angle); // similar to the above, excpet now (1 - t) because we go backwards
			yield return null;
		}

		_transform.rotation = Quaternion.identity; // force reset to iron out inaccuracies due to rounding errors
		enableClick = true; // re-enable clicks
	}
}
