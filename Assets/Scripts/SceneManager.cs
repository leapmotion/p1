using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using System.Text.RegularExpressions;

namespace P1
{
		public class SceneManager : MonoBehaviour
		{
				SceneManager instance;
				Texture2D texture_;
				float creationTime;
				private float timer_threshold_ = 2.0f;
				public List<string> scenes;
				Regex q = new Regex ("(.*)");
				private string currentScene_;

				public string currentScene {
						get { return currentScene_;}
						set {
								currentScene_ = value; 
								creationTime = Time.time;
								sceneLoaded_ = true;
						}
				}

				private bool sceneLoaded_ = false;

				// Use this for initialization
				void Start ()
				{
						DoStart ();
						InitializeGUI ();
				}

				public void DoStart ()
				{ 
						scenes = new List<string> ();
						JSONNode n = Utils.FileToJSON ("Assets/config/scene_config.json");
						if (n == null)
								throw new UnityException ("No data");
						for (int i = 0; i < n["scenes"].Count; ++i) {
								scenes.Add (n ["scenes"] [i].Value);
						}
						if (Application.isPlaying)
								currentScene_ = Application.loadedLevelName;
						else
								currentScene_ = scenes [0];
						instance = this;
						creationTime = Time.time;
				}

				void InitializeGUI ()
				{
						Rect pixelInset = new Rect (0, 0, Screen.width, Screen.height);
						guiTexture.color = Color.black;
						guiTexture.pixelInset = pixelInset;
				}
	
				// Update is called once per frame
				void Update ()
				{
						// Implement Fading for easier transition
						if (Input.GetKeyDown (KeyCode.RightArrow)) {
								Debug.Log ("Going to next");
								Next ();
						} else if (Input.GetKeyDown (KeyCode.LeftArrow)) {
								Debug.Log ("Going to prev");
								Prev ();
						}

						if (sceneLoaded_ && ((Time.time - creationTime) / timer_threshold_) > 1.0f) {
								Application.LoadLevel (currentScene);
						}
				}
		
				public void Next ()
				{
						int i = currentIndex;
						if (i < scenes.Count - 1)
								currentScene = scenes [i + 1];
				}
		
				public void Prev ()
				{
						int i = currentIndex;
						if (i > 0)
								currentScene = scenes [i - 1];
				}

				public int currentIndex {
						get {
								int i = 0;
								while (i < scenes.Count) {
										if (scenes [i] == currentScene)
												return i;
										++i;
								}
								return -1;
						}
				}

				void OnGUI ()
				{
						float alpha = 1.0f - Mathf.Clamp01 ((Time.time - creationTime) / timer_threshold_);
						if (sceneLoaded_) {
								alpha = 1.0f - alpha;
						}
						Color color = guiTexture.color;
						color.a = alpha;
						guiTexture.color = color;
				}
		}
}
