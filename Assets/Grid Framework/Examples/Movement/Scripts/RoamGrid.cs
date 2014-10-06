using UnityEngine;
using System.Collections;

/*
	ABOUT THIS SCRIPT
	
This script, when attached to an object, makes it walk around the grid
randomly from one face to an adjacent one. The object will never walk
off the grid. You can even change the grid's size during runtime and the
object will adapt its behaviour. If you shrink the grid faster than the
object can react it will head straigt back inside, but it will take a
few turns to do so, depending on how far outside it is.

This script demonstrates how you can easily perform grid-based movement,
automate it and limit it to a certain range while still retaining the
option of dynamically altering the setup during runtime.
*/

public class RoamGrid : MonoBehaviour {

	public GFGrid grid;
	public float roamingSpeed = 1.0f;
	
	//whether the object is to move or not, to where and how fast
	private bool doMove = false;
	private Vector3 start;
	private Vector3 goal;
	private float roamingFactor;
	
	//cache the transform for performance
	private Transform cachedTransform;

	void Awake () {
		cachedTransform = transform;
	
		//make a check to prevent getting stuck in a null exception
		if(grid){
			//snap to the grid  no matter where we are
			grid.AlignTransform(cachedTransform);
		}
	}
	
	void Update () {
		if(!grid)
			return;
		
		if(doMove){
			//move towards the desination
			cachedTransform.position = Vector3.Lerp (start, goal, roamingFactor);
			roamingFactor += Time.deltaTime * roamingSpeed;
			//check if we reached the destination (use a certain tolerance so we don't miss the point becase of rounding errors)
			if(Mathf.Abs(cachedTransform.position.x - goal.x) < 0.01f && Mathf.Abs(cachedTransform.position.y - goal.y) < 0.01f)
				doMove = false;
			//if we did stop moving
		} else{
			//make sure the speed is always positive
			if(roamingSpeed < 0.01f)
				roamingSpeed = 0.01f;
			//find the next destination
			start = cachedTransform.position;
			goal = FindNextFace();
			//resume movement with the new goal
			doMove = true;
			roamingFactor = 0;
		}
	}

	Vector3 FindNextFace () {
		//we will be operating in grid space, so convert the position
		Vector3 newPosition = grid.WorldToGrid(cachedTransform.position);
		
		//first let's pick a random number for one of the four possible directions
		int n = Random.Range(0, 4);
		//now add one grid unit onto position in the picked direction
		if(n == 0){
			newPosition = newPosition + new Vector3(1,0,0);
		} else if(n == 1){
			newPosition = newPosition + new Vector3(-1,0,0);
		} else if(n == 2){
			newPosition = newPosition + new Vector3(0,1,0);
		} else if(n == 3){
			newPosition = newPosition + new Vector3(0,-1,0);
		}
		//if we would wander off beyond the size of the grid turn the other way around
		for (int j = 0; j < 2; j++) {
			if(Mathf.Abs(newPosition[j]) > grid.size[j])
				newPosition[j] -= Mathf.Sign(newPosition[j]) * 2.0f;
		}
		
		//return the position in world space
		return grid.GridToWorld(newPosition);
	}

	void OnDrawGizmos () {
		if(!cachedTransform)
			return;
		Gizmos.color = Color.red;
		Gizmos.DrawSphere (goal, 0.3f);
		Gizmos.color = Color.green;
		Gizmos.DrawLine(cachedTransform.position, goal);
	}
}
