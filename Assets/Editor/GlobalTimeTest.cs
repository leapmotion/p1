using UnityEngine;
using System.Collections;
using NUnit.Framework;

namespace ButtonMonkey
{
		public class GlobalTimeTest
		{
				[Test]
				public void TimeReflection ()
				{
						GlobalTime t = new GlobalTime ();
						t.now = 0.1f;
						Assert.AreEqual (0.1f, t.now, "GlobalTime Accepts time input");
				}

		
		[Test]
		public void TimeUpdate ()
		{
			GlobalTime t = new GlobalTime ();
			t.now = 0.1f;
			Assert.AreEqual (0.1f, t.now, "GlobalTime Accepts time input");
			t.now = 1.0f;
			Assert.AreEqual (1.0f, t.now, "GlobalTime Updated");
		}
		}
}