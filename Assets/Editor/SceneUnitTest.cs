using UnityEngine;
using NUnit.Framework;
using System.Collections.Generic;

namespace P1
{
	public class SceneUnitTest
	{

		CameraManager newScene(){
			
			GameObject g = new GameObject ();
			g.AddComponent ("CameraManager");
			return g.GetComponent<CameraManager> ();
}

		#region config
		
		[Test]
		public void SceneConfigFirstItemTest ()
		{
			CameraManager s = newScene();
			s.LoadScenes();

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
			CameraManager s = newScene();
      s.LoadScenes();
			Assert.AreEqual (3, s.scenes.Count, "has three scenes");
		}

		#endregion

		#region currentIndex
		
		[Test]
		
		public void CurrentIndexTest ()
		{
			CameraManager s = newScene();
      s.LoadScenes();
			Assert.AreEqual(0, s.currentIndex, "current index starts at 0");
		}
		#endregion
		
		#region next
		
		[Test]
		
		public void NextTest ()
		{
			GameObject g = new GameObject ();
			g.AddComponent ("CameraManager");
			CameraManager s = g.GetComponent<CameraManager> ();
      s.LoadScenes();
			s.NextScene();
			Assert.AreEqual(1, s.currentIndex, "current index is 1 after next");
		}
		
		[Test]
		
		public void NextContentTest ()
		{
			CameraManager s = newScene();
      s.LoadScenes();
			s.NextScene();
			Assert.AreEqual(s.currentScene, s.scenes[1], "current scenes is the next scene after next");
		}
		
		
		[Test]
		
		public void NextOverflowTest ()
		{
			CameraManager s = newScene();
      s.LoadScenes();
      s.LoadScenes();
			s.NextScene();
			s.NextScene();
			s.NextScene();
			s.NextScene();
			s.NextScene();
			s.NextScene();
			s.NextScene();
			s.PrevScene();
			s.NextScene();
			s.NextScene();
			Assert.AreEqual (s.currentScene, s.scenes [s.scenes.Count - 1], "current scene is the last one after overflow");
		}
		
		#endregion

		#region prev
		
		[Test]
		
		public void PrevTest ()
		{
			CameraManager s = newScene();
      s.LoadScenes();
			s.PrevScene();
			Assert.AreEqual(0, s.currentIndex, "current index is 0 after prev");
		}
		
		[Test]
		
		public void PrevContentTest ()
		{
			CameraManager s = newScene();
      s.LoadScenes();
			s.PrevScene();
			Assert.AreEqual(s.currentScene, s.scenes[0], "current scenes is the first scene after prev");
		}
		
		
		public void NextPrevTest ()
		{
			CameraManager s = newScene();
      s.LoadScenes();
			s.NextScene();
			s.NextScene();
			s.PrevScene();
			Assert.AreEqual(1, s.currentIndex, "current index is 1 after next next prev");
		}
		
		[Test]
		
		public void NextPrevContentTest ()
		{
			CameraManager s = newScene();
      s.LoadScenes();
			s.NextScene();
			s.NextScene();
			s.PrevScene();
			Assert.AreEqual(s.currentScene, s.scenes[1], "current scenes is the second scene after next next prev");
		}

		#endregion
		
	}
}
