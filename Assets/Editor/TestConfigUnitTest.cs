using UnityEngine;
using NUnit.Framework;
using System.Collections.Generic;

namespace ButtonMonkey
{
		class TestConfigUnitTest
		{

			#region state

				[Test]

				public void TrialConfigLoaderTest ()
				{
						TrialConfigLoader l = new TrialConfigLoader ("Assets/testFiles/config/trial_config.json");
						Assert.AreEqual (7, l.testDigits, "seven test digits");
						Assert.AreEqual (9, l.trialCount, "nine trials");
				}
		
		#endregion
		
		}
}
