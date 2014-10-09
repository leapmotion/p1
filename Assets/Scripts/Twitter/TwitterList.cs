using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace P1
{
		public class TwitterList : MonoBehaviour
		{
				TwitterReader tr;
				public GameObject items;
				List<TwitterListStatus> statuses = new List<TwitterListStatus> ();
				const int MAX_TWEETS = 7;
				bool targetSet = false;
#region loop
		
				// Use this for initialization
				void Start ()
				{
						if (!StateList.HasList ("listTriggerState"))
								InitListTriggerState ();
						tr = new TwitterReader ("Assets/data/justin_tweets.json");
						foreach (TwitterStatus s in tr.statuses) {
								AddStatus (s);
						}
				}
		
				// Update is called once per frame
				void Update ()
				{
						if (!targetSet) {
								targetSet = true;
								statuses [Random.Range (0, statuses.Count - 1)].state.Change ("target");
						}
				}

#endregion

				void InitListTriggerState ()
				{
						new StateList ("listTriggerState", "base", "scrolling");
				}

				void AddStatus (TwitterStatus s)
				{
						if (statuses.Count >= MAX_TWEETS)
								return;
						GameObject go = (GameObject)Instantiate (Resources.Load ("TwitterListStatus"));
						go.transform.parent = items.transform;
			
						TwitterListStatus status = go.GetComponent<TwitterListStatus> ();
						status.list = this;
						status.status = s;
						status.index = statuses.Count;

						statuses.Add (status);
				}

				public TwitterListStatus PrevStatus (TwitterListStatus s)
				{
						if (s.index <= 0) {
								return null;
						}
						return statuses [s.index - 1];
				}

				public void TargetSet (TwitterListStatus status)
				{
						foreach (TwitterListStatus s in statuses) {
								if (s.index != status.index)
										s.state.Change ("base");
						}
				}
		}
}