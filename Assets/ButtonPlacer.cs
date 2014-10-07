using UnityEngine;
using System.Collections;
using SimpleJSON;

namespace P1
{
		public struct KeyDef
		{
				public string label;
				public int i;
				public int j;

				public KeyDef (string l, int ii, int jj)
				{
						label = l;
						i = ii;
						j = jj;
				}
		}

		public class ButtonPlacer : MonoBehaviour
		{

				public Vector3 buttonScale;
				public KeyDef[] keys = new KeyDef[]{
					new KeyDef ("0", 0, -2),
					new KeyDef ("1", 1, 1),
					new KeyDef ("2", 0, 1),
					new KeyDef ("3", -1, 1),
					new KeyDef ("4", 1, 0),
					new KeyDef ("5", 0, 0),
					new KeyDef ("6", -1, 0),
					new KeyDef ("7", 1, -1),
					new KeyDef ("8", 0, -1),
					new KeyDef ("9", -1, -1)
		};
				public GameObject buttonTemplate;
				public GFRectGrid grid;

#region loop

				// Use this for initialization
				void Start ()
				{
						DoStart ();
				}

				public void DoStart ()
				{
						if (grid == null) {	
								grid = GetComponent<GFRectGrid> ();
						}
						SetGridFromConfig ("Assets/config/grid_config.json");
						foreach (KeyDef k in keys) {
								Vector3 pos = grid.GridToWorld (new Vector3 (k.i, k.j, 0));
								GameObject go = ((GameObject)Instantiate (buttonTemplate, pos, Quaternion.identity));
								TenKeyKey g = (TenKeyKey)(go.gameObject.GetComponent<TenKeyKey> ());
								g.label = k.label;
								g.transform.parent = transform;
								g.transform.position = pos;
				        g.KeypadScale = buttonScale;
						}
				}
	
				// Update is called once per frame
				void Update ()
				{
	
				}
		#endregion
		
		#region configuration
		
				public void SetGridFromConfig (string filePath)
				{
						JSONNode data = Utils.FileToJSON (filePath);
						float x = data ["spacing"] ["x"].AsFloat;
						float y = data ["spacing"] ["y"].AsFloat;
						float z = data ["spacing"] ["z"].AsFloat;

						grid.spacing = new Vector3 (x, y, z);
			
						x = data ["buttonScale"] ["x"].AsFloat;
						y = data ["buttonScale"] ["y"].AsFloat;
						z = data ["buttonScale"] ["z"].AsFloat;
			
						buttonScale = new Vector3 (x, y, z);
				}
		
		#endregion
		}
}