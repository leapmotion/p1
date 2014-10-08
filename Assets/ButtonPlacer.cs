using UnityEngine;
using System.Collections;

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
				GFRectGrid grid;
				int test;
				ButtonTrial monkeyDo;
				float monkeyTime;

				// Use this for initialization
				void Start ()
				{
						DoStart ();
				}

				public void DoStart ()
				{
						monkeyDo = new ButtonTrial ();
						monkeyTime = monkeyDo.WasAtTime ();

						grid = GetComponent<GFRectGrid> ();
						foreach (KeyDef k in keys) {
								Vector3 pos = grid.GridToWorld (new Vector3 (k.i, k.j, 0));
								GameObject go = ((GameObject)Instantiate (buttonTemplate));
								TenKeyKey g = (TenKeyKey)(go.gameObject.GetComponent<TenKeyKey> ());
								g.label = k.label;
								g.transform.parent = transform;
								g.transform.position = pos;
								g.TenKeyEventBroadcaster += new TenKeyKey.TenKeyEventDelegate (monkeyDo.WhenPushed);
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
		}
}