using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using ButtonMonkey;

namespace P1
{
		public class TwitterList : MonoBehaviour
		{
				TwitterMonkey monkeyDo;
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
				TwitterStatusButton targetedButton;
				public GameObject upInd;
				public GameObject downInd;
				public GameObject upArrowInd;
				public GameObject downArrowInd;
				public ArrowDisplay upDisplay;
				public ArrowDisplay downDisplay;
		
		#region TouchState
				const string TWITTER_LIST_STATE_NAME = "Touched state name";
				const string TWITTER_LIST_UNTOUCHED = "Untouched twitter";
				const string TWITTER_LIST_TOUCHED = "Touched twitter";
				State touchedState;
				public float stopDelay = 0.25f;
		
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
								//	UnityEngine.Debug.Log ("You touched Bieber!");
						}
			
						if (sc.fromState.name == TWITTER_LIST_TOUCHED &&
								sc.toState.name == TWITTER_LIST_UNTOUCHED) {
								//	UnityEngine.Debug.Log ("You jilted Bieber!");
								rigidbody.velocity = Vector3.zero;
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
								Utils.Elapsed (lastTouched, stopDelay)) {
								touchedState.Change (TWITTER_LIST_UNTOUCHED);
								if (Radical.instance.activeTwitter) {
//										UnityEngine.Debug.Log ("Monkey picked: " + Radical.instance.activeTwitter.index);
										//monkeyDo.WhenPushed (true, Radical.instance.activeTwitter.index);
								}
						}

						ActivateClosestRadicalButton ();
				}

				public void Trigger ()
				{
						if (Radical.instance.activeTwitter) {
								monkeyDo.WhenPushed (true, Radical.instance.activeTwitter.index);
						}
				}

			#endregion

				void ActivateClosestRadicalButton ()
				{

						float distance = 10000;
						TwitterStatusButton closest = null;
						if (Radical.instance == null)
								return;
						float radialY = Radical.instance.transform.position.y;

						foreach (TwitterStatusButton b in statusButtons) {
								if (closest == null)
										closest = b;
								else if (Radical.DistanceFrom (b.text.transform.position.y) < distance) {
										closest = b;
										distance = Radical.DistanceFrom (closest.text.transform.position.y);
								}
						}

						Radical.instance.activeTwitter = closest;
				}

#region loop
		
				// Use this for initialization
				void Start ()
				{
						LoadConfigs ();
						InitState ();
						if (tweetSource != "")
								ReadTweets (tweetSource);
						InitTouchState ();
						monkeyDo = new TwitterMonkey ();
						monkeyDo.ConfigureTest ("twitter");
						monkeyDo.TrialEvent += TrialUpdate;
						monkeyDo.Start ();
				}
		
				// Update is called once per frame
				void Update ()
				{
						if (!targetSet) {
								SetRandomTarget ();
						}
						rigidbody.velocity *= FRICTION;
			
						if (targetSet) {
								ShowArrowKeys ();
						}
						UpdateTouched ();
						UpdateInd ();
				}

				public void TrialUpdate (MonkeyTester trial)
				{
						if (monkeyDo.StageComplete ()) {
								// Show final correct result
								Debug.Log ("Autopsy report for monkey:\n" + monkeyDo.ToString ());
								if (CameraManager.instance) {
										CameraManager.instance.NextScene ();
								}
						} else {
								if (monkeyDo.TrialComplete ()) {
										// Show final correct result
										SetRandomTarget ();
										Debug.Log ("Monkey, type: " + monkeyDo.GetTrialKeysString ());
								} else {
										if (monkeyDo.WasCorrect ()) { 
												Debug.Log ("Good monkey! Next, type: " + monkeyDo.GetTrialKeysString () [monkeyDo.GetTrialStep ()]);
										} else {
												Debug.Log ("Bad monkey! You were told to type: " + monkeyDo.GetTrialKeysString () [monkeyDo.GetTrialStep ()]);
										}
								}
						}
				}
		
		#endregion
		
		
				public void UpdateInd ()
				{
						TwitterStatusButton a = Radical.instance.activeTwitter;

						if (upInd != null)
								upInd.SetActive (false);
						if (downInd != null)
								downInd.SetActive (false);

						if ((targetedButton == null) || (a == null))
								return;
						if (upInd != null) {	
								upInd.SetActive (targetedButton.index < a.index);
						}

						if (downInd != null) {
								downInd.SetActive (targetedButton.index > a.index);
						}
				}

				public void LoadConfigs ()
				{
						LoadConfigs ("twitter_config.json");
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
						monkeyDo.statusButtonsCount = statusButtons.Count - 1;
						monkeyDo.Start ();
						int target = monkeyDo.GetTrialKeys () [0];
						if (statusButtons.Count > 0) {
								statusButtons [target].targetState.Change ("target");
								targetSet = true;
						} else {
								targetSet = true;
						}
				}

				public void ReadTweets (string source)
				{
						tr = new TwitterReader ("justin_tweets.json");
						if (tr != null) {
								foreach (Tweet s in tr.statuses) {
										AddStatus (s);
								}
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

						GameObject go = (GameObject)Instantiate (Resources.Load ("TwitterListStatus"));
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
		
				public void TargetSet (TwitterStatusButton activeButton)
				{
						targetedButton = activeButton;
						foreach (TwitterStatusButton s in statusButtons) {
								if (activeButton == null)
										s.targetState.Change ("base");
								else if (s.index != activeButton.index)
										s.targetState.Change ("base");
						}
						
						targetedButton = activeButton;
				}

				public void ResetAllColors ()
				{
						foreach (TwitterStatusButton sb in statusButtons) {
								sb.ResetColor ();
						}
				}

				public void ShowArrowKeys ()
				{
						TwitterStatusButton a = Radical.instance.activeTwitter;

						if (a != null && targetedButton != null) {
								int difference = targetedButton.index - a.index;

								upDisplay.level = -difference;
								downDisplay.level = difference;
								/*					if (upArrowInd != null) {
										upArrowInd.SetActive (a.index > targetedButton.index);
										upArrowInd.renderer.material.color = Color.green;
				}
								if (downArrowInd != null) {
										downArrowInd.SetActive (a.index < targetedButton.index);
										downArrowInd.renderer.material.color = Color.green;
						} */
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