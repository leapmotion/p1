using UnityEngine;
using System.Collections;
using TMPro;

namespace P1
{
		public class TwitterStatusButton : MonoBehaviour
		{
		
				TwitterList list_;
				Tweet status_;
				int index_;
				static float MIN_HEIGHT = 0;
				const float MIN_WIDTH = 25;
				const float MARGIN_HEIGHT = 0.5f;
				const float MARGIN_WIDTH = 1.5f;
				const float LINE_HEIGHT = 1.0f;
				public	TextMeshPro text;
				public GameObject background;
				public GameObject backgroundActive;
				public GameObject grips;
				public float h;
				public float w;
				Color baseColor;
				public Color targetColor = Color.black;
				public State targetState;
				public State hoverState;
				const string STATE_NAME_TLS_HOVER = "twitter status button hover state";
				const string STATE_NAME_TLS = "twitter status button trigger state";

				public Tweet status {
						get { return status_; }
						set {
								status_ = value;
								text.text = value.text;
								RefreshPosition ();
						}
				}

				public TwitterList list {
						get { return list_; }
						set {
								list_ = value;
						}
				}

				public int index {
						get { return index_; }
						set {
								index_ = value;
								RefreshPosition ();
						}
				}

#region loop

// Update is called once per frame
				void Update ()
				{
						RefreshPosition ();
				}
		
				// Use this for initialization
				void Start ()
				{
						InitState ();
						InitText ();
						InitBackground ();
				}

				public void InitText ()
				{
						if (text == null)
								text = GetComponentInChildren<TextMeshPro> ();
				}

				public void InitBackground ()
				{
						grips.SetActive (false);
						baseColor = background.renderer.material.color;
						if (MIN_HEIGHT <= 0)
								MIN_HEIGHT = background.renderer.bounds.size.y;
				}
		
				public void InitState ()
				{
						if (!StateList.HasList (STATE_NAME_TLS))
								InitTLS ();
						if (!StateList.HasList (STATE_NAME_TLS_HOVER))
								InitTLSHover ();

						targetState = new State (STATE_NAME_TLS);
						hoverState = new State (STATE_NAME_TLS_HOVER);

						targetState.StateChangedEvent += OnTLSStateChange;
						hoverState.StateChangedEvent += OnTLSHoverStateChange;
				}

#endregion

#region targetState
		
				void OnTLSStateChange (StateChange change)
				{
						switch (change.toState.name) {
						case "base": 
								background.SetActive (true);
								backgroundActive.SetActive (false);
								break;
				
						case "target": 
								background.SetActive (false);
								backgroundActive.SetActive (true);
								list.TargetSet (this);
								break;
						}
				}
		
				void InitTLS ()
				{
						new StateList (STATE_NAME_TLS, "base", "target");
				}
		
#endregion

#region hover
		
				void OnTLSHoverStateChange (StateChange change)
				{
						switch (change.toState.name) {
						case "base": 
								background.SetActive (true);
								backgroundActive.SetActive (false);
								grips.SetActive (false);
								break;
				
						case "hover": 
								background.SetActive (false);
								backgroundActive.SetActive (true);
								list.HoverSet (this); // neutralize any old hovers
								grips.SetActive (true);
								break;
						}
				}

				void InitTLSHover ()
				{
						new StateList (STATE_NAME_TLS_HOVER, "base", "hover");
				}
		
#endregion

#region dimensions

				public float height { get { return MARGIN_HEIGHT + Mathf.Max (text.bounds.size.y, MIN_HEIGHT); } }
		
				public float width { get { return MARGIN_WIDTH + Mathf.Max (text.bounds.size.x, MIN_WIDTH); } }
		
				public void RefreshPosition ()
				{
						h = height;
						w = width;
						transform.localPosition = new Vector3 (0, index * -(MIN_HEIGHT + MARGIN_HEIGHT), 0);
				}

#endregion

		}
}