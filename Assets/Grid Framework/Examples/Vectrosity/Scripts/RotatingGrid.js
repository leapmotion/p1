#pragma strict
//make a grid rotate and render it using Vectrosity

// UNCOMMENT THIS SCRIPT FOR IT TO WORK


public var lineWidth: float = 7.0;
public var lineMaterial: Material;
/*
private var grid: GFGrid;
private var gridLine: Vectrosity.VectorLine;
private var cachedTransform: Transform;

function Start () {
	cachedTransform = transform;
	// in order for the rendering to align properly with the grid the grid has to be at the world's origin
	var tempPos: Vector3 = cachedTransform.position;
	cachedTransform.position = Vector3.zero;
	grid = GetComponent.<GFGrid>();

	if(lineWidth < 1.0) lineWidth = 1.0;
	
	gridLine = new Vectrosity.VectorLine("Rotating Lines", grid.GetVectrosityPoints(), Color.green, lineMaterial, lineWidth);
	gridLine.Draw3DAuto(cachedTransform);
	
	cachedTransform.position = tempPos;
}

function Update () {
	//rotate the grid
	cachedTransform.Rotate(10*Vector3.right * Time.deltaTime);
	cachedTransform.Rotate(15*Vector3.up * Time.deltaTime, Space.World);
}
*/