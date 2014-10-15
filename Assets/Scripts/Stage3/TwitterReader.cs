using UnityEngine;
using System.Collections;
using SimpleJSON;
using System.Collections.Generic;
using System.IO;

namespace P1
{
		public class TwitterReader
		{

				public List<Tweet> statuses = new List<Tweet> ();

		#region constructor

				const string STATUSES = "statuses";
		
				public TwitterReader (string configFilePath)
				{
						JSONNode n = Utils.FileToJSON (configFilePath);
						if (n [STATUSES] == null) {
								Debug.Log ("No statuses in " + n.ToString ());
						} else
								for (int i = 0; i < n[STATUSES].Count; ++i) {
//										Debug.Log ("Reading status " + i);
										AddStatus (n [STATUSES] [i]);
								}
				}

		#endregion

		#region input

				void AddStatus (JSONNode n)
				{
						statuses.Add (new Tweet (n));
				}

		#endregion

		}
}