#pragma strict
//make a bouning grid and render it using Vectrosity. The bouncing is handled by the physics engine

// UNCOMMENT THIS SCRIPT FOR IT TO WORK

/*
public var lineWidth: float = 7.0;
public var lineMaterial: Material;

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
	
	gridLine = new Vectrosity.VectorLine("Bouncy Lines", grid.GetVectrosityPoints(), Color.red, lineMaterial, lineWidth);
	gridLine.Draw3DAuto(cachedTransform);
	
	cachedTransform.position = tempPos;
	cachedTransform.rotation = Random.rotation; //apply an initial random rotation
}
*/