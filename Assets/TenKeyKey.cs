using UnityEngine;
using System.Collections;

namespace P1
{
		public class TenKeyKey : MonoBehaviour
		{
		
				[SerializeField]
				private float
						m_KeypadUniformScale;

				public float KeypadUniformScale {
						get { return m_KeypadUniformScale; }
						set {
								this.transform.localScale = new Vector3 (value, value, value);
								m_KeypadUniformScale = value;
						}
				}

				[SerializeField]
				private Vector3
						m_KeypadScale;

				public Vector3 KeypadScale {
						get { return m_KeypadScale; }
						set {
								this.transform.localScale = value;
								m_KeypadScale = value;
						}
				}

				public GameObject[] labels = new GameObject[10];
				public GameObject button;
				string label_ = "";
				private bool mouse_clicked = false;

				public string label {
						get {
								return label_;
						}

						set {
								switch (value) {
								case "1": 
										break;
					
					
								case "2": 
										break;
					
					
								case "3": 
										break;
					
					
								case "4": 
										break;
					
					
								case "5": 
										break;
					
					
								case "6": 
										break;
					
					
								case "7": 
										break;
					
					
								case "8": 
										break;
					
					
								case "9": 
										break;
					
					
								case "0": 
										break;
					
					
								default: 
										return;
								}
								label_ = value;
								foreach (GameObject g in labels) {
										g.SetActive (false);
								}

								try {
										labels [System.Convert.ToInt16 (value)].SetActive (true);
								} catch (UnityException e) {
										Debug.Log ("Cannot set letter for value " + value);
								}
						}

				}

				public void UpdateColor (Color color)
				{
						float alpha = button.renderer.material.color.a;
						color.a = alpha;
						button.renderer.material.color = color;
				}
		#region loop

				State state;

				// Use this for initialization
				void Start ()
				{
						if (!StateList.HasList ("ButtonState")) {
								new StateList ("ButtonState", "unknown", "default", "over", "down");
						}
						state = new State ("ButtonState");
					
				}
	
				// Update is called once per frame
				void Update ()
				{
						if (!mouse_clicked &&
								Input.GetMouseButtonDown (0)) {
								mouse_clicked = true;
								Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
								RaycastHit hit;
								if (Physics.Raycast (ray, out hit, 100)) {
										if (hit.transform.parent.parent == this.transform && 
												hit.transform.gameObject.layer == LayerMask.NameToLayer ("Mouse")) {
												OnTenKeyEvent (true, "click"); //Mouse click cannot be incomplete
										}
								}
						}
          
						//NOTE: ButtonUp is an event, NOT a state,
						//so mouse_clicked must be initialized to false
						//in order to register the first click
						if (Input.GetMouseButtonUp (0)) {
								mouse_clicked = false;
						}
				}

		#endregion

		#region broadcast
		
				public delegate void TenKeyEventDelegate (bool complete, char symbol);

				public event TenKeyEventDelegate TenKeyEventBroadcaster;
		
				public void OnTenKeyEvent (bool complete, string e)
				{
						if (TenKeyEventBroadcaster != null) {
								Debug.Log ("Event Firing: " + label [0]);
								TenKeyEventBroadcaster (complete, label [0]);
						}
				}

		#endregion

		#region mouse2D
		
				void OnMouseEnter ()
				{
						//Debug.Log ("OnMouseEnter");
						state.Change ("over");
				}
		
				void OnMouseDown ()
				{
						//Debug.Log ("OnMouseDown");
						state.Change ("down");
				}
		
				void OnMouseUp ()
				{
						//Debug.Log ("OnMouseUp");
						OnTenKeyEvent (true, "click");
						state.Change ("over");
				}
		
				void OnMouseExit ()
				{
						//Debug.Log ("OnMouseExit");
						state.Change ("default");
				}

		#endregion

		}
}