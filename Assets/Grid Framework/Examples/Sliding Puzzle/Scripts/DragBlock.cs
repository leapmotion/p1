using UnityEngine;
using System.Collections;

public class DragBlock : MonoBehaviour {

	///<summary>Whether this block is being dragged by the player.</summary>
	public bool beingDragged;

	///<summary>The offset between the touch point and the centre.</summary>
	/// The player will rarely click the block at its centre, but the block stil lhas to move around it. This offset is applied every time the player drags
	/// the block to make up for that difference.
	private Vector3 touchOffset;

	///<summary>The last known save position we were able to snap to (the foresight works only from snap to snap).</summary>
	private Vector3 lastSnap;

	///<summary>We can only move the block within these bounds (the grid itself is the largest possible bound).</summary>
	private Vector3[] bounds;

	/// <summary>Tells us whether diagonal movement is allowed; if not it will be canceled. (work in progress)</summary>
	private bool[] diagonalBounds;

	void Start () {
		beingDragged = false; // When the game starts no blocks are being dragged.
		SlidingPuzzleExample.RegisterObstacle(transform, false); // Register this block in the matrix as occupied space.
	}
	
	void OnMouseDown(){
		beingDragged = true; // Start dragging.
		touchOffset = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position; //offset between the cursor and the block centre
		lastSnap = transform.position; // this is obviously where we snapped the last time
		bounds = SlidingPuzzleExample.CalculateSlidingBounds(transform.position, transform.lossyScale); // create default bounds
		SlidingPuzzleExample.RegisterObstacle(transform, true); // marks this space as free in the matrix (or else we won't be able to return back here)
	}
	
	void OnMouseUp(){
		beingDragged = false; // stop dragging
		SlidingPuzzleExample.mainGrid.AlignTransform(transform); // snap into position precisely
		transform.position = ClampPosition(transform.position); // clamp the position to be safe (because of possible rounding errors above)
		lastSnap = transform.position; //this is out last save position
		SlidingPuzzleExample.RegisterObstacle(transform, false); // mark the space as occupied again
	}
	
	void FixedUpdate () {
		if(beingDragged)
			Drag();
	}
	
	///<summary>This is where the dragging logic takes places.</summary>
	void Drag(){
		// the destination is where the cursor points (minus the offset) clamped by the bounds
		Vector3 destination = ClampPosition(Camera.main.ScreenToWorldPoint(Input.mousePosition) - touchOffset);
		destination.z = transform.position.z;
		
		// now use that information to get the new bounds
		bounds = SlidingPuzzleExample.CalculateSlidingBounds(lastSnap, transform.lossyScale);
		
		// simulate a snap to the grid so we can get potentially new bounds in the next step
		lastSnap = ClampPosition(SlidingPuzzleExample.mainGrid.AlignVector3(destination, transform.lossyScale));
		
		// move to the destination!
		transform.position = destination;
	}
	
	///<summary>Doesn't let the block move out of the grid (uses the grid's renderFrom and renderTo).</summary>
	Vector3 ClampPosition(Vector3 vec){
		// if there are no other blocks in the way then at least the grid's RenderFrom and RenderTo must serve as bounds or else we go off grid
		Vector3 lowerLimit = Vector3.Max(SlidingPuzzleExample.mainGrid.GridToWorld(SlidingPuzzleExample.mainGrid.renderFrom) + 0.5f * transform.lossyScale, bounds[0]);
		Vector3 upperLimit = Vector3.Min(SlidingPuzzleExample.mainGrid.GridToWorld(SlidingPuzzleExample.mainGrid.renderTo) - 0.5f * transform.lossyScale, bounds[1]);
		
		upperLimit.z = transform.position.z; lowerLimit.z = transform.position.z;
		
		// this method of using the maximum of the minimum is similar to Unity's Mathf.Clamp(), except it is for vectors
		return Vector3.Max(lowerLimit, Vector3.Min(upperLimit, vec));
	}

	// work in progress...
	void RestrictDiagonally(ref Vector3 dest, ref bool[] restrict) {
		Vector3 diff = dest - transform.position;

		if (diff.x > 0) { // East
			if (restrict[0] && diff.y > 0) {        // North-East
				Debug.Log("block NE!");
				if ((diff.y > diff.x) ||transform.position.y > lastSnap.y) {
					dest.x = lastSnap.x; // cancel horizontal
					Debug.Log("cancel horiz.");
				} else {
					dest.y = lastSnap.y; // cancel vertical
				}
			} else if (restrict[3] && diff.y < 0) { // South-East
				if (-diff.y > diff.x) {
					dest.x = lastSnap.x; // cancel horizontal
				} else {
					dest.y = lastSnap.y; // cancel vertical
				}
			}
		} else { // West
			if (restrict[1] && diff.y > 0) {        // North-West
				if (diff.y > -diff.x) {
					dest.x = lastSnap.x; // cancel horizontal
				} else {
					dest.y = lastSnap.y; // cancel vertical
				}
			} else if (restrict[2] && diff.y > 0) { // South-West
				if (diff.y < diff.x) {
					dest.x = lastSnap.x; // cancel horizontal
				} else {
					dest.y = lastSnap.y; // cancel vertical
				}
			}
		}
	}
}