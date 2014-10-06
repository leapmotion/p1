/*
	ABOUT THIS SCRIPT

In the grid-based movement example I demonstrated how to move square by sqare. Here it is taken
one step further, an entire chain of objects are moved. we store all segments (including the head)
in a list and issue movement for all of them in each turn. The head is moved manually and all other
segments take the psotion of their predecessor. The actual nake object is not moved at all, instead
it just manages the semgents.

This example demonstrates how a simple concept (moving from square to sqaure) can be used to solve
a problem that appears to be more complex. Instead of moving each segment we just move the head and
have the other egments follow it.
*/


using UnityEngine;
using System.Collections;
using System.Collections.Generic; // needed for List<T>
using System.Linq; // neeed for Last()

public class Snake : MonoBehaviour {
	public GFRectGrid grid; // the grid we use for moving the head
	public Transform segmentPrefab;
	public int startSize = 8; // the starting length of our snake
	public float speed = 5.0f; // snake movement speed (applies to each segment)

	private int grow = 0; // by how many segments to grow the snake (add to make the snake grow during gameplay)
	private bool movable = true; // this will be false while moving, we don't want movements to stack
	private bool onHold = true; // we will hold all movement until all segments are ready
	int movingSegments = 0; // keep track of moving segments, enable movement only after each one has finished moving

	private List<Transform> segments; // we will store our segments here, the first segment is the head, the last one the tail
	private enum Direction {N, E, S, W, C}; // the four possible directions and one neutral (centre) direction
	private Transform _transform; // cached the transform for some minor performance

	void Awake () {
		segments = new List<Transform> ();
		_transform = transform;
		BuildSnake (); // build the snake with its initial starting length
	}

	void BuildSnake () { // builds our initial snake by setting its initial growth to the starting size
		grow = Mathf.Max (1, startSize); // a snake needs at least a head, so make sure the minimum growth is 1
		Move (Vector3.zero); // moving by zero will not move the snake but it will still make a segment, our head, spawn
	}

	void Update () {
		// ignore all input if the snake is already moving or if the player isn't giving any directions
		if (!movable || (Input.GetAxisRaw("Horizontal") == 0 && Input.GetAxisRaw("Vertical") == 0))
			return;

		Direction dir = Direction.C; // otherwise pick the direction depending on the player's input
		if (Input.GetAxisRaw("Horizontal") == 1) {
			dir = Direction.E;
		} else if (Input.GetAxisRaw("Horizontal") == -1) {
			dir = Direction.W;
		} else if (Input.GetAxisRaw("Vertical") == 1) {
			dir = Direction.N;
		} else if (Input.GetAxisRaw("Vertical") == -1) {
			dir = Direction.S;
		}

		Move (DirectionToVector (dir)); // now move the snake (the direction is translated to a vector before that)
	}

	private Vector3 DirectionToVector (Direction dir) { // takes in one of the five directions and returns the corresponding vector
		Vector3 vec = Vector3.zero;
		switch (dir) {
		case Direction.N:
			vec = grid.up; break;
		case Direction.E:
			vec = grid.right; break;
		case Direction.S:
			vec = -grid.up; break;
		case Direction.W:
			vec = -grid.right; break;
		default:
			break; // if the direction was C we just keep the zero vector
		}
		return vec;
	}

	private void Move (Vector3 dir) {
		// first check if the destination we ant to move towards is within bounds; if not we print a message
		if (segments.Count > 0) { // if the snake has no head we can't do any check, so make sure there is at least one segment
			if (OutsideRange (grid.WorldToGrid (segments [0].position + dir))) {
				Debug.Log ("Illegal Move, turn around!");
				return;
			}
		}

		// now let's grow our snake; we grow only one segment per turn
		if (grow > 0) {
			Vector3 tailPos = segments.Count > 0 ? segments.Last ().position : _transform.position; // if the list is empty (no head) we spawn our head where the sanek object lies
			Transform newSegment = Instantiate (segmentPrefab) as Transform;
			newSegment.parent = _transform; // this isn't really needed, but it keeps things organized
			newSegment.position = tailPos; // place the new segment at the tail
			segments.Add (newSegment); // and append it to the list
			grow--; // decrease the growth counter
		}

		movable = false; // we are ready to move, so stop all movment input

		// from behind move every segment to the position of its predecessor; note that the animation is on hold until every movement has been assigned
		for (int i = segments.Count - 1; i > 0; i--) { 
			StartCoroutine (MoveWithSpeed (segments [i], segments [i - 1].position));
		}
		// finally, move the head
		StartCoroutine (MoveWithSpeed (segments [0], segments [0].position + dir));

		// all movement has been issued, now we can release the segments
		onHold = false;
	}

	private bool OutsideRange (Vector3 pos) {
		if (!grid.useCustomRenderRange) { // using the `size`
			if (Mathf.Abs (pos.x) > Mathf.Abs (grid.size.x) || Mathf.Abs (pos.y) > Mathf.Abs (grid.size.y)) {
				return true;
			} else {
				return false;
			}
		} else { // using `renderFrom` and `renderTo`
			if (pos.x > grid.renderTo.x || pos.y > grid.renderTo.y ||pos.x < grid.renderFrom.x || pos.y < grid.renderFrom.y){
				return true;
			} else {
				return false;
			}
		}
	}

	// This is a coroutine, it runs independetly of the script's Update method. Coroutines can be used to "do stuff over time".
	private IEnumerator MoveWithSpeed (Transform trns, Vector3 pos) {
		movingSegments++; // for each segment that has had its movement assigned we increment this counter
		while (onHold) // until all segments (including the head) have been assigned the animation is on hold
			yield return null;

		Vector3 startPos = trns.position;
		float distance = Vector3.Magnitude (startPos - pos);
		float startTime = Time.time;
		float distanceCovered = 0.0f;
		float t = 0.0f;

		while (t < 1.0f && distance > 0.0f) { // the check distance > 0.0f is needed to make sure we don't divide by 0
			distanceCovered = (Time.time - startTime) * speed; // s = v • t
			t = distanceCovered / distance;
			trns.position = Vector3.Lerp (startPos, pos, t);
			yield return null;
		}

		trns.position = pos;
		movingSegments--; // this segment is done, so decrease the ounter
		if (movingSegments == 0) { // once the last segment has finished its animation
			onHold = true; // all future animations will be on hold
			movable = true; // and the snake can receive movement input again
		}
	}
}
