using UnityEngine;
using System.Collections;

/*
	ABOUT THIS SCRIPT
	
This script automates the task of placing tiles for the lights-out game
manually. it will loop in a circular fasion through the grid and place tiles
based on ther grid coordinates. Once a tile object has been pu it place
the appropriate components are dded and set up.

For each tile we generate a custom mesh based on the tile's position in
the grid. Every tile has a unique shape, but we can atomate it as well by
using the tile#s grid coordinates to calculate the vertices and onnect them
to triangles. The same mesh is then used for collision as well.

This script demonstrates how to use Grid Framework both for object placement
and for mesh generation. The generated meshed fit together nicely and appear
as one seamless object, but they stay separate.
*/

[RequireComponent (typeof (GFPolarGrid))]
public class ConstructPolarBlocks : MonoBehaviour {
	
	public Material blockMaterialDark; // dark and light materials for the lights
	public Material blockMaterialLight;

	private int layers; // how many loops around the circle
	private GFPolarGrid grid; // the grid used to place the tiles on
	private Transform _transform; // we will do quite a lot of loops, so cache this
	
	public void Awake () {
		// cache components
		grid = GetComponent<GFPolarGrid>();
		_transform = transform;
		layers = Mathf.FloorToInt(grid.size.x / grid.radius);
		
		for (int i = 0; i < layers; i++) { // loop throught the layers
			for (int j = 0; j < grid.sectors; j++) { // loop through the sectorsof each layer
				GameObject go = BuildObject(i, j); // create the object at the current cell's centre
				SetComponents(go, i, j); // Set up its components
			}
		}
	}
	
	private GameObject BuildObject (int i, int j) {
		// instantiate the object and give it a fitting name
		GameObject go = new GameObject();
		go.name = "polarBlock_" + i + "_" + j;
		
		// position it at the cel's centre and make it a child of the grid (just for cleanliness, no special reason)
		go.transform.position = grid.GridToWorld(new Vector3(i + 0.5f, j + 0.5f, 0));
		go.transform.parent = _transform;
		
		return go;
	}
	
	private void SetComponents (GameObject go, int i, int j) {
		// add a mesh filter, a mesh renderer, a mesh collider and then construct the mesh (also pass the iteration steps for reference)
		BuildMesh(go.AddComponent<MeshFilter>(), go.AddComponent<MeshRenderer>(), go.AddComponent<MeshCollider>(), i, j);
		
		// add the script for lights and set up the variables
		LightsBehaviour lb = go.AddComponent<LightsBehaviour>();
		lb.onMaterial = blockMaterialLight;
		lb.offMaterial = blockMaterialDark;
		lb.connectedGrid = grid;
		// perform a light switch back and forth so the object picks up the newly assigned materials (kind of messy, but harmless enough)
		lb.SwitchLights();
		lb.SwitchLights();
	}
	
	private void BuildMesh (MeshFilter mf, MeshRenderer mr, MeshCollider mc, int i, int j) {
		Mesh mesh = new Mesh();
		mesh.Clear();
		
		// the vertices of our new mesh, separated into two groups
		Vector3[] inner = new Vector3[grid.smoothness + 1]; // the inner vertices (closer to the centre)
		Vector3[] outer = new Vector3[grid.smoothness + 1]; // the outer vertices

		// vertices must be given in local space
		Transform trnsfrm = mf.gameObject.transform;
		
		// the amount of vertices depends on how much the grid is smoothed
		for (int k = 0; k < grid.smoothness + 1; k++) {
			// rad is the current distance from the centre, sctr is the current sector and i * (1.0f / grid.smoothness) is the fraction inside the current sector
			inner[k] = trnsfrm.InverseTransformPoint(grid.GridToWorld(new Vector3(i, j + k * (1.0f / grid.smoothness), 0)));
			outer[k] = trnsfrm.InverseTransformPoint(grid.GridToWorld(new Vector3(i + 1, j + k * (1.0f / grid.smoothness), 0)));
		}
		
		//this is wher the actual vertices go
		Vector3[] vertices = new Vector3[2 * (grid.smoothness + 1)];
		// copy the sorted vertices into the new array
		inner.CopyTo(vertices, 0);
		// for each inner vertex its outer counterpart has the same index plus grid.smoothness + 1, this will be relevant later
		outer.CopyTo(vertices, grid.smoothness + 1);
		// assign them as the vertices of the mesh
		mesh.vertices = vertices;
		
		// now we have to assign the triangles
		int[] triangles = new int[6 * grid.smoothness]; // for each smoothing step we need two triangles and each triangle is three indices
		int counter = 0; // keeps track of the current index
		for (int k = 0; k < grid.smoothness; k++) {
			// triangles are assigned in a clockwise fashion
			triangles[counter] = k;
			triangles[counter+1] = k + (grid.smoothness + 1) + 1;
			triangles[counter+2] = k + (grid.smoothness + 1);
			
			triangles[counter+3] = k + 1;
			triangles[counter+4] = k + (grid.smoothness + 1) + 1;
			triangles[counter+5] = k;

			counter += 6; // increment the counter for the nex six indices
		}
		mesh.triangles = triangles;
		
		// add some dummy UVs to keep the shader happy or else it complains, but they are not used in this example
		Vector2[] uvs = new Vector2[vertices.Length];
        for (int k = 0; k < uvs.Length; k++) {
            uvs[k] = new Vector2(vertices[k].x, vertices[k].y);
        }
        mesh.uv = uvs;
		
		// the usual cleanup
		mesh.RecalculateNormals();
		mesh.RecalculateBounds();
		mesh.Optimize();
		
		// assign the mesh  to the mesh filter and mesh collider
		mf.mesh = mesh;
		mc.sharedMesh = mesh;
	}

	#region GUI Message
	string guiMessage = "Click a tile and all adjacent tiles swap their colour, the player's goal is to turn off all lights. " +
		"This example uses events and delegates to make all tiles compare their grid position to the clicked one's grid position to decide whether to swap colours. " +
		"The tiles themeselves don't know anything about the rest of the grid.\n\n" +
		"The tiles of the polar grid are using custom meshes constructed from code, rather than placing the tiles by hand.";

	void OnGUI () {
		GUI.TextArea (new Rect (Screen.width - 10 - 425, Screen.height - 10 - 130, 425, 130), guiMessage);
	}
	#endregion
}