using UnityEngine;
using NUnit.Framework;
using System.Collections.Generic;

namespace P1
{
		class TwitterReaderUnitTest
		{

		#region state
		
				[Test]
		
				public void ReaderUnitTest ()
				{
						TwitterReader tr = new TwitterReader ("Assets/testFiles/data/justin_tweets.json");
						Assert.AreEqual (6, tr.statuses.Count, "has five statuses");
			
				}
		
				[Test]
		
				public void ReaderContentUnitTest ()
				{
						TwitterReader tr = new TwitterReader ("Assets/testFiles/data/justin_tweets.json");
			Assert.AreEqual ("RT @JeremyBieber: today we explore this ancient world #blessed",
			                 tr.statuses [0].text, "text of first status");
				}
		
		#endregion

#region date
		
		
				[Test]
		
				public void ReaderDateUnitTest ()
				{
						TwitterReader tr = new TwitterReader ("Assets/testFiles/data/justin_tweets.json");
						Assert.AreEqual (10, tr.statuses [0].time.Month, "First tweet is february");
						Assert.AreEqual (8, tr.statuses [0].time.Day, "First tweet is on the sixth");
						Assert.AreEqual (2014, tr.statuses [0].time.Year, "First tweet is at 2012");
			
				}
		
				[Test]
		
				public void MonthUnitTest ()
				{
						Assert.AreEqual (3, TwitterStatus.StrToMonth ("Mar"), "march is the third month");
			
				}
#endregion
		
		}
}
