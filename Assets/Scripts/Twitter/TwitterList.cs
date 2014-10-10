using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace P1
{
		public class TwitterList : MonoBehaviour
		{
				TwitterReader tr;
				public GameObject items;
				List<TwitterStatusButton> statuses = new List<TwitterStatusButton> ();
				const int MAX_TWEETS = 7;
				bool targetSet = false;
				public string tweetSource;
				const string TWITTER_LIST_TARGET_STATE = "listTriggerState";

		#if UNITY_EDITOR || UNITY_STANDALONE
#endif

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
				}

#endregion

				public void InitState ()
				{
						if (!StateList.HasList (TWITTER_LIST_TARGET_STATE))
								InitListTriggerState ();
				}

				public void SetRandomTarget ()
				{
						statuses [Random.Range (0, statuses.Count - 1)].targetState.Change ("target");
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
						if (statuses.Count >= MAX_TWEETS)
								return;

						GameObject go;

						#if UNITY_EDITOR
			            go = (GameObject)Instantiate (Resources.Load ("TwitterListStatus"));
						#else 
						go = new GameObject ();
						go.AddComponent ("TwitterStatusButton");
						go.transform.parent = items.transform;
						#endif
			
						TwitterStatusButton status = go.GetComponent<TwitterStatusButton> ();
						status.list = this;
						status.status = s;
						status.index = statuses.Count;

						statuses.Add (status);
				}

				public TwitterStatusButton PrevStatus (TwitterStatusButton s)
				{
						if (s.index <= 0) {
								return null;
						}
						return statuses [s.index - 1];
				}

#region aggregate state changes
/**
 * these states can only be true for a single button;clears sets for other buttons
 */
		
				public void TargetSet (TwitterStatusButton status)
				{
						foreach (TwitterStatusButton s in statuses) {
								if (s.index != status.index)
										s.targetState.Change ("base");
						}
				}
		
				public void HoverSet (TwitterStatusButton status)
				{
						foreach (TwitterStatusButton s in statuses) {
								if (s.index != status.index)
										s.hoverState.Change ("base");
						}
				}

#endregion
		}
}