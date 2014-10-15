using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

namespace P1
{
		public class TwitterList : MonoBehaviour
		{
				TwitterReader tr;
				public GameObject items;
				public List<TwitterStatusButton> statusButtons = new List<TwitterStatusButton> ();
				static int MAX_TWEETS = 0;
				bool targetSet = false;
				public string tweetSource;
				const string TWITTER_LIST_TARGET_STATE = "listTriggerState";
				bool inEditor = false;
				static float MOVE_SCALE_COUNTER = 100f;
				static float MOVE_SCALE = 50;
				static float FRICTION = 0.9f;
				private float lastTouched = 0.0f;

		#region TouchState
				const string TWITTER_LIST_STATE_NAME = "Touched state name";
				const string TWITTER_LIST_UNTOUCHED = "Untouched twitter";
				const string TWITTER_LIST_TOUCHED = "Touched twitter";
				State touchedState;

				void InitTouchState ()
				{
						if (!StateList.HasList (TWITTER_LIST_STATE_NAME))
								StateList.Create (TWITTER_LIST_STATE_NAME,
				                  TWITTER_LIST_UNTOUCHED,
				                  TWITTER_LIST_TOUCHED);
						touchedState = new State (TWITTER_LIST_STATE_NAME,
			                         TWITTER_LIST_UNTOUCHED);
						touchedState.StateChangedEvent += OnStateChanged;
				}

				void OnStateChanged (StateChange sc)
				{
						if (sc.fromState.name == TWITTER_LIST_UNTOUCHED &&
								sc.toState.name == TWITTER_LIST_TOUCHED) {
								UnityEngine.Debug.Log ("You touched Bieber!");
						}
			
						if (sc.fromState.name == TWITTER_LIST_TOUCHED &&
								sc.toState.name == TWITTER_LIST_UNTOUCHED) {
								UnityEngine.Debug.Log ("You jilted Bieber!");
						}
				}
		
				public void Touched ()
				{
						lastTouched = Time.time;
						if (touchedState.state == TWITTER_LIST_UNTOUCHED)
								touchedState.Change (TWITTER_LIST_TOUCHED);
				}
		
				void UpdateTouched ()
				{
						if (touchedState.state == TWITTER_LIST_TOUCHED &&
								Utils.Elapsed (lastTouched, 1.0f)) {
								touchedState.Change (TWITTER_LIST_UNTOUCHED);
						}
				}

			#endregion

#region loop
		
				// Use this for initialization
				void Start ()
				{
						LoadConfigs ();
						InitState ();
						if (tweetSource != "")
								ReadTweets (tweetSource);

						InitTouchState ();
				}
		
				// Update is called once per frame
				void Update ()
				{
						if (!targetSet) {
								SetRandomTarget ();
						}
						rigidbody.velocity *= FRICTION;

						UpdateTouched ();
				}

#endregion

				public void LoadConfigs ()
				{
						LoadConfigs ("twitter_list.json");
				}

				public void LoadConfigs (string s)
				{
						JSONNode n = Utils.FileToJSON (s);
						MOVE_SCALE = n ["move_scale"].AsFloat;
						MOVE_SCALE_COUNTER = n ["move_scale_counter"].AsFloat;
						FRICTION = n ["friction"].AsFloat;
						MAX_TWEETS = n ["max_tweets"].AsInt;
				}

				public void InitState ()
				{
						if (!StateList.HasList (TWITTER_LIST_TARGET_STATE))
								InitListTriggerState ();
				}

				public void SetRandomTarget ()
				{
						statusButtons [Random.Range (0, statusButtons.Count - 1)].targetState.Change ("target");
						targetSet = true;
				}

				public void ReadTweets (string source)
				{
						tr = new TwitterReader (source);
						foreach (Tweet s in tr.statuses) {
								AddStatus (s);
						}
				}

				void InitListTriggerState ()
				{
						new StateList (TWITTER_LIST_TARGET_STATE, "base", "scrolling");
				}

				void AddStatus (Tweet s)
				{
						if (statusButtons.Count >= MAX_TWEETS)
								return;

						GameObject go;

						#if UNITY_EDITOR
			            go = (GameObject)Instantiate (Resources.Load ("TwitterListStatus"));
						#else
						//go = new GameObject ();
						//go.AddComponent ("TwitterStatusButton");
						go = (GameObject)Instantiate (Resources.Load ("TwitterListStatus"));
						#endif
						go.transform.parent = items.transform;
						go.transform.rotation = transform.rotation;
						go.transform.localScale = Vector3.one;
			
						TwitterStatusButton status = go.GetComponent<TwitterStatusButton> ();
						status.list = this;
						status.status = s;
						status.index = statusButtons.Count;

						statusButtons.Add (status);
				}

				public TwitterStatusButton PrevStatus (TwitterStatusButton s)
				{
						if (s.index <= 0) {
								return null;
						}
						return statusButtons [s.index - 1];
				}

#region aggregate state changes
/**
 * these states can only be true for a single button;clears sets for other buttons
 */
		
				public void TargetSet (TwitterStatusButton status)
				{
						foreach (TwitterStatusButton s in statusButtons) {
								if (s.index != status.index)
										s.targetState.Change ("base");
						}
				}

				public void ResetAllColors ()
				{
						foreach (TwitterStatusButton sb in statusButtons) {
								sb.ResetColor ();
						}
				}

#endregion

				public void MoveList (Vector3 movement)
				{
						float s;
						if ((movement.y > 0) != (rigidbody.velocity.y > 0))
								s = MOVE_SCALE_COUNTER;
						else 
								s = MOVE_SCALE;
						rigidbody.AddForce (movement * s);
				}
		}
}