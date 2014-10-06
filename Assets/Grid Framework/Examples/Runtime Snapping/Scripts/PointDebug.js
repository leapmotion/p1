#pragma strict

/*
	ABOUT THIS SCRIPT
	
This script works similar to SnappingUnits, except instead of
aligning an entire Transform it aligns just a Vector3. You can
use this approach if you want more control over the point itself
instead of manipulatingt he Transform directly.

The scale variable decides whether to align to vertices or edges,
depending on whether the components are even or odd multiples
of the grid's spacing.

Make sure gizmos are turned on in the game view to see the results.
*/

@script RequireComponent(Collider)
@script RequireComponent(GFGrid)

// the "scale" we use to determine how exactly to align the point (see the scripting reference for AlignVector3)
public var scale: Vector3 = Vector3 (1, 2, 1);

private var debugPoints: boolean = false; //this will be true while the player is holding down the mouse button above the grid
private var point: Vector3; // the point the pleyer is pointing to

private var col: Collider; // the collider of the grid (used for handling mouse input)
private var grid:GFGrid; // the grid component

function Awake () { // store components for later reference
	col = collider;
	grid = GetComponent.<GFGrid>();
}

function OnMouseDown () {
	debugPoints = true; // start the debugging process
}

function OnMouseUp () {
	debugPoints = false; // stop the debugging process
}

function Update () {
	if (!debugPoints) //only debug while the player is gragging the mouse over the grid
		return;

	//handle mouse input here
	var hit: RaycastHit;
	col.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), hit, Mathf.Infinity);
	point = hit.collider != null ? hit.point : transform.position;
}

function OnDrawGizmos () {
	if (!debugPoints)
		return;
	Gizmos.color = Color.green;
	Gizmos.DrawSphere (point, 0.3f); // where the plyer is pointing at
	Gizmos.color = Color.red;
	Gizmos.DrawSphere (grid.AlignVector3(point, scale), 0.3f); // the aligned point
}