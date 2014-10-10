using UnityEngine;
using NUnit.Framework;
using System.Collections.Generic;

namespace P1
{
	public class BoneTrackerUnitTest
	{

		BoneTracker newBoneTracker(){
			
			GameObject g = new GameObject ();
			g.AddComponent ("BoneTracker");
			return g.GetComponent<BoneTracker> ();
}

		
		[Test]
		public void BTCreationTest ()
		{
			BoneTracker t = newBoneTracker();
			Assert.AreEqual(0, t.boneCount, "starts with zero bones");
		}
		
		[Test]
		public void BTboneEnterTest ()
		{
			BoneTracker t = newBoneTracker();
			t.TallyGameObject(new GameObject());
			Assert.AreEqual(0, t.boneCount, "if a non-proper name object found, still has zero bones");
		}
		
		[Test]
		public void BTboneEnterBone1Test ()
		{
			BoneTracker t = newBoneTracker();
			t.TallyGameObject(new GameObject("bone1"));
			Assert.AreEqual(1, t.boneCount, "if a bone object found, has 1 bones");
		}
		
		
		[Test]
		public void BTboneEnterBone2Test ()
		{
			BoneTracker t = newBoneTracker();
			t.TallyGameObject(new GameObject("bone2"));
			Assert.AreEqual(1, t.boneCount, "if a bone object found, has 1 bones");
		}
		
		[Test]
		public void BTboneEnterBone3Test ()
		{
			BoneTracker t = newBoneTracker();
			t.TallyGameObject(new GameObject("bone3"));
			Assert.AreEqual(1, t.boneCount, "if a bone object found, has 1 bones");
		}
		
#region  time dependant tests
		
		[Test]
		public void TimeBasedUpdatingTest ()
		{
			BoneTracker t = newBoneTracker();
			GameObject addedAtTimeZero = new GameObject("bone3");
			GameObject addedJustBeforeMaxTime = new GameObject("bone3");
			GameObject addedAtMaxTime = new GameObject("bone3");
			t.TallyGameObject(addedAtTimeZero);
			
//			Debug.Log ("R1: " + t.Report());
			Assert.AreEqual(1, t.boneCount, "R1 after first bone added, has 1 bones");

			t.time.now = BoneTracker.MAX_BONE_STALE_TIME - 0.0001f;
			t.TallyGameObject (addedJustBeforeMaxTime);
			t.RetireOldBones();
			
		//	Debug.Log ("R2: " + t.Report());
			Assert.AreEqual(2, t.boneCount, "R2 after second bone added, has 2 bones");
			
			t.time.now = BoneTracker.MAX_BONE_STALE_TIME;
			t.TallyGameObject (addedAtMaxTime);
			t.RetireOldBones();
			
		//	Debug.Log ("R3: " + t.Report());
			Assert.AreEqual(3, t.boneCount, "R3 after third bone added has 3 bones");
			
			t.time.now = BoneTracker.MAX_BONE_STALE_TIME + 0.0001f;
			t.RetireOldBones();
			
		//	Debug.Log ("R4: " + t.Report());
			Assert.AreEqual(2, t.boneCount, "R4 just after stale time has 2 bones");
			
			t.time.now = BoneTracker.MAX_BONE_STALE_TIME * 2;
			t.RetireOldBones();
			
		//	Debug.Log ("R5: " + t.Report());
			Assert.AreEqual(1, t.boneCount, "R5 after 2 x stale time has  1 bones");
			
			t.time.now = BoneTracker.MAX_BONE_STALE_TIME * 2 + 0.00001f;
			t.RetireOldBones();
			
		//	Debug.Log ("R6: " + t.Report());
			Assert.AreEqual(0, t.boneCount, "R6 after 2 x stale time + has  0 bones");
		}
		
#endregion
		
	}
}
