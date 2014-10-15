using UnityEngine;
using System.Collections;
using SimpleJSON;
using System.Collections.Generic;

namespace P1
{
		public class TwitterReader
		{

				public List<Tweet> statuses = new List<Tweet> ();

		#region constructor
		
				public TwitterReader (string configFilePath)
				{
						JSONNode n = Utils.FileToJSON (configFilePath, Application.dataPath + "/data/");
						for (int i = 0; i < n["statuses"].Count; ++i) {
								AddStatus (n ["statuses"] [i]);
				
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