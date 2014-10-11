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
		SliderMonkey monkeyDo;
		public GameObject pinPrompt;
		
		SliderTrialTrigger() {
			monkeyDo = new SliderMonkey();
			monkeyDo.TrialEvent += TrialUpdate;
			
		}
		
		public void Start() {
			DoStart();
		}
	
		public void DoStart ()
		{
			//if (grid == null) {	
			//    grid = GetComponent<GFRectGrid> ();
			//}
			//SetGridFromConfig ("Assets/config/grid_config.json");
			
			monkeyDo.ConfigureTest (Application.dataPath, "slider");
			monkeyDo.TrialEvent += TrialUpdate;
			
			monkeyDo.Start ();
			Debug.Log ("Monkey, type: " + monkeyDo.GetTrialKeys ());
			pinPrompt.GetComponent<PINPrompt> ().UpdatePIN (monkeyDo.GetTrialKeys ());
		}
		
		// Called once for each key pushed
		void  TrialUpdate (ButtonTrial trial)
		{
			if (monkeyDo.StageComplete ()) {
				// Show final correct result
				pinPrompt.GetComponent<PINPrompt> ().TogglePIN (true);
				Debug.Log ("Autopsy report for monkey:\n" + monkeyDo.ToString ());
				
				if (SceneManager.instance) {
					SceneManager.instance.Next ();
				}
			} else {
				if (monkeyDo.TrialComplete ()) {
					// Show final correct result
					pinPrompt.GetComponent<PINPrompt> ().TogglePIN (true);
					
					monkeyDo.Start ();
					Debug.Log ("Monkey, type: " + monkeyDo.GetTrialKeys ());
					pinPrompt.GetComponent<PINPrompt> ().UpdatePIN (monkeyDo.GetTrialKeys ());
				} else {
					if (monkeyDo.WasCorrect ()) {
						Debug.Log ("Good monkey! Next, type: " + monkeyDo.GetTrialKeys () [monkeyDo.GetTrialStep ()]);
						pinPrompt.GetComponent<PINPrompt> ().TogglePIN (true);
					} else {
						Debug.Log ("Bad monkey! You were told to type: " + monkeyDo.GetTrialKeys () [monkeyDo.GetTrialStep ()]);
						pinPrompt.GetComponent<PINPrompt> ().TogglePIN (false);
					}
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
//			Debug.Log ("Update");
			if(Input.GetKey(KeyCode.U)){
				Debug.Log ("U");
				monkeyDo.WhenPushed (true, '0');
				monkeyDo.WhenPushed (true, '1');
				monkeyDo.WhenPushed (true, '2');
				monkeyDo.WhenPushed (true, '3');
				monkeyDo.WhenPushed (true, '4');
				monkeyDo.WhenPushed (true, '5');
				monkeyDo.WhenPushed (true, '6');
				monkeyDo.WhenPushed (true, '7');
				monkeyDo.WhenPushed (true, '8');
				monkeyDo.WhenPushed (true, '9');
			}
		}
		
	}
}
