#pragma strict
import System.Collections.Generic; //needed for the generic list class
import System.IO; //needed for the StringReader class

@script RequireComponent(GFRectGrid)

public var levelData: TextAsset[]; //an array of text files to be read

//prefabs for our objects
public var red: GameObject;
public var green: GameObject;
public var blue: GameObject;

private var levelGrid: GFGrid; //the grid we place blocks on
private var reader: StringReader = null; //this object is what reads the text file
//private var levelFile: FileInfo = null; //this object would be needed if you wanted to read files not included with the game (mods, DLC)

private var currentLevel: int = 0; //which level from the levels array to load
private var blocks: List.<GameObject>;	//in order to delete all the blocks we need to keep track of them
private var shift: float; //the shift from the coordinate we get (depends on the type of grid)

enum ButtonPosition {upLeft, downLeft};
public var buttonPosition: ButtonPosition;

function Awake(){
	levelGrid = GetComponent.<GFRectGrid>();
	blocks = new List.<GameObject>();
	// if and how much we need to shift the objects depends on the type of grid
	shift = levelGrid.GetType() == typeof(GFRectGrid) ? 0.5f : 0;
	BuildLevel(levelData[currentLevel], levelGrid);
}

function BuildLevel(levelData: TextAsset, levelGrid: GFGrid){
	//abort if there are no prefabs to instantiate
	if(!red || !green || !blue)
		return;
	
	//loop though the list of old blocks and destroy all of them, we don't want the new level on top of the old one
	for(var go: GameObject in blocks){
		if(go)
			Destroy(go);
	}
	//destroying the blocks doesn't remove the reference to them in the list, so clear the list
	blocks.Clear();
	
	//setup the reader, a variable for storing the read line and keep track of the number of the row we just read
	reader = new StringReader(levelData.text);
	var line: String = reader.ReadLine();
	var row: int = 0;
	
	//read the text file line by line as long as there are lines
	while(line != null){
		//line = reader.ReadLine();
		//read each line character by character
		for(var i: int = 0; i < line.Length; i++){
			//first set the target position based on where in the text file we are, then place a block there
			var targetPosition: Vector3 = levelGrid.GridToWorld(Vector3(i + shift, -row - shift, 0)); //offset by 0.5
			CreateBlock(line[i], targetPosition);
		}
		//we read a row, now it's time to read the next one; increment the counter and then read it
		row++;
		line = reader.ReadLine();
	}
}

function CreateBlock(letter: char, targetPosition: Vector3){
	var spawn: GameObject = null;
	//set the value of cube based on the supplied character
	switch(letter){
		case 'R':
			spawn = red;
			break;
		case 'G':
			spawn = green;
			break;
		case 'B':
			spawn = blue;
			break;
		default: //if the character is neither R, nor G, nor C we don't place any cube
			break;
	}
	//instantiate the cube if one was picked, else don't do anything
	if(spawn){
		var obj: GameObject = Instantiate(spawn, targetPosition, Quaternion.identity) as GameObject;
		//add that cube into our list of blocks
		blocks.Add(obj);
	}
}

//this function creates a GUI button that lets you switch levels
function OnGUI(){
	var top: float = buttonPosition == ButtonPosition.upLeft ? 10 : Screen.height - 50 -10;
	if(GUI.Button(Rect(10, top, 150, 50), "Try Another Level")){
		//increment the level counter; using % makes the number revert back to 0 once we have reached the limit
		currentLevel = (currentLevel + 1) % levelData.Length;
		//now build the level (BuildLevel uses the blocks variable to find and destroy any previous blocks)
		BuildLevel(levelData[currentLevel], levelGrid);
	}
}