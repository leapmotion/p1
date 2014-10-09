using UnityEngine;
using System.Collections;
using TMPro;

namespace P1
{
		public class TwitterListStatus : MonoBehaviour
		{
		
				TwitterList list_;
				TwitterStatus status_;
				int index_;
				static float MIN_HEIGHT = 0;
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
				public State state;

				public TwitterStatus status {
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

				// Use this for initialization
				void Start ()
				{
						baseColor = background.renderer.material.color;
						if (!StateList.HasList ("tls"))
								InitTLS ();
						state = new State ("tls");
						state.StateChangedEvent += OnStateChange;
						if (text == null)
								text = GetComponentInChildren<TextMeshPro> ();
						if (MIN_HEIGHT <= 0)
								MIN_HEIGHT = background.renderer.bounds.size.y;
				}

				void OnStateChange (StateChange change)
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
	
				// Update is called once per frame
				void Update ()
				{
						RefreshPosition ();
				}

				void InitTLS ()
				{
						new StateList ("tls", "base", "target");
				}
		
				public float height { get { return MARGIN_HEIGHT + Mathf.Max (text.bounds.size.y, MIN_HEIGHT); } }

				public float width { get { return MARGIN_WIDTH + Mathf.Max (text.bounds.size.x, MIN_WIDTH); } }
		
				public void RefreshPosition ()
				{
						h = height;
						w = width;
						transform.localPosition = new Vector3 (0, index * -(MIN_HEIGHT + MARGIN_HEIGHT), 0);
				}
		}
}