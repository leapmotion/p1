using UnityEngine;
using System.Collections;

public class SlidingPuzzleGrid : MonoBehaviour {

	string guiMessage = "Unit's built-in physics system is great for 3D games with realistic behaviour, but sometimes you need more basic predictable " +
		"and 'video-gamey' behaviour. This example doesn't use physics at all, instead it keeps track of which squares are occupied and which are free, " +
		"then is restricts movement accordingly by clamping the position vectors.";

	// Awake is being called before Start; this makes sure we have a matrix to begin with when we add the blocks
	void Awake() {
		// because of how we wrote the accessor this will also immediately build the matrix of our level
		SlidingPuzzleExample.mainGrid = gameObject.GetComponent<GFRectGrid>();
	}
	
	// visualizes the matrix in text form to let you see what's going on
	void OnGUI(){
		GUI.TextArea (new Rect (10, 10, 400, 100), guiMessage);
		GUI.TextArea (new Rect (10, Screen.height - 10 - 150, 250, 150), SlidingPuzzleExample.MatrixToString());
	}
}