using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace P1
{
		public class TwitterList : MonoBehaviour
		{
				TwitterReader tr;
				List<TwitterListStatus> statuses = new List<TwitterListStatus> ();

#region loop
		
				// Use this for initialization
				void Start ()
				{
						tr = new TwitterReader ("Assets/data/justin_tweets.json");
						foreach (TwitterStatus s in tr.statuses)
								AddStatus (s);
				}
		
				// Update is called once per frame
				void Update ()
				{
			
				}

#endregion

				void AddStatus (TwitterStatus s)
				{
						GameObject go = (GameObject)Instantiate (Resources.Load ("TwitterListStatus"));
						go.transform.parent = transform;
			
						TwitterListStatus status = go.GetComponent<TwitterListStatus> ();
						status.list = this;
						status.status = s;
						status.index = statuses.Count;

						statuses.Add (status);
				}
		}
}