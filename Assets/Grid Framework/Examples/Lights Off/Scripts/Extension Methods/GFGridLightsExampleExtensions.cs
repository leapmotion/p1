using UnityEngine;
using System.Collections;

/*
	ABOUT THIS SCRIPT
	
This script creates an extension method for the GFGrid class, which means it adds a new
method to the class without the need to alter the source code of the class. Once declared
you can use the method just like any normal class method.

One of the design principles behind Grid Framework says that the GFGrid class and all its
methods are abstract and that the exact implementation lies in the sublasses. However, since
extensions methods are static they cannot be abstract, it's simply part of the language
design of C#, so I had to do a workaround. The method performs some preperation and then
it picks the implementation based on the exact type of grid. Note that those functions are
not extension methods, they are just regular methods of this static class.

*/

public static class GFGridLightsExampleExtensions{

	public static bool IsAdjacent(this GFGrid theGrid, Vector3 position, Vector3 reference){
		//convert to Grid Space first
		Vector3 gridPosition = theGrid.WorldToGrid(position);//the light we want to test
		Vector3 gridReference = theGrid.WorldToGrid(reference);//the light that was pressed
		
		//pick the implentation based on the type of grid
		if (theGrid.GetType() == typeof(GFRectGrid)) {
			return RectIsAdjacent((GFRectGrid)theGrid, gridPosition, gridReference);
		} else if (theGrid.GetType() == typeof(GFHexGrid)) {
			return HexIsAdjacent((GFHexGrid)theGrid, gridPosition, gridReference);
		} else if (theGrid.GetType() == typeof(GFPolarGrid)) {
			return PolarIsAdjacent((GFPolarGrid)theGrid, gridPosition, gridReference);
		} else {
			return false;
		}
	}
	
	private static bool RectIsAdjacent(GFRectGrid theGrid, Vector3 position, Vector3 reference){
		bool isAdjacent = Mathf.Abs(position.x-reference.x) <= 1.25f && Mathf.Abs(position.y-reference.y) <= 1.25f;
		bool isDiagonal = 0.25f <= Mathf.Abs(position.x-reference.x) && 0.25f <= Mathf.Abs(position.y-reference.y) && isAdjacent;
		return isAdjacent && !isDiagonal;
	}
	
	private static bool HexIsAdjacent(GFHexGrid theGrid, Vector3 position, Vector3 reference){
		bool isNeighbour = false;
		if(Mathf.Abs(reference.y - position.y) < 1.1f && Mathf.Abs(reference.x - position.x) < 0.1f){//straight above or below
			isNeighbour = true;
		} else{
			if(Mathf.RoundToInt(reference.x) % 2 == 0){//two cases, depending on whether the x-coordinate is even or odd
				//neighbours are either strictly left or right of the switch or right/left and one unit below
				if(Mathf.Abs(reference.x - position.x) < 1.1f && position.y - reference.y < 0.1f && position.y - reference.y > -1.1f)
					isNeighbour = true;
			} else{//x-coordinate odd
				//neighbours are either strictly left or right of the switch or right/left and one unit above
				if(Mathf.Abs(reference.x - position.x) < 1.1f && reference.y - position.y < 0.1f && position.y - reference.y < 1.1f)
					isNeighbour = true;
			}
		}
		return isNeighbour;
	}
	
	// this one is very similar to the rectangular case, expect for one difference
	private static bool PolarIsAdjacent (GFPolarGrid theGrid, Vector3 position, Vector3 reference) {
		// note the last condition: if the last and first tile in a circle are adjacent as well, this simulates the wrapping.
		bool isAdjacent = Mathf.Abs(position.x-reference.x) <= 1.25f && (Mathf.Abs(position.y-reference.y) <= 1.25f || Mathf.Abs(position.y-reference.y) > theGrid.sectors - 1.25f);
		bool isDiagonal = 0.25f <= Mathf.Abs(position.x-reference.x) && 0.25f <= Mathf.Abs(position.y-reference.y) && isAdjacent;
		return isAdjacent && !isDiagonal;
	}
}