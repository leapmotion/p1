
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Camera))]
public class InfinityCamera : MonoBehaviour {
	
	// the seemingly infinite grid we want to resize dynamically during gameplay
	public GFGrid myGrid;
	
	//how much buffer do we want to use? More buffer means more to render each time, but less frequent recalculations
	public float buffer = 10.0f;
	
	// speed of the camera, boost factor and a boost key to hold down
	public float cameraSpeed = 15.0f;
	public float speedBoost = 2.0f;
	public string boostButton = "left shift";
	
	// use these internally to decide when to recalculate
	private Transform _transform;
	private Vector3 _lastPosition;
	private Camera _cam;
	
	void Awake () {
		// we will be referencing the transform every frame, so better cache it
		_transform = transform;
		_lastPosition = _transform.position;
		
		// the same applies to the camera
		_cam = GetComponent<Camera>();
		_cam.orthographic = true; // make sure it is orthographic (we will use an orthographic camera here because it is simpler)
		
		// set the grid's range according to the camera's size plus the buffer we want to use
		float x = _cam.aspect * _cam.orthographicSize + 1.1f * buffer; // I scale the buffer with 1.1 to make it a little bit larger, just in case
		float y = _cam.orthographicSize + 1.1f * buffer;
		myGrid.renderFrom = new Vector3(_transform.position.x - x, _transform.position.y - y, 0); // we set the range relative to the camera's position
		myGrid.renderTo = new Vector3(_transform.position.x + x, _transform.position.y + y, 0);
	}
	
	void Update () {
		HandleMovement(); // first move the grid
		
		// if we exceeded the buffer limit let's recalculate the points for rendering
		if (Mathf.Abs(_transform.position.x - _lastPosition.x) >= buffer || Mathf.Abs(_transform.position.y - _lastPosition.y) >= buffer)
			ResizeGrid();
	}
	
	void ResizeGrid () {
		Vector3 rangeShift = Vector3.zero; // we can't manipulate the individual components of renderFrom/To directly so use a temp variable
		for (int i = 0; i < 2; i++) {
			rangeShift[i] += _transform.position[i] - _lastPosition[i]; // use the difference in positions for the shift
		}
		
		myGrid.renderFrom += rangeShift; // now add the shift to both values
		myGrid.renderTo += rangeShift;
		
		_lastPosition = _transform.position; // save this current position as the new last fixed point
	}
	
	// just a simple method for handling movement using the arrow keys (hold boostButton to move faster)
	void HandleMovement(){
		_transform.position += new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0) * cameraSpeed * (Input.GetKey(boostButton) ? speedBoost : 1 ) * Time.deltaTime;
	}
	
	// display some information
	void OnGUI () {
		GUI.TextArea(new Rect (10, 10, 600, 150), "Use arrow keys to scroll the camera, hold shift to scroll faster. " +
			"The current camera position is\n\t" + _transform.position.x +" / "+_transform.position.y + "\n and the last fixed position was\n\t" + _lastPosition.x +" / "+ _lastPosition.y +
			"\n\vThe grid's rendering range is only adjusted when the camera reaches the edge of the grid, and that prompts the grid to re-calculate its points. This gives us the illusion" +
			" of a seemingly endless grid while we only render what's within close reach." +
			"This is cheaper performance-wise than rendering a huge grid when the player will only see a small part of it at any given time.");
	}
}