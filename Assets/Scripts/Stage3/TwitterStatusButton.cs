using UnityEngine;
using System.Collections;
using TMPro;
using System.Text.RegularExpressions;
using SimpleJSON;

namespace P1
{
		public class TwitterStatusButton : MonoBehaviour
		{
				TwitterList list_;
				Tweet status_;
				int index_;
				static float HEIGHT = 3;
				const float MIN_WIDTH = 25;
				const float MARGIN_HEIGHT = 0.5f;
				const float MARGIN_WIDTH = 1.5f;
				const float LINE_HEIGHT = 1.0f;
				public	TextMeshPro text;
				public GameObject background;
				public GameObject backgroundTarget;
				public float h;
				public float w;
				Color baseColor;
				public Color targetColor = Color.black;
				public State targetState;
				public State overState;
				const string TARGETED_STATE_NAME = "twitter status button targeted";
				const string BASE = "base";
				const string TARGET = "target";
				const string OVER_STATE_NAME = "twitter status button over";
				const string NOT_OVER = "not over";
				const string OVER = "over";
				public GripManager gripManager;
				static Color back_color;
				static Color over_color;
				static Color target_color;
				static Color over_target_color;
				public TextMeshPro indexTextMesh;
				static bool colorsLoaded = false;
		
				public Tweet status {
						get { return status_; }
						set {
								status_ = value;
				text.text = SizeMe(value.text);
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
						InitColors ();
						InitBackground ();
						SetColor (back_color);
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
				}
		
				public void InitState ()
				{
						OverStates ();

						targetState = new State (TARGETED_STATE_NAME);
						overState = new State (OVER_STATE_NAME);

						targetState.StateChangedEvent += OnTargetedStateChange;
						overState.StateChangedEvent += OnOverStateChange;

				}

#endregion
		
		const int MAX_WORD_LENGTH = 30;
		const int SIZE_BOOST = 2;
		string SizeMe(string s)
		{
			string first = "";
			string rest = "";
			string[] words = s.Split(' ');
			foreach (string w in words)
			{
				if (first.Length < MAX_WORD_LENGTH)
					first += (w + " ");
				else
					rest += (w + " ");
			}
			return string.Format("<b>{1}</b><size=-{0}>{2}</size>", SIZE_BOOST, first, rest);
		}

#region targetState

		
				void InitColors ()
				{
						if (!colorsLoaded) {
								JSONNode n = Utils.FileToJSON ("twitter_button_config.json");
				
								back_color = Utils.JsonNodeToColor (n ["back"]);
								over_color = Utils.JsonNodeToColor (n ["over"]);
								target_color = Utils.JsonNodeToColor (n ["target"]);
								over_target_color = Utils.JsonNodeToColor (n ["over_target"]);
				
								colorsLoaded = true;
						}
			
				}

				public void Activate ()
				{
						foreach (TwitterStatusButton b in list.statusButtons) {
								if (b.index != index)
					b.overState.Change (NOT_OVER);
			}
						overState.Change (OVER);
			
				}
		
				public void SetColor (Color color)
				{
						SetColor (color, color);
				}
		
				public void SetColor (Color color, Color tColor)
				{
						background.renderer.material.color = color;
						backgroundTarget.renderer.material.color = tColor;
				}
		
				public void ResetColor ()
				{
						SetColor (back_color, over_color);
				}

				void OnOverStateChange (StateChange change)
				{
						UpdateColors ();
				}

				void OnTargetedStateChange (StateChange change)
				{
						Debug.Log (string.Format ("Setting state of {0} to {1}", index, change.state));

						switch (change.toState.name) {
						case BASE: 
								background.SetActive (true);
								backgroundTarget.SetActive (false);
								break;
				
						case  TARGET: 
								background.SetActive (false);
								backgroundTarget.SetActive (true);
								list.TargetSet (this);
								break;
						}
						UpdateColors ();
				}

				void UpdateColors ()
				{
						if (targetState.state == TARGET) {
								if (overState.state == OVER) {
										SetColor (over_target_color);
								} else {
										SetColor (target_color);
								}
						} else {
								if (overState.state == OVER) {
										SetColor (over_color);
								} else {
										SetColor (back_color);
								}
						}
				}
		
				void OverStates ()
				{
						if (!StateList.HasList (TARGETED_STATE_NAME))
								StateList.Create (TARGETED_STATE_NAME, BASE, TARGET);
						if (!StateList.HasList (OVER_STATE_NAME))
								StateList.Create (OVER_STATE_NAME, NOT_OVER, OVER);
				}
		
#endregion

#region dimensions

				public float height { get { return MARGIN_HEIGHT + Mathf.Max (text.bounds.size.y, HEIGHT); } }
		
				public float width { get { return MARGIN_WIDTH + Mathf.Max (text.bounds.size.x, MIN_WIDTH); } }
		
				public void RefreshPosition ()
				{
						h = height;
						w = width;
						transform.localPosition = new Vector3 (0, index * -(HEIGHT + MARGIN_HEIGHT * 2.0f), 0);
				}

#endregion

				public void MoveList (Vector3 movement)
				{
						list.MoveList (movement);
				}

		}
}