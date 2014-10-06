using UnityEngine;
using System.Collections;

// this is a simple script to generate a square mesh to hold the (sprite) image of the dial

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
[RequireComponent(typeof(GFPolarGrid))]
public class RotaryDialSprite : MonoBehaviour {

	void Awake () {
		float size = GetComponent<GFPolarGrid> ().size.x; // this the radius of the grid drawing
		Mesh mesh = BuildMesh (size);
		GetComponent<MeshFilter> ().mesh = mesh;
		GetComponent<MeshCollider> ().sharedMesh = mesh;
	}

	Mesh BuildMesh (float size = 1.0f) {
		Mesh mesh = new Mesh ();
		mesh.vertices = new Vector3[4] {
			new Vector3 (-size, -size, 0), // bottom left
			new Vector3 (-size, size, 0), // top left
			new Vector3 (size, size, 0), // top right 
			new Vector3 (size, -size, 0) // bottom right
		};
		mesh.triangles = new int[6] { 0, 1, 2, 0, 2, 3 };
		mesh.uv = new Vector2[] {new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 1), new Vector2(1, 0)};

		mesh.Optimize();
		return mesh;
	}
}
