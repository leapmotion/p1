using UnityEngine;
using NUnit.Framework;
using System.Collections.Generic;

namespace P1
{
	public class SceneUnitTest
	{

		SceneManager newScene(){
			
			GameObject g = new GameObject ();
			g.AddComponent ("SceneManager");
			return g.GetComponent<SceneManager> ();
}

		#region config
		
		[Test]
		public void SceneConfigFirstItemTest ()
		{
			SceneManager s = newScene();
			s.DoStart ();

			Assert.AreEqual ("Stage1", s.currentScene, "First scene is Stage1");
			bool firstInScenes = false;
			foreach (string ss in s.scenes)
								if (ss == s.currentScene)
										firstInScenes = true;
			Assert.IsTrue (firstInScenes, "first scene is in scenes");
		}
		
		
		[Test]
		public void SceneConfigScenesTest ()
		{
			SceneManager s = newScene();
			s.DoStart ();
			Assert.AreEqual (3, s.scenes.Count, "has three scenes");
		}

		#endregion

		#region currentIndex
		
		[Test]
		
		public void CurrentIndexTest ()
		{
			SceneManager s = newScene();
			s.DoStart ();
			Assert.AreEqual(0, s.currentIndex, "current index starts at 0");
		}
		#endregion
		
		#region next
		
		[Test]
		
		public void NextTest ()
		{
			GameObject g = new GameObject ();
			g.AddComponent ("SceneManager");
			SceneManager s = g.GetComponent<SceneManager> ();
			s.DoStart ();
			s.Next ();
			Assert.AreEqual(1, s.currentIndex, "current index is 1 after next");
		}
		
		[Test]
		
		public void NextContentTest ()
		{
			SceneManager s = newScene();
			s.DoStart ();
			s.Next ();
			Assert.AreEqual(s.currentScene, s.scenes[1], "current scenes is the next scene after next");
		}
		
		
		[Test]
		
		public void NextOverflowTest ()
		{
			SceneManager s = newScene();
			s.DoStart ();
			s.DoStart ();
			s.Next ();
			s.Next ();
			s.Next ();
			s.Next ();
			s.Next ();
			s.Next ();
			s.Next ();
			s.Prev ();
			s.Next ();
			s.Next ();
			Assert.AreEqual (s.currentScene, s.scenes [s.scenes.Count - 1], "current scene is the last one after overflow");
		}
		
		#endregion

		#region prev
		
		[Test]
		
		public void PrevTest ()
		{
			SceneManager s = newScene();
			s.DoStart ();
			s.Prev ();
			Assert.AreEqual(0, s.currentIndex, "current index is 0 after prev");
		}
		
		[Test]
		
		public void PrevContentTest ()
		{
			SceneManager s = newScene();
			s.DoStart ();
			s.Prev ();
			Assert.AreEqual(s.currentScene, s.scenes[0], "current scenes is the first scene after prev");
		}
		
		
		public void NextPrevTest ()
		{
			SceneManager s = newScene();
			s.DoStart ();
			s.Next ();
			s.Next ();
			s.Prev ();
			Assert.AreEqual(1, s.currentIndex, "current index is 1 after next next prev");
		}
		
		[Test]
		
		public void NextPrevContentTest ()
		{
			SceneManager s = newScene();
			s.DoStart ();
			s.Next ();
			s.Next ();
			s.Prev ();
			Assert.AreEqual(s.currentScene, s.scenes[1], "current scenes is the second scene after next next prev");
		}

		#endregion
		
	}
}
