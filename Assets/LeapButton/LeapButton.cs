using UnityEngine;
using System.Collections;

namespace P1
{
		public struct ButtonEvent
		{
				public LeapButton button;
				public string eventType;

				public ButtonEvent (LeapButton b, string e)
				{
						button = b;
						eventType = e;
				}
		}

		public class LeapButton : MonoBehaviour
		{
				public string buttonValue;

				public GameObject defaultSprite;
				public GameObject overSprite;
				public GameObject downSprite;

				void Init ()
				{
						if (!StateList.HasList ("ButtonState")) {
								new StateList ("ButtonState", "unknown", "default", "over", "down");
						}
						state = new State ("ButtonState");
				}

				void ToggleSprites (string stateName)
				{
						switch (stateName) {
						case "default":
								defaultSprite.GetComponent<SpriteRenderer> ().enabled = true;
								overSprite.GetComponent<SpriteRenderer> ().enabled = false;
								downSprite.GetComponent<SpriteRenderer> ().enabled = false;
								break;
						case "over":
								defaultSprite.GetComponent<SpriteRenderer> ().enabled = false;
								overSprite.GetComponent<SpriteRenderer> ().enabled = true;
								downSprite.GetComponent<SpriteRenderer> ().enabled = false;
								break;
						case "down":
								defaultSprite.GetComponent<SpriteRenderer> ().enabled = false;
								overSprite.GetComponent<SpriteRenderer> ().enabled = false;
								downSprite.GetComponent<SpriteRenderer> ().enabled = true;
								break;
						default:
								Debug.Log ("Unknown stateName " + stateName);
								break;
						}
				}

				void OnStateChange (StateChange change)
				{
						if (change.allowed == true) {
								ToggleSprites (change.toState.name);
						}
				}

		#region mouse
		
		public State state;

				void OnMouseEnter ()
				{
						state.Change ("over");
				}

				void OnMouseDown ()
				{
						state.Change ("down");
				}

				void OnMouseUp ()
				{
						OnButtonEvent ("click");
						state.Change ("over");
				}

				void OnMouseExit ()
				{
						state.Change ("default");
				}

		#endregion

		#region Events
		
				public delegate void ButtonEventDelegate (ButtonEvent change);
		
				/// <summary>An event that gets fired </summary>
				public event ButtonEventDelegate ButtonEventBroadcaster;

				public void OnButtonEvent (string e)
				{
						if (ButtonEventBroadcaster != null) // fire the event
								ButtonEventBroadcaster (new ButtonEvent (this, e));
				}
		
		#endregion

			#region loop
				// Use this for initialization
				void Start ()
				{
						Init ();
						state.StateChangedEvent += OnStateChange;
						state.Change ("default");
				}
		
				// Update is called once per frame
				void Update ()
				{
		
				}
			#endregion
		}
}