using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace P1
{
		public class TwitterList : MonoBehaviour
		{
				TwitterReader tr;
				public GameObject items;
				public List<TwitterStatusButton> statusButtons = new List<TwitterStatusButton> ();
				const int MAX_TWEETS = 7;
				bool targetSet = false;
				public string tweetSource;
				const string TWITTER_LIST_TARGET_STATE = "listTriggerState";
				bool inEditor = false;
				const float MOVE_SCALE_COUNTER = 300f;
				const float MOVE_SCALE = 300f;
				const float FRICTION = 0.97f;

#region loop
		
				// Use this for initialization
				void Start ()
				{
						InitState ();
						if (tweetSource != "")
								ReadTweets (tweetSource);
				}
		
				// Update is called once per frame
				void Update ()
				{
						if (!targetSet) {
								SetRandomTarget ();
						}
						rigidbody.velocity *= FRICTION;
				}

#endregion

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
			
			inEditor = true;
						#endif

						if (!inEditor) {
								go = new GameObject ();
								go.AddComponent ("TwitterStatusButton");
						}
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