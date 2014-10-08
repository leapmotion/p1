using UnityEngine;
using System.Collections;
using System.IO;
using System.Text;

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
				string testPath = ""; //DEFAULT: Record in TestResults
				int testNum = 1; //DEFAULT: Run one trial
				ButtonTrial monkeyDo;
				public GameObject pinPrompt;

#region loop

				// Use this for initialization
				void Start ()
				{
						DoStart ();
				}

				public void DoStart ()
				{
						monkeyDo = new ButtonTrial ();
						if (grid == null) {	
								grid = GetComponent<GFRectGrid> ();
						}
						SetGridFromConfig ("Assets/config/grid_config.json");
						foreach (KeyDef k in keys) {
								Vector3 pos = grid.GridToWorld (new Vector3 (k.i, k.j, 0));
								GameObject go = ((GameObject)Instantiate (buttonTemplate, pos, Quaternion.identity));
								TenKeyKey g = (TenKeyKey)(go.gameObject.GetComponent<TenKeyKey> ());
								g.KeypadScale = buttonScale;
								g.label = k.label;
								go.transform.parent = transform;
								go.gameObject.transform.FindChild ("button").FindChild ("default").GetComponent<SpringJoint> ().connectedAnchor = pos;
								g.TenKeyEventBroadcaster += new TenKeyKey.TenKeyEventDelegate (monkeyDo.WhenPushed);
								go.transform.position = pos;
								go.transform.rotation = transform.rotation;
						}

						// Begin first trial
						SetTestFromConfig ("Assets/config/test_config.json");
						test = 1;

						monkeyDo.TrialEvent += TrialUpdate;
						monkeyDo.Start ();
						Debug.Log ("Monkey, type: " + monkeyDo.GetTrialKeys ());
						pinPrompt.GetComponent<PINPrompt> ().UpdatePIN (monkeyDo.GetTrialKeys (), 1);
				}

				// Called once for each key pushed
				void  TrialUpdate (ButtonTrial trial, bool correct)
				{
						if (trial.IsComplete ()) {
								if (test < testNum) {
										if (test > 0) {
												pinPrompt.GetComponent<PINPrompt> ().TogglePIN (true);
										}
										// Initial instructions
										test += 1;
										monkeyDo.Start ();
										Debug.Log ("Monkey, type: " + monkeyDo.GetTrialKeys ());
										pinPrompt.GetComponent<PINPrompt> ().UpdatePIN (monkeyDo.GetTrialKeys (), 60);
								} else {
										pinPrompt.GetComponent<PINPrompt> ().TogglePIN (true);
										Debug.Log ("Autopsy report for monkey:\n" + monkeyDo.ToString ());
										string path = Application.dataPath + "/TestResults/" + testPath;
										Directory.CreateDirectory (path);
										path += string.Format ("ButtonTest-{0:yyyy-MM-dd_hh-mm-ss-tt}.csv", System.DateTime.Now);
										File.WriteAllText (path, monkeyDo.ToString ());
										Debug.Log ("Autopsy report written to: " + path);

										//TODO: Applaud Monkey *IN-SCENE*
										SceneManager.instance.Next();
								}
						} else {
								if (monkeyDo.WasCorrect ()) {
										pinPrompt.GetComponent<PINPrompt> ().TogglePIN (true);
										Debug.Log ("Good monkey! Next, type: " + monkeyDo.GetTargetKey ());
								} else {
										pinPrompt.GetComponent<PINPrompt> ().TogglePIN (false);
										Debug.Log ("Bad monkey! You were told to type: " + monkeyDo.GetTargetKey ());
								}
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

				public void SetTestFromConfig (string filePath)
				{
						JSONNode data = Utils.FileToJSON (filePath);
						testPath = data ["results_dir"].ToString ();
						// NOTE: JSONNode ToString helpfully interprets both path/ (no quotes in file) and "path/" (quotes in file)
						// as "path/" (quotes IN string).
						testPath = testPath.Substring (1, testPath.Length - 2);
						testNum = data ["trial_count"].AsInt;
				}
		
		#endregion
		}
}