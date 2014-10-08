﻿using UnityEngine;
using System.Collections;
using SimpleJSON;

using ButtonMonkey;

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
					new KeyDef ("1", -1, 1),
					new KeyDef ("2", 0, 1),
					new KeyDef ("3", 1, 1),
					new KeyDef ("4", -1, 0),
					new KeyDef ("5", 0, 0),
					new KeyDef ("6", 1, 0),
					new KeyDef ("7", -1, -1),
					new KeyDef ("8", 0, -1),
					new KeyDef ("9", 1, -1)
				};
				public GameObject buttonTemplate;
				public GFRectGrid grid;
				int test;
				ButtonTrial monkeyDo;
				float monkeyTime;

#region loop

				// Use this for initialization
				void Start ()
				{
						DoStart ();
				}

				public void DoStart ()
				{
						monkeyDo = new ButtonTrial ();
						monkeyTime = monkeyDo.WasAtTime ();
						if (grid == null) {	
								grid = GetComponent<GFRectGrid> ();
						}
						SetGridFromConfig ("Assets/config/grid_config.json");
						foreach (KeyDef k in keys) {
								Vector3 pos = grid.GridToWorld (new Vector3 (k.i, k.j, 0));
								GameObject go = ((GameObject)Instantiate (buttonTemplate, pos, Quaternion.identity));
                TenKeyKey g = (TenKeyKey)(go.gameObject.GetComponent<TenKeyKey>());
                g.KeypadScale = buttonScale;
								g.label = k.label;
								go.transform.parent = transform;
								go.gameObject.transform.FindChild ("button").FindChild ("default").GetComponent<SpringJoint> ().connectedAnchor = pos;
								g.TenKeyEventBroadcaster += new TenKeyKey.TenKeyEventDelegate (monkeyDo.WhenPushed);
                go.transform.position = pos;
                go.transform.rotation = transform.rotation;
						}

						// Begin first trial
						monkeyDo.Start ();
						Debug.Log ("Monkey, type: " + monkeyDo.GetTrialKeys ());
				}
	
				// Update is called once per frame
				void Update ()
				{
						if (monkeyTime < monkeyDo.WasAtTime ()) {
								monkeyTime = monkeyDo.WasAtTime ();

								// DEBUG - print progress to log
								if (monkeyDo.IsComplete ()) {
										// Initial instructions
										monkeyDo.Start ();
										Debug.Log ("Monkey, type: " + monkeyDo.GetTrialKeys ());
								} else {
										if (monkeyDo.WasCorrect ()) {
												Debug.Log ("Good monkey! Next, type: " + monkeyDo.GetTargetKey ());
										} else {
												Debug.Log ("Bad monkey! You were told to type: " + monkeyDo.GetTargetKey ());
										}
								}
						}
				}
		#endregion
		
		#region configuration
		
				public void SetGridFromConfig (string filePath)
				{
						JSONNode data = Utils.FileToJSON (filePath);
            float x = data["spacing"]["x"].AsFloat;
            float y = data["spacing"]["y"].AsFloat;
            float z = data["spacing"]["z"].AsFloat;

            grid.spacing = new Vector3(x, y, z);

            x = data["buttonScale"]["x"].AsFloat;
            y = data["buttonScale"]["y"].AsFloat;
            z = data["buttonScale"]["z"].AsFloat;

            buttonScale = new Vector3(x, y, z);
				}
		
		#endregion
		}
}