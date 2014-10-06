#pragma strict

//Make a resizing grid and render it using Vectrosity. In reality you shouldn't call Resize every frame
// for performance reasons.

// UNCOMMENT THIS SCRIPT FOR IT TO WORK
/*

public var lineWidth: float = 7.0;
public var lineMaterial: Material;

private var grid: GFRectGrid;
private var gridLine: Vectrosity.VectorLine;
private var cachedTransform: Transform;
private var tempPos: Vector3;

//some toggling variables for resizing
private var growingSpacingX = true;
private var growingSizeX = true;
private var growingSpacingY = true;
private var growingSizeY = true;

function Start () {
	cachedTransform = transform;
	// in order for the rendering to align properly with the grid the grid has to be at the world's origin
	tempPos = cachedTransform.position;
	cachedTransform.position = Vector3.zero;
	grid = GetComponent.<GFRectGrid>();
	
	if(lineWidth < 1.0) lineWidth = 1.0;
	
	gridLine = new Vectrosity.VectorLine("Resizing Lines", grid.GetVectrosityPoints(), Color.yellow, lineMaterial, lineWidth);
	gridLine.Draw3DAuto(cachedTransform);
	
	cachedTransform.position = tempPos;
}

function Update () {
	resizeGrid();
	
	// in order for the rendering to align properly with the grid the grid has to be at the world's origin
	tempPos = cachedTransform.position;
	cachedTransform.position = Vector3.zero;
	gridLine.Resize(grid.GetVectrosityPoints()); //calculate the new grid points
	cachedTransform.position = tempPos;
}

function resizeGrid(){
	if(growingSpacingX){
		grid.spacing.x = Mathf.MoveTowards(grid.spacing.x, 3.0, Random.Range(0.25, 0.5)*Time.deltaTime);
		if(Mathf.Abs(grid.spacing.x - 3.0) < 0.01)
			growingSpacingX = false;
	} else{
		grid.spacing.x = Mathf.MoveTowards(grid.spacing.x, 2.0, Random.Range(0.25, 0.5)*Time.deltaTime);
		if(Mathf.Abs(grid.spacing.x - 2.0) < 0.01)
			growingSpacingX = true;
	}
	
	if(growingSizeX){
		grid.size.x = Mathf.MoveTowards(grid.size.x, 15.0, Random.Range(2.0, 3.0)*Time.deltaTime);
		if(Mathf.Abs(grid.size.x - 15.0) < 0.01)
			growingSizeX = false;
	} else{
		grid.size.x = Mathf.MoveTowards(grid.size.x, 10.0, Random.Range(1.0, 3.0)*Time.deltaTime);
		if(Mathf.Abs(grid.size.x - 10.0) < 0.01)
			growingSizeX = true;
	}
	
	if(growingSpacingY){
		grid.spacing.y = Mathf.MoveTowards(grid.spacing.y, 2.0, Random.Range(0.25, 0.5)*Time.deltaTime);
		if(Mathf.Abs(grid.spacing.y - 2.0) < 0.01)
			growingSpacingY = false;
	} else{
		grid.spacing.y = Mathf.MoveTowards(grid.spacing.y, 1.0, Random.Range(0.25, 0.5)*Time.deltaTime);
		if(Mathf.Abs(grid.spacing.y - 1.0) < 0.01)
			growingSpacingY = true;
	}
	
	if(growingSizeY){
		grid.size.y = Mathf.MoveTowards(grid.size.y, 13.0, Random.Range(1.0, 2.0)*Time.deltaTime);
		if(Mathf.Abs(grid.size.y - 13.0) < 0.01)
			growingSizeY = false;
	} else{
		grid.size.y = Mathf.MoveTowards(grid.size.y, 10.0, Random.Range(1.0, 2.0)*Time.deltaTime);
		if(Mathf.Abs(grid.size.y - 10.0) < 0.01)
			growingSizeY = true;
	}
}

function PingPongValue(current: float, from: float, to: float, forward: boolean){

}
*/