using UnityEngine;
using UnityEditor; // for EditorApplication.isPlaying
using System.Collections;
using System.IO; //needed for the StringReader class

[RequireComponent(typeof(GFRectGrid))]
public class TerrainMeshBuilder : MonoBehaviour {
	
	// a plain text file with (perferably) integer numbers for the height
	public TextAsset heightMap;
	
	// the amount of rows (X-direction towards positive) and columns (Z-direction towards negative), will be calculated from the text
	private int rows = 0, columns = 0;
	// after the heights have been read form the height map store them in this integer array
	private int[] heights;
	
	// the mesh used for terrain, we will be referencing it often
	private Mesh mesh;
	// internal variables for reference
	private MeshFilter mf;
	private MeshCollider mc;
	private MeshRenderer mr;
	private GFRectGrid grid;
	
	// used for the wireframe rendering
	Material lineMaterial;

	void Awake () {
		if (!heightMap)
			return;

		// make sure the needed components exist and store them; CheckComponent<T>() is defined in this script near the bottom
		mf = CheckComponent<MeshFilter>();
		mc = CheckComponent<MeshCollider>();
		mr = CheckComponent<MeshRenderer>();
		grid = GetComponent<GFRectGrid> ();
		
		// create the mesh
		BuildMesh ();
		// then assign it to the components that need it
		AssignMesh ();
		// show the height map in the GUI
		UpdateHeightString ();
		
		// the material used for the wireframe lines
		lineMaterial = new Material(planeShader);
	}
	
	void BuildMesh () {
		// this will be parsing our text file
		StringReader sr = new StringReader(heightMap.text);
		// line stores the text of the current line that's being read, text stores the concatenation of all lines without line breaks 
		string line, text = "";
		
		// instantiate a new mesh and wipe it clean
		mesh = new Mesh();
		mesh.Clear();
		
		// let's first get the amount of rows and columns
		while ((line = sr.ReadLine()) != null) {
			rows++; // count the amount of rows so far
			columns = Mathf.Max(columns, line.Length); // the longest line is the amount of columns
		}
		// build the height array from that
		heights = new int[rows * columns];
		
		// now concatenate all lines into one large string.
		sr = new StringReader(heightMap.text); // reset the reader first
		while ((line = sr.ReadLine()) != null) {
			// Fill short lines with extra 0 to make them all have the same length
			for (int i = line.Length; i < columns; i++)
				line += "0";
			text += line; // concat all the lines into one large string (strips out the line breaks)
		}
		
		//copy the text into the heights array
		for (int i = 0; i < text.Length; i++)
			heights[i] = (int)char.GetNumericValue(text[i]);
		
		//this is where we will store the vertices
		Vector3[] vertices = new Vector3[rows * columns];
		
		for (int i = 1; i <= rows; i++){ // traverse the row first
			for (int j = 1; j <= columns; j++) { // then the column
				// pick the height from text. The row and column get translated into the index within the long text string
				int h = heights[RowColumn2Index(i, j)];
				// now assign the vertex depending on its row, column and height (vector in local space; row, column and height in grid space)
				vertices[RowColumn2Index(i, j)] = transform.InverseTransformPoint(grid.GridToWorld(new Vector3(j - 1, h, -i + 1)));
			}
		}
		// assign the vertices to the mesh
		mesh.vertices = vertices;
		
		// now for the triangles; 3 vertices per triangle, two triangles per square, one square between two columns/rows
		int[] triangles = new int[3 * 2 * (rows - 1) * (columns - 1)];
		int counter = 0; // this will keep track of where in the triangles array we currently are
		
		for (int i = 1; i < rows; i++) {
			for (int j = 1; j < columns; j++){
				// assign the vertex indices in a clockwise direction
				triangles[counter] = RowColumn2Index(i, j);
				triangles[counter + 1] = RowColumn2Index(i, j + 1);
				triangles[counter + 2] = RowColumn2Index(i + 1, j);
				triangles[counter + 3] = RowColumn2Index(i + 1, j);
				triangles[counter + 4] = RowColumn2Index(i, j + 1);
				triangles[counter + 5] = RowColumn2Index(i + 1, j + 1);
				counter += 6; // incerement the counter for the next two triangles (six vertices)
			}
		}
		// assign the triangles to the mesh
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
	}
	
	// the index is calculated by going through each row until the end and then jumping to the beginning of the next row (rows and colums start at 1)
	int RowColumn2Index (int row, int column) {
		return (row - 1) * columns + column - 1;
	}
	
	void AssignMesh () {
		mf.mesh = mesh;
		mc.sharedMesh = mesh;
		// add a nice transparent shader to the renderer
		mr.sharedMaterial = new Material(lineShader); // lineShader is defined below in the Wireframe Rendering region
	}
	
	// this is called every frame the mouse is hovering above the collider
	void OnMouseOver () {
		int index = HandleMouseInput(); // the index of the neareast vertex we are above (in a real game you would not want to call this every frame)
		if(Input.GetMouseButtonDown(0)) { // when the player left-clicks
			RaiseVertex(index, 1);
		} else if (Input.GetMouseButtonDown(1)) { // when the player right-clicks
			RaiseVertex(index, -1);
		}
	}

	//handle mouse input by casting a ray from the cursor through the camera at the mesh collider and returning the index of the nearest vertex
	int HandleMouseInput () {
		RaycastHit hit;
		mc.Raycast(Camera.main.ScreenPointToRay (Input.mousePosition), out hit, Mathf.Infinity);
		cursorPoint = hit.point; // used for the green gizmo, see under Gizmos
		Vector3 gp = grid.WorldToGrid(hit.point); // the point that was hit in grid coordinates
		return RowColumn2Index(Mathf.RoundToInt(-gp.z) + 1, Mathf.RoundToInt(gp.x) + 1); // remember, the index is related to the grid coordinates
	}
	
	// raise (or lower) a vertex with given index by a certain amount along the Y-axis
	void RaiseVertex (int index, int amount) {
		Vector3[] vertices = mesh.vertices; // extract the vertices of the mesh
		vertices[index] += new Vector3(0, amount * grid.spacing.y, 0); // then raise the vertex
		mesh.vertices = vertices; // assign the vertices back

		mesh.RecalculateNormals(); // do the cleanup again
		mesh.RecalculateBounds();
		mesh.Optimize();

		mc.sharedMesh = null; // to update the mesh collider remove its mesh
		mc.sharedMesh = mesh; // and then re-assign it again
		
		heights[index] += amount; // update the heights index (to make sure the height map in the GUI is up to date)
		UpdateHeightString (); // update the heightmap in the GUI using the updated heights array
	}
	
	// this handy method returns a component of given type and if there is none it creates one for you.
	private T CheckComponent<T>() where T: Component {
		T component = GetComponent<T>();
		if (!component)
			component = gameObject.AddComponent<T>();
		return component;
	}

	#region Gizmos
	private Vector3 cursorPoint; // a small helper that will draw a sphere where the ray hits the plane

	#if UNITY_EDITOR // EditorApplication is only defined when running in the Unity editor, otherwise it won't compile
	void OnDrawGizmos () {
		if (!EditorApplication.isPlaying)
			return;
		Gizmos.color = Color.green;
		Gizmos.DrawSphere(cursorPoint, 0.3f);
	}
	#endif // UNITY_EDITOR
	#endregion
	
	#region Wireframe Rendering
	string planeShader = "Shader \"Lines/Colored Blended\" { SubShader { Pass { Blend SrcAlpha OneMinusSrcAlpha BindChannels { Bind \"Color\",color } ZWrite On Cull Front Fog { Mode Off } } } }";
	string lineShader = "Shader \"Alpha Additive\" {" +
            "Properties { _Color (\"Main Color\", Color) = (1,1,1,0) }" +
            "SubShader {" +
            "    Tags { \"Queue\" = \"Transparent\" }" +
            "    Pass {" +
            "        Blend One One ZWrite Off ColorMask RGB" +
            "        Material { Diffuse [_Color] Ambient [_Color] }" +
            "        Lighting On" +
            "        SetTexture [_Dummy] { combine primary double, primary }" +
            "    }" +
            "}" +
            "}";
	
	// this is not necessarily the best way to achieve wireframe rendering, but it gets the job done. For a real game you should look for a better solution
	void OnRenderObject () {
		lineMaterial.SetPass(0);
		GL.PushMatrix();
		GL.MultMatrix(transform.localToWorldMatrix); 
		GL.Begin(GL.LINES);
		
		GL.Color(Color.black);
		for (int i = 0; i < mesh.triangles.Length / 3; i++) {
			GL.Vertex(mesh.vertices[mesh.triangles[i * 3]]);
			GL.Vertex(mesh.vertices[mesh.triangles[i * 3 + 1]]);
			
			GL.Vertex(mesh.vertices[mesh.triangles[i * 3 + 1]]);
			GL.Vertex(mesh.vertices[mesh.triangles[i * 3 + 2]]);
			
			GL.Vertex(mesh.vertices[mesh.triangles[i * 3 + 2]]);
			GL.Vertex(mesh.vertices[mesh.triangles[i * 3]]);
		}
		
		GL.End();
		GL.PopMatrix();
	}
	#endregion
	
	#region GUI message
	private string guiMessage = "Left-click a vertex to raise it by one unit, right-click to lower it. Turn on gizmos to see where on the grid the cursor is pointing at.";
	string heightString;
	
	void UpdateHeightString () {
		heightString = "Height Map:\n"; // boilerplate text
		for (int i = 1; i <= rows; i++) {
			for (int j = 1; j <= columns; j++) {
				heightString += " " + heights[RowColumn2Index(i, j)].ToString() + " "; // add the entries from the heights array
			}
			heightString += "\n"; // line break after reaching the end of the row
		}
	}

	void OnGUI () {
		GUI.TextArea (new Rect (10, 10, 400, 100), guiMessage);
		GUI.TextArea (new Rect (10, Screen.height - 10 - 200, 400, 200), heightString);
	}
	#endregion
}
