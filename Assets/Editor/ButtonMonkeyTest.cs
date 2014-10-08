using UnityEngine;
using NUnit.Framework;
using System.Collections.Generic;

namespace ButtonMonkey
{
	public class ButtonMonkeyTest
	{
		#region ButtonItem
		[Test]
		/// <summary>
		/// Verify that initial condition includes no successes
		/// </summary>
		public void InitialExpectationsTest ()
		{
			ButtonCounter monkeyDo = new ButtonCounter();
			Assert.AreEqual (0, monkeyDo.CurrentAttemptsCount);
			Assert.AreEqual (0, monkeyDo.CompletedTrialsCount);
			Assert.AreEqual (0, monkeyDo.CurrentSuccessCount);
			Assert.AreEqual (0, monkeyDo.CompletedSuccessCount);
		}

		[Test]
		/// <summary>
		/// No Target Set => Record Possible
		/// </summary>
		public void NoTargetTest ()
		{
			ButtonCounter monkeyDo = new ButtonCounter();

			monkeyDo.WhenPushed (true, '0', 0.0f);

			Assert.AreEqual (0, monkeyDo.CurrentAttemptsCount,"Cannot record attempts before target is set");
			Assert.AreEqual (0, monkeyDo.CompletedTrialsCount);
			Assert.AreEqual (0, monkeyDo.CurrentSuccessCount);
			Assert.AreEqual (0, monkeyDo.CompletedSuccessCount);
		}
		
		[Test]
		/// <summary>
		/// Verify failure
		/// </summary>
		public void FailureTest ()
		{
			ButtonCounter monkeyDo = new ButtonCounter();
			
			monkeyDo.ChangeTarget('0');
			monkeyDo.WhenPushed (true, '1', 0.0f);
			
			Assert.AreEqual (1, monkeyDo.CurrentAttemptsCount);
			Assert.AreEqual (0, monkeyDo.CompletedTrialsCount);
			Assert.AreEqual (0, monkeyDo.CurrentSuccessCount);
			Assert.AreEqual (0, monkeyDo.CompletedSuccessCount);
		}
		
		[Test]
		/// <summary>
		/// Verify failure followed by success
		/// </summary>
		public void FailureSuccessTest ()
		{
			ButtonCounter monkeyDo = new ButtonCounter();
			
			monkeyDo.ChangeTarget('0');
			monkeyDo.WhenPushed (true, '1', 0.0f);
			monkeyDo.WhenPushed (true, '0', 0.1f);
			
			Assert.AreEqual (2, monkeyDo.CurrentAttemptsCount);
			Assert.AreEqual (0, monkeyDo.CompletedTrialsCount);
			Assert.AreEqual (1, monkeyDo.CurrentSuccessCount);
			Assert.AreEqual (0, monkeyDo.CompletedSuccessCount);
		}
		
		[Test]
		/// <summary>
		/// Verify success
		/// </summary>
		public void SuccessTest ()
		{
			ButtonCounter monkeyDo = new ButtonCounter();
			
			monkeyDo.ChangeTarget('0');
			monkeyDo.WhenPushed (true, '0', 0.1f);
			
			Assert.AreEqual (1, monkeyDo.CurrentAttemptsCount);
			Assert.AreEqual (0, monkeyDo.CompletedTrialsCount);
			Assert.AreEqual (1, monkeyDo.CurrentSuccessCount);
			Assert.AreEqual (0, monkeyDo.CompletedSuccessCount);
		}
		
		[Test]
		/// <summary>
		/// Verify success
		/// </summary>
		public void SuccessFailureTest ()
		{
			ButtonCounter monkeyDo = new ButtonCounter();
			
			monkeyDo.ChangeTarget('0');
			monkeyDo.WhenPushed (true, '0', 0.0f);
			monkeyDo.WhenPushed (true, '1', 0.1f);
			
			Assert.AreEqual (2, monkeyDo.CurrentAttemptsCount);
			Assert.AreEqual (0, monkeyDo.CompletedTrialsCount);
			Assert.AreEqual (1, monkeyDo.CurrentSuccessCount);
			Assert.AreEqual (0, monkeyDo.CompletedSuccessCount);
		}
		
		#endregion

		#region TimingItem
		[Test]
		/// <summary>
		/// Verify time recorded with button pushed
		/// </summary>
		public void TimeReflectionTest ()
		{
			ButtonCounter monkeyDo = new ButtonCounter();
			
			monkeyDo.ChangeTarget('0');
			monkeyDo.WhenPushed (true, '0', 1.0f);
			
			Assert.AreEqual(1.0f,monkeyDo.GetTrial(0).time,"Correct time association");
		}
		
		#endregion
	}
}