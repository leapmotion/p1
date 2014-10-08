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

				public LeapButton b0;
				public LeapButton b1;
				public LeapButton b2;
				public LeapButton b3;
				public LeapButton b4;
				public LeapButton b5;
				public LeapButton b6;
				public LeapButton b7;
				public LeapButton b8;
				public LeapButton b9;
				public LeapButton bStar;
				public LeapButton bHash;

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
								label [0] == '9' ||
								label [0] == '*' ||
								label [0] == '#') {
								return true;
						}
						return false;
				}

				void OnButtonClick (ButtonEvent e)
				{
						UnityEngine.Debug.Log ("Button value = " + e.button.buttonValue);

						// Using labeling string instead of referencing Label object
						if (IsKeyPad (e.button.buttonValue)) {
								counter.WhenPushed (true, e.button.buttonValue [0], timer.ElapsedMilliseconds / 1000.0f);
						} else {
								UnityEngine.Debug.Log ("Ignoring button " + e.button.buttonValue);
						}
				}
	
	#endregion
	
	
	#region loop

				// Use this for initialization
				void Start ()
				{
						b0.ButtonEventBroadcaster += OnButtonClick;
						b1.ButtonEventBroadcaster += OnButtonClick;
						b2.ButtonEventBroadcaster += OnButtonClick;
						b3.ButtonEventBroadcaster += OnButtonClick;
						b4.ButtonEventBroadcaster += OnButtonClick;
						b5.ButtonEventBroadcaster += OnButtonClick;
						b6.ButtonEventBroadcaster += OnButtonClick;
						b7.ButtonEventBroadcaster += OnButtonClick;
						b8.ButtonEventBroadcaster += OnButtonClick;
						b9.ButtonEventBroadcaster += OnButtonClick;
						bStar.ButtonEventBroadcaster += OnButtonClick;
						bHash.ButtonEventBroadcaster += OnButtonClick;

						timer.Start ();
				}
	
				// Update is called once per frame
				void Update ()
				{
	
				}

	#endregion
		}
}
