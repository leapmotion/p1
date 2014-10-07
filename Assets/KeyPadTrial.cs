using UnityEngine;
using System.Collections;
using System.Diagnostics;

using ButtonMonkey;

namespace P1
{
		public class KeyPadTrial : MonoBehaviour
		{
				Stopwatch timer;

				public ButtonCounter counter;

				KeyPadTrial ()
				{
						counter = new ButtonCounter ();
						timer = new Stopwatch ();
				}

	#region events

				bool IsKeyPad (string label)
				{
						if (label.Length != 1) {
								return false;
						}
						if (label [0] == '0' ||
								label [0] == '1' ||
								label [0] == '2' ||
								label [0] == '3' ||
								label [0] == '4' ||
								label [0] == '5' ||
								label [0] == '6' ||
								label [0] == '7' ||
								label [0] == '8' ||
								label [0] == '9') {
								return true;
						}
						return false;
				}

				void OnButtonClick (ButtonEvent e)
				{
						UnityEngine.Debug.Log ("Button value = " + e.button.buttonValue);

						// Using labeling string instead of referencing Label object
						if (IsKeyPad (e.button.buttonValue)) {
								counter.WhenPushed (e.button.buttonValue [0], timer.ElapsedMilliseconds / 1000.0f);
						} else {
								UnityEngine.Debug.Log ("Ignoring button " + e.button.buttonValue);
						}
				}
	
	#endregion
	
	
	#region loop
				LeapButton broadcaster;

				// Use this for initialization
				void Start ()
				{
						broadcaster.ButtonEventBroadcaster += OnButtonClick;

						timer.Start ();
				}
	
				// Update is called once per frame
				void Update ()
				{
	
				}

	#endregion
		}
}
