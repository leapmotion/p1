using UnityEngine;
using System.Collections;
using System.Collections.Generic; //needed for the generic list class
using System.IO; //needed for the StringReader class

[RequireComponent(typeof(GFGrid))]

public class LevelTextParser : MonoBehaviour {
	public TextAsset[] levelData; //an array of text files to be read
		
	//prefabs for our objects
	public GameObject red;
	public GameObject green;
	public GameObject blue;
	
	private GFGrid levelGrid; //the grid we place blocks on
	private StringReader reader = null; //this object is what reads the text file
	//FileInfo levelFile = null; //this object would be needed if you wanted to read files not included with the game (mods, DLC)
	
	private int currentLevel = 0; //which level from the levels array to load
	private List<GameObject> blocks;	//in order to delete all the blocks we need to keep track of them
	private float shift; //the shift from the coordinate we get (depends on the type of grid)
	
	public enum ButtonPosition {upLeft, downLeft, downRight};
	public ButtonPosition buttonPosition;
	
	public void Awake(){
		levelGrid = GetComponent<GFGrid>();
		blocks = new List<GameObject>();
		// if and how much we need to shift the objects depends on the type of grid
		shift = levelGrid.GetType() == typeof(GFRectGrid) ? 0.5f : 0;
		BuildLevel(levelData[currentLevel], levelGrid);
	}

	public void BuildLevel(TextAsset levelData, GFGrid levelGrid){
		//abort if there are no prefabs to instantiate
		if(!red || !green || !blue)
			return;
		
		//loop though the list of old blocks and destroy all of them, we don't want the new level on top of the old one
		foreach(GameObject go in blocks){
			if(go)
				Destroy(go);
		}
		//destroying the blocks doesn't remove the reference to them in the list, so clear the list
		blocks.Clear();
		
		//setup the reader, a variable for storing the read line and keep track of the number of the row we just read
		reader = new StringReader(levelData.text);
		string line;
		int row = 0;
		
		//read the text file line by line as long as there are lines
		while((line = reader.ReadLine()) != null){
			//read each line character by character
			for(int i = 0; i < line.Length; i++){
				//first set the target position based on where in the text file we are, then place a block there (add 1 for polar grids because we don't want the origin)
				Vector3 targetPosition = levelGrid.GridToWorld(new Vector3(i + shift + (levelGrid.GetType() == typeof(GFPolarGrid) ? 1 : 0), -row - shift, 0)); //offset by 0.5
				CreateBlock(line[i], targetPosition);
			}
			//we read a row, now it's time to read the next one; increment the counter
			row++;
		}
	}
	
	void CreateBlock(char letter, Vector3 targetPosition){
		GameObject spawn = null;
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
			GameObject obj = Instantiate(spawn, targetPosition, Quaternion.identity) as GameObject;
			//add that cube into our list of blocks
			blocks.Add(obj);
		}
	}
	
	//this function creates a GUI button that lets you switch levels
	void OnGUI(){
		float top = buttonPosition == ButtonPosition.upLeft ? 0 : Screen.height - 50;
		float left = buttonPosition == ButtonPosition.downRight ? Screen.width - 170 : 0;
		if(GUI.Button(new Rect(left + 10, top - 10, 150, 50), "Try Another Level")){
			//increment the level counter; using % makes the number revert back to 0 once we have reached the limit
			currentLevel = (currentLevel + 1) % levelData.Length;
			//now build the level (BuildLevel uses the blocks variable to find and destroy any previous blocks)
			BuildLevel(levelData[currentLevel], levelGrid);
		}
	}
}