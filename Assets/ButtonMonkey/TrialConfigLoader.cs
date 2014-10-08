using UnityEngine;
using System.Collections;
using P1;
using SimpleJSON;

namespace ButtonMonkey
{
		public class TrialConfigLoader
		{
				public int trialCount;
				public int testDigits;

#region constructors
		
				public TrialConfigLoader (string configFile)
				{
						JSONNode n = Utils.FileToJSON (configFile);
						trialCount = n ["trial_count"].AsInt;
						testDigits = n ["test_digts"].AsInt;
				}
		
				public TrialConfigLoader (): this("Assets/config/trial_config.json")
				{
			
				}
#endregion
		
		}
}