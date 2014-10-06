using UnityEngine;
using System.Collections;

/*	ABOUT THIS SCRIPT

 For the sake of simplicity we will assume that the grid is always using a
 custom rendering range and starts at (0,0,0). If you want to see how to handle
 any grid please refer to the "movement with obstacles" example.
 
*/
using System.Diagnostics;

public static class SlidingPuzzleExample{
	
	private static GFRectGrid _grid; //private member variable for the grid

	///<summary>A public accessor for the sliding puzzle grid</summary>
	public static GFRectGrid mainGrid{
		get {return _grid;}
		set {
			_grid = value;
			BuildLevelMatrix(); //when we set a new grid we also immediately build a new matrix for it
		}
	}
	
	///<summary>This is where we store our information; all game logic uses this matrix.</summary>
	private static bool[,] levelMatrix;

	///<summary>Size of the level matrix, first component is horizontal, second vertical.</summary>
	private static int[] levelMatrixSize;
		
	///<summary>Takes the grid's rendering range and builds a matrix based on that. All entries are set to true.</summary>
	private static void BuildLevelMatrix() {
		int[] size = SetMatrixSize();             // amount of rows and columns, either based on size or rendering range (first entry rows, second one columns).
						
		levelMatrix = new bool[size[0], size[1]]; // Build a default matrix.

		for (int i = 0; i < size[0]; i++) {       // Set all entries to true.
			for (int j = 0; j < size[1]; j++) {
				levelMatrix[i,j] = true;          // all squares allowed initially
			}
		}
	}
	
	///<summary>How large the matrix should be. For the sake of simplicity we only use the rendering range here.</summary>
	private static int[] SetMatrixSize() {
		if (levelMatrixSize == null) // If there is no matrix yet create it, otherwise we can just overwrite its values.
			levelMatrixSize = new int[2];
		for (int i = 0; i < 2; i++) {
			//get the distance between both ends (in world units), divide it by the spacing (to get grid units) and round down to the nearest integer
			levelMatrixSize[i] = Mathf.FloorToInt(_grid.renderTo[i] / _grid.spacing[i]);
		}
		return levelMatrixSize;
	}
	
	///<summary>Takes world coodinates, finds the corresponding square and sets that entry to either true or false. Use it to disable or enable squares.</summary>
	public static void RegisterObstacle(Transform obstacle, bool state){
		Vector3[,] parts = BreakUpObstacle(obstacle); // First break up the obstacle into several 1x1 obstacles.
		
		for (int i = 0; i < parts.GetLength(0); i++) {  // Now find the square of each part and set it to true or false.
			for (int j = 0; j < parts.GetLength(1); j++) {
				int[] square = GetSquare(parts[i,j]);
				levelMatrix[square[0],square[1]] = state;
			}
		}
	}

	///<summary>Calculates the movement constrains for a block that's being dragged.</summary>
	/// When we want to slide a block we need to know how far we can go before we "collide" (note that there is no actual Unity collision detection involved anywhere).
	/// We can only look up to on square ahead in each direction, so the bounds need to be recalculated from time to time; this allows us to have obstacles in all
	/// sorts of directions, like a maze that can change all the time.
	public static Vector3[] CalculateSlidingBounds(Vector3 pos, Vector3 scl){
		//break up the block and find the lower left and upper right square in the matrix
		Vector3[,] squares    = BreakUpObstacle(pos, scl);
		int[]      lowerLeft  = GetSquare(squares[0,0]); // we store the position inside the matrix here
		int[]      upperRight = GetSquare(squares[squares.GetLength(0)-1,squares.GetLength(1)-1]);

		int left = lowerLeft[0], bottom = lowerLeft[1], right = upperRight[0], top = upperRight[1];

		// For each adjacent left square check if all left fields are free (a bitmask would have been the way to go instead of four bools, but let's keep it simple).
		bool freeEast = true;
		//iterate over all the squares one square left of the left edge
		for (int i = bottom; i <= top; i++) {
			freeEast = freeEast && levelMatrix[Mathf.Min(levelMatrix.GetLength(0)-1, right + 1), i]; // use Min so we don't go off the matrix size
		}

		bool freeWest = true;
		//iterate
		for( int i = bottom; i <= top; i++){
			freeWest = freeWest && levelMatrix[Mathf.Max(0, left - 1), i]; // use the Max so we don't get negative values (the matrix starts at 0)
		}
				
		bool freeSouth = true;
		for( int i = left; i <= right; i++){
			freeSouth = freeSouth && levelMatrix[i, Mathf.Max(0, bottom - 1)];
		}
		
		bool freeNorth = true;
		for (int i = left; i <= right; i++) {
			freeNorth = freeNorth && levelMatrix[i, Mathf.Min(levelMatrix.GetLength(1)-1, top + 1)];
		}

		//now assume the block canot move anywhere; for each free direction loosen the constraints by one grid unit (world unit * spacing)
		Vector3[] bounds = new Vector3[2] {pos, pos};
		if(freeEast)
			bounds[1] += _grid.spacing.x*Vector3.right;
		if(freeWest)
			bounds[0] -= _grid.spacing.x*Vector3.right;
		if(freeSouth)
			bounds[0] -= _grid.spacing.y*Vector3.up;
		if(freeNorth)
			bounds[1] += _grid.spacing.y*Vector3.up;
		
		// the bounds can still be outside of the grid, so we need to clamp that as well
		for (int i = 0; i < 2; i++) {
			for (int j = 0; j < 2; j++) {
				bounds[i][j] = Mathf.Clamp(bounds[i][j], _grid.GridToWorld(Vector3.zero)[j] + 0.5f * scl[j], _grid.GridToWorld(_grid.renderTo)[j] - 0.5f * scl[j]);
			}
		}
		
		return bounds;
	}

	public static void CalculateDiagonalBounds(Vector3 pos, Vector3 scl, ref bool[] bounds) {
		//break up the block and find the lower left and upper right square in the matrix
		Vector3[,] squares    = BreakUpObstacle(pos, scl);
		int[]      lowerLeft  = GetSquare(squares[0,0]); // we store the position inside the matrix here
		int[]      upperRight = GetSquare(squares[squares.GetLength(0)-1,squares.GetLength(1)-1]);
		
		int left = lowerLeft[0], bottom = lowerLeft[1], right = upperRight[0], top = upperRight[1];

		if (bounds == null)
			bounds = new bool[4];

		bounds[0] = top     < levelMatrixSize[1] - 1 && right < levelMatrixSize[0] - 1 && levelMatrix[right + 1, top + 1]     == false; // North-East
		bounds[1] = top     < levelMatrixSize[1] - 1 && left  > 0                      && levelMatrix[left  - 1, top + 1]     == false; // North-West
		bounds[2] = bottom  > 0                      && left  > 0                      && levelMatrix[left  - 1, bottom  - 1] == false; // South-West
		bounds[3] = bottom  > 0                      && right < levelMatrixSize[0] - 1 && levelMatrix[right + 1, bottom  - 1] == false; // South-East
	}
	
	///<summary>Break a large obstacle spanning several squares into several obstacles spanning one square each.</summary>
	public static Vector3[,] BreakUpObstacle(Vector3 pos, Vector3 scl){
		// first convert the scale to int and store X and Y values separate
		int[] obstacleScale = new int[2];
		for (int i = 0; i < 2; i++) {
			obstacleScale[i] = Mathf.Max(1, Mathf.CeilToInt(Mathf.RoundToInt(scl[i]) / Mathf.RoundToInt(_grid.spacing[i]))); //no lower than 1
		}
		
		// we will apply a shift so we always get the centre of the broken up parts, the shift depends on whether even or odd
		Vector3[] shift = new Vector3[2];
		for (int k = 0; k < 2; k++) {
			if (obstacleScale[k]%2 == 0) { // even
				shift[k] = (-obstacleScale[k] / 2.0f + 0.5f) * (k==0 ? Vector3.right : Vector3.up);
			} else { // odd
				shift[k] = (-(obstacleScale[k] - 1) / 2.0f) * (k==0 ? Vector3.right : Vector3.up);
			}
		}
		
		// this is where we store the single obstacles
		Vector3[,] obstacleMatrix = new Vector3[obstacleScale[0],obstacleScale[1]];
		
		// now break the obstacle up into squares and handle each square individually like an obstacle
		for (int i = 0; i < obstacleScale[0]; i++) {
			for (int j = 0; j < obstacleScale[1]; j++) {
				// what shift does is shift from the centre of the obstacle to the centre of the lower left sqare; from there we slowly increment over the remaining squares
				//obstacleMatrix[i,j] = pos + shift[0] + shift[1] + Vector3.Scale(new Vector3(i, j, 0), SlidingPuzzleExample.mainGrid.spacing); // <-- wrong?
				obstacleMatrix[i,j] = pos + shift[0] + shift[1] + new Vector3(i, j, 0); // <-- wrong?
			}
		}
		
		return obstacleMatrix;
	}
	
	///<summary>An alternative to the above that takes in a Transform as an argument.</summary>
	public static Vector3[,] BreakUpObstacle(Transform obstacle) {
		return BreakUpObstacle(obstacle.position, obstacle.lossyScale);
	}
	
	///<summary>Take world coodinates and find the corresponding square. The result is returned as an int array that contains that square's position in the matrix.</summary>
	private static int[] GetSquare(Vector3 vec) {
		int[] square = new int [2];
		for (int i = 0; i < 2; i++) {
			// remember, boxes dont have whole coordiantes, that's why we use a little shift to turn e.g. (3.5, 2.5, 1.5) into (3, 2, 1)
			square[i] = Mathf.RoundToInt((_grid.NearestBoxG(vec) - 0.5f * Vector3.one)[i]);
		}
		return square;
	}
	
	///<summary>This returns the matrix as a string so you can read it yourself, like in a GUI for debugging (nothing grid-related going on here, feel free to ignore it).</summary>
	public static string MatrixToString() {
		string text = "Occupied fields are 1, free fields are 0:\n\n";
		for (int j = levelMatrix.GetLength(1)-1; j >= 0; j--) {
			for (int i = 0; i < levelMatrix.GetLength(0); i++) {
				text = text + (levelMatrix[i,j] ? "0" : "1") + " ";
			}
			text += "\n";
		}
		return text;
	}
	
	// This method was used at some point but now it is of no use; I left it in though for you if you are interested
/*	//takes world coodinates, finds the corresponding square and returns the value of that square. Use it to cheack if a square is forbidden or not
	public static bool CheckObstacle(Transform obstacle){
		bool free = true; // assume it is allowed
		//first break up the obstacle into several 1x1 obstacles
		Vector3[,] parts = BreakUpObstacle(obstacle);
		//now find the square of each part and set it to true or false
		for(int i = 0; i < parts.GetUpperBound(0) + 1; i++){
			for(int j = 0; j < parts.GetUpperBound(1) + 1; j++){
				int[] square = GetSquare(parts[i,j]);
				free = free && levelMatrix[square[0],square[1]]; // add all the entries, returns true if and only if all are true
			}
		}
		return free;
	}
*/
}