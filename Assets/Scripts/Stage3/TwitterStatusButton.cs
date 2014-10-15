using UnityEngine;
using System.Collections;
using TMPro;
using System.Text.RegularExpressions;

namespace P1
{
		public class TwitterStatusButton : MonoBehaviour
		{
				TwitterList list_;
				Tweet status_;
				int index_;
				static float HEIGHT = 2;
				const float MIN_WIDTH = 25;
				const float MARGIN_HEIGHT = 0.5f;
				const float MARGIN_WIDTH = 1.5f;
				const float LINE_HEIGHT = 1.0f;
				public	TextMeshPro text;
				public GameObject background;
				public GameObject backgroundActive;
				public float h;
				public float w;
				Color baseColor;
				public Color targetColor = Color.black;
				public State targetState;
				const string STATE_NAME_TLS = "twitter status button trigger state";
				public GripManager gripManager;
				private Color original_bg_color = new Color (160.0f / 255.0f, 178.0f / 255.0f, 193.0f / 255.0f);
				private Color original_bg_active_color;
				public TextMeshPro indexTextMesh;
		
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
								indexTextMesh.text = value.ToString ();
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
						gripManager = GetComponentInChildren<GripManager> ();
						gripManager.twitterList = list;
						baseColor = background.renderer.material.color;
						if (HEIGHT <= 0)
								HEIGHT = background.renderer.bounds.size.y;

						//original_bg_color = background.renderer.material.color;
						original_bg_active_color = backgroundActive.renderer.material.color;
				}
		
				public void InitState ()
				{
						if (!StateList.HasList (STATE_NAME_TLS))
								InitTLS ();
						targetState = new State (STATE_NAME_TLS);

						targetState.StateChangedEvent += OnTLSStateChange;
				}

#endregion

#region targetState

				public void SetColor (Color color)
				{
						list.ResetAllColors ();

						background.renderer.material.color = color;
						backgroundActive.renderer.material.color = color;
				}

				public void ResetColor ()
				{
						background.renderer.material.color = original_bg_color;
						backgroundActive.renderer.material.color = original_bg_active_color;
				}

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

#region dimensions

				public float height { get { return MARGIN_HEIGHT + Mathf.Max (text.bounds.size.y, HEIGHT); } }
		
				public float width { get { return MARGIN_WIDTH + Mathf.Max (text.bounds.size.x, MIN_WIDTH); } }
		
				public void RefreshPosition ()
				{
						h = height;
						w = width;
						transform.localPosition = new Vector3 (0, index * -(HEIGHT + MARGIN_HEIGHT), 0);
				}

#endregion

				public void MoveList (Vector3 movement)
				{
						list.MoveList (movement);
				}

		}
}