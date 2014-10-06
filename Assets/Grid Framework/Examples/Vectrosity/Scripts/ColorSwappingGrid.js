#pragma strict
//import Vectrosity;

// UNCOMMENT THIS SCRIPT FOR IT TO WORK


public var lineWidth: float = 10.0;
public var lineMaterial: Material;
/*
private var grid: GFGrid;
private var gridLine: Vectrosity.VectorLine;
private var cachedTransform: Transform;

//the array of possible colours
private var colors: Color[];
//the array of assigned colours
private var lineColors: Color[];

private var iterator: int = 0; //for looping though the arrays
private var doChangeColor: boolean = true;

function Start () {
	cachedTransform = transform;
	var tempPos: Vector3 = cachedTransform.position;
	// in order for the rendering to align properly with the grid the grid has to be at the world's origin
	cachedTransform.position = Vector3.zero;
	
	grid = GetComponent.<GFGrid>();
	if(lineWidth < 1.0) lineWidth = 1.0;
	
	//list possible colous and then assign them to the line segments
	colors = new Color[7];
	colors[0]=Color.white; colors[1]=Color.red; colors[2]=Color.green; colors[3]=Color.blue; colors[4]=Color.yellow; colors[5]=Color.cyan; colors[6]=Color.magenta;
	lineColors = new Color[grid.GetVectrosityPoints().Length / 2];
	for(var i: int = 0; i < lineColors.Length; i++){
		//i % colors.Length returns always a number between 0 and the amout of colous we have listed. it increments every time and when the maximum
		// has been reached it reverts back to zero
		lineColors[i] = colors[i % colors.Length];
	}
	
	gridLine = new Vectrosity.VectorLine("Rotating Lines", grid.GetVectrosityPoints(), lineColors, lineMaterial, lineWidth);
	
	gridLine.Draw3DAuto(cachedTransform);
	cachedTransform.position = tempPos;
}

function delayChanging(){
	// wait a while before allowing to change colours again
	yield WaitForSeconds(Random.Range(0.2, 1.0));
	doChangeColor = true;
}

function Update () {
	delayChanging();
	if(doChangeColor){
		//pick a random colour for the current line and aply it
		lineColors[iterator] = colors[Random.Range(0, 7)];
		gridLine.SetColors(lineColors);
	
		iterator++; //next line
		iterator = iterator % lineColors.Length;// 0 -> 1 -> 2 -> 3 -> 4 -> 5 -> 6 -> 0 -> 1 -> 2 -> ...
		doChangeColor = false;
	}
	//rotate the grid
	cachedTransform.Rotate(-15*Vector3.right * Time.deltaTime);
	cachedTransform.Rotate(10*Vector3.up * Time.deltaTime, Space.World);
}
*/