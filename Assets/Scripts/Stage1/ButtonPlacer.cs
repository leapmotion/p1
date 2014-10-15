using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;

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
				public KeyDef[] keys = new KeyDef[]{
					new KeyDef ("1", -1, 1),
					new KeyDef ("2", 0, 1),
					new KeyDef ("3", 1, 1),
					new KeyDef ("4", -1, 0),
					new KeyDef ("5", 0, 0),
					new KeyDef ("6", 1, 0),
					new KeyDef ("7", -1, -1),
					new KeyDef ("8", 0, -1),
					new KeyDef ("9", 1, -1),
					new KeyDef ("0", 0, -2)
				};
				public GameObject buttonTemplate;
				GridMonkey monkeyDo;
				public GameObject pinPrompt;
				private bool restrain_buttons_ = false;
				[HideInInspector]
				public List<GameObject>
						key_gameObjects_ = new List<GameObject> ();

    #region loop

				// Use this for initialization
				void Start ()
				{
						DoStart ();
				}

				public void DoStart ()
				{
						SetGridFromConfig ("grid_config.json");
						restrain_buttons_ = Utils.FileToJSON ("grid_config.json") ["grid"] ["restrainButtons"].AsBool;
						monkeyDo = new GridMonkey ();
						monkeyDo.ConfigureTest ("grid");
						monkeyDo.TrialEvent += TrialUpdate;
						monkeyDo.Start ();
						Debug.Log ("Monkey, type: " + monkeyDo.GetTrialKeysString ());

						pinPrompt.GetComponent<PINPrompt> ().UpdatePIN (monkeyDo.GetTrialKeysString ());
				}

				// Called once for each key pushed
				void TrialUpdate (MonkeyTester trial)
				{
						if (monkeyDo.StageComplete ()) {
								// Show final correct result
								pinPrompt.GetComponent<PINPrompt> ().TogglePIN (true);
								Debug.Log ("Autopsy report for monkey:\n" + monkeyDo.ToString ());
								if (CameraManager.instance) {
										CameraManager.instance.NextScene ();
								}
						} else {
								if (monkeyDo.TrialComplete ()) {
										// Show final correct result
										pinPrompt.GetComponent<PINPrompt> ().TogglePIN (true);

										monkeyDo.Start ();
										Debug.Log ("Monkey, type: " + monkeyDo.GetTrialKeysString ());
										pinPrompt.GetComponent<PINPrompt> ().UpdatePIN (monkeyDo.GetTrialKeysString ());
								} else {
										if (monkeyDo.WasCorrect ()) { 
												Debug.Log ("Good monkey! Next, type: " + monkeyDo.GetTrialKeysString () [monkeyDo.GetTrialStep ()]);
												pinPrompt.GetComponent<PINPrompt> ().TogglePIN (true);
										} else {
												Debug.Log ("Bad monkey! You were told to type: " + monkeyDo.GetTrialKeysString () [monkeyDo.GetTrialStep ()]);
												pinPrompt.GetComponent<PINPrompt> ().TogglePIN (false);
												pinPrompt.GetComponent<PINPrompt> ().CheckPinIndex (monkeyDo.GetTrialStep ());
										}
								}
						}
				}

				// Update is called once per frame
				void Update ()
				{
				}

				void FixedUpdate ()
				{
						if (restrain_buttons_) {
								GameObject active_key = null;
								foreach (GameObject key in key_gameObjects_) {
										if (key.GetComponent<TenKeyKey> ().button.GetComponent<ButtonTrigger> ().is_active_) {
												active_key = key;
												break;
										}
								}

								foreach (GameObject key in key_gameObjects_) {
										if (active_key != null && key != active_key) {
												key.GetComponent<TenKeyKey> ().button.GetComponent<ButtonTrigger> ().restricted_ = true;
										} else {
												key.GetComponent<TenKeyKey> ().button.GetComponent<ButtonTrigger> ().restricted_ = false;
										}
								}
						}
				}

    #endregion

    #region configuration

				public void SetGridFromConfig (string filePath)
				{
						JSONNode data = Utils.FileToJSON (filePath);

						float angle = data ["grid"] ["angle"].AsFloat;
						float distance = data ["grid"] ["distance"].AsFloat;
						float size = data ["button"] ["size"].AsFloat;

						// Prompt Landscape
						// True - Left->Right
						// False - Top->Down
						bool isLandscape = data ["prompt"] ["isLandscape"].AsBool;
						float prompt_h = (isLandscape) ? 1.0f * size : 4.0f * size;
						float prompt_w = (isLandscape) ? 4.0f * size : 1.0f * size;
						float prompt_padding = data ["prompt"] ["padding"].AsFloat;

						// Prompt Pos
						// 0 - Top
						// 1 - Bottom
						// 2 - Right
						// 3 - Left
						int promptPos = data ["prompt"] ["position"].AsInt;
						float prompt_rel_h = 0.0f;
						float prompt_rel_w = 0.0f;
						switch (promptPos) {
						case 0: // Top
								prompt_rel_h += prompt_h + prompt_padding;
								break;
						case 1: // Bottom
								prompt_rel_h -= prompt_h + prompt_padding;
								break;
						case 2: // Right
								prompt_rel_w += prompt_w + prompt_padding;
								break;
						case 3: // Left
								prompt_rel_w -= prompt_w + prompt_padding;
								break;
						default: // Default: Top
								prompt_rel_h += prompt_h + prompt_padding;
								break;
						}

						Vector3 center = new Vector3 (
        -prompt_rel_w / 2.0f,
        -prompt_rel_h / 2.0f,
        -size
						);

						Vector3 spacing = new Vector3 (data ["grid"] ["spacing_x"].AsFloat, data ["grid"] ["spacing_y"].AsFloat, 0.0f);
						float sensitivity = data ["button"] ["sensitivity"].AsFloat;

						int row = data ["grid"] ["row"].AsInt;
						int col = data ["grid"] ["col"].AsInt;
						int num_keys = row * col;
						if (num_keys > keys.Length) {
								num_keys = keys.Length;
								row = 4;
								col = 3;
						}

						float degree_per_unit = 1.0f / distance * 180.0f / Mathf.PI;
						float numpad_w = (float)(col - 1) * spacing.y + (float)col * size;
						float numpad_h = (float)(row - 1) * spacing.x + (float)row * size;
						float x_coord = -(numpad_w / 2.0f - size / 2.0f);
						float y_coord = (numpad_h / 2.0f - size / 2.0f);
						for (int i = 0; i < num_keys; ++i) {
								KeyDef k = keys [i];

								//// Construct the matrix of keys based on the rows and cols. The last key will be at the last rol and centered
								float x_degree = (x_coord + center.x) * degree_per_unit;
								float y_degree = (y_coord + center.y) * degree_per_unit;
								Vector3 position = Quaternion.Euler (-y_degree, x_degree, 0.0f) * Vector3.forward * distance;

								if (i == keys.Length - 1 && row == 4 && col == 3) { // Zero
										x_coord = 0.0f;
								} else {
										x_coord += spacing.x + size;

										if (x_coord > numpad_w / 2.0f) {
												x_coord = -(numpad_w / 2.0f - size / 2.0f);
												y_coord -= spacing.y + size;
										}
								}

								GameObject go = ((GameObject)Instantiate (buttonTemplate, position, Quaternion.identity));
								go.SetActive (true);
								TenKeyKey g = (TenKeyKey)(go.gameObject.GetComponent<TenKeyKey> ());
								g.KeypadScale = Vector3.one * size;
								g.label = k.label;
								go.transform.parent = transform;
								g.TenKeyEventBroadcaster += new TenKeyKey.TenKeyEventDelegate (monkeyDo.WhenPushed);
								go.transform.localScale = Vector3.one * size;
								go.transform.rotation = Quaternion.LookRotation (go.transform.position - Camera.main.transform.position);
								key_gameObjects_.Add (go);
						}

						Vector3 promptPosition = center;
						switch (promptPos) {
						case 0:
								promptPosition.y += prompt_padding + (numpad_h + prompt_h) / 2.0f;
								break;
						case 1:
								promptPosition.y -= prompt_padding + (numpad_h + prompt_h) / 2.0f;
								break;
						case 2:
								promptPosition.x += prompt_padding + (numpad_w + prompt_w) / 2.0f;
								break;
						case 3:
								promptPosition.x -= prompt_padding + (numpad_w + prompt_w) / 2.0f;
								break;
						default:
								promptPosition.y += prompt_padding + (numpad_h + prompt_h) / 2.0f;
								break;
						}
      
						pinPrompt.transform.position = Quaternion.Euler (-(promptPosition.y * degree_per_unit), promptPosition.x * degree_per_unit, 0.0f) * Vector3.forward * distance;
						pinPrompt.transform.localScale = Vector3.one * size;
						pinPrompt.transform.rotation = Quaternion.LookRotation (pinPrompt.transform.position - Camera.main.transform.position);
						pinPrompt.GetComponent<PINPrompt> ().SetOrientation (isLandscape);

						transform.rotation = Quaternion.Euler (-angle, 0.0f, 0.0f);
						foreach (GameObject key in key_gameObjects_) {
								key.gameObject.transform.FindChild ("button").FindChild ("default").GetComponent<SpringJoint> ().connectedAnchor = key.transform.position;
								key.gameObject.transform.FindChild ("button").FindChild ("default").GetComponent<SpringJoint> ().spring = 1000;
						}

						transform.Find ("Sphere").transform.localScale = Vector3.one * distance * (2 + size);
						Mesh mesh = transform.Find ("Sphere").GetComponent<MeshFilter> ().mesh;
						mesh.triangles = mesh.triangles.Reverse ().ToArray ();
						mesh.RecalculateNormals ();
				}

    #endregion
		}
}