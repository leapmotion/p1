using UnityEngine;
using System.Collections;

namespace P1
{
		public class LeapButton : MonoBehaviour
		{
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

				public State state;

				void ToggleSprites (string stateName)
				{
						Debug.Log ("Changing state to " + stateName);
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
						}
				}

				void OnStateChange (StateChange change)
				{
						if (change.allowed == true) {
								ToggleSprites (change.toState.name);
						}
				}

		#region mouse
		void OnMouseEnter()
		{
			state.Change("over");
				}

		void OnMouseDown()
		{
			state.Change ("down");
		}

		void OnMouseUp()
		{
			state.Change ("over");
				}

		void OnMouseExit()
		{
			state.Change ("default");
				}

		#endregion

			#region loop
				// Use this for initialization
				void Start ()
				{
						Init ();
						state.StateChangedEvent += OnStateChange;
			state.Change("default");
				}
		
				// Update is called once per frame
				void Update ()
				{
		
				}
			#endregion
		}
}