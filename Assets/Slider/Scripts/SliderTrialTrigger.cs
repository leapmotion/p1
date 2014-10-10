using UnityEngine;
using System.Collections;

using System.IO;
using System.Text;

using SimpleJSON;

using ButtonMonkey;

namespace P1
{
	
	public class SliderTrialTrigger : MonoBehaviour {
	
		int test;
		string testPath = ""; //DEFAULT: Record in TestResults
		int testNum = 1; //DEFAULT: Run one trial
		ButtonTrial monkeyDo;
		public GameObject pinPrompt;
		
		SliderTrialTrigger() {
			monkeyDo = new ButtonTrial();
			monkeyDo.TrialEvent += TrialUpdate;
			
		}
		

		
		public void InintializeSliderTrial (){
		}
	
		// Called once for each key pushed
		void  TrialUpdate (ButtonTrial trial, bool correct)
		{
			if (trial.IsComplete ()) {
				if (test < testNum) {
					if (test > 0) {
						pinPrompt.GetComponent<PINPrompt> ().TogglePIN (true);
					}
					// Initial instructions
					test += 1;
					monkeyDo.Start ();
					Debug.Log ("Monkey, type: " + monkeyDo.GetTrialKeys ());
					//pinPrompt.GetComponent<PINPrompt> ().UpdatePIN (monkeyDo.GetTrialKeys ());
					pinPrompt.GetComponent<PINPrompt> ().UpdatePIN ("1");
				} else {
					pinPrompt.GetComponent<PINPrompt> ().TogglePIN (true);
					Debug.Log ("Autopsy report for monkey:\n" + monkeyDo.ToString ());
					string path = Application.dataPath + "/TestResults/" + testPath;
					Directory.CreateDirectory (path);
					path += string.Format ("ButtonTest-{0:yyyy-MM-dd_hh-mm-ss-tt}.csv", System.DateTime.Now);
					File.WriteAllText (path, monkeyDo.ToString ());
					Debug.Log ("Autopsy report written to: " + path);
					
					//TODO: Applaud Monkey *IN-SCENE*
					if (SceneManager.instance)
					{
						SceneManager.instance.Next();
					}
				}
			} else {
				if (monkeyDo.WasCorrect ()) {
					pinPrompt.GetComponent<PINPrompt> ().TogglePIN (true);
					Debug.Log ("Good monkey! Next, type: " + monkeyDo.GetTargetKey ());
				} else {
					pinPrompt.GetComponent<PINPrompt> ().TogglePIN (false);
					Debug.Log ("Bad monkey! You were told to type: " + monkeyDo.GetTargetKey ());
				}
			}
		}
		bool isHandInTestTrigger = false;
		void OnTriggerEnter (){
			// if its a hand
			//start or advance test
		}
		
		void OnTriggerStay () {
		}
		
		void OnTriggerExit () {
			//if the hand leaves
			//end and/or advance test
		}
		
		
		public void SetTestFromConfig (string filePath)
		{
			JSONNode data = Utils.FileToJSON (filePath);
			testPath = data ["results_dir"].ToString ();
			// NOTE: JSONNode ToString helpfully interprets both path/ (no quotes in file) and "path/" (quotes in file)
			// as "path/" (quotes IN string).
			testPath = testPath.Substring (1, testPath.Length - 2);
			testNum = data ["trial_count"].AsInt;
		}
		
		void Update (){
			if(Input.GetKey(KeyCode.T)){
				TrialUpdate(monkeyDo, true);
			}
		}
		
	}
}
