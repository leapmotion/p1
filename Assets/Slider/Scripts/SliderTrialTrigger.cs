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
		ButtonTrial monkeyDo;
		public GameObject pinPrompt;
		private SliderManager sliderManager;
		private SliderDragger sliderDragger;
		
		
		
		SliderTrialTrigger() {
			monkeyDo = new ButtonTrial();
			monkeyDo.TrialEvent += TrialUpdate;

			
		}
		
		public void Start() {
			DoStart();
			sliderManager = (SliderManager)FindObjectOfType (typeof(SliderManager));
			if (sliderManager == null) {
				Debug.LogWarning ("You are missing a Slider Manager in the scene.");
			}
			sliderDragger =  (SliderDragger)FindObjectOfType (typeof(SliderDragger));
			if (sliderDragger == null) {
				Debug.LogWarning ("You are missing a Slider Dragger in the scene.");
			}
		}
		
		public void DoStart ()
		{
			monkeyDo = new ButtonTrial ();
			//if (grid == null) {	
			//    grid = GetComponent<GFRectGrid> ();
			//}
			//SetGridFromConfig ("Assets/config/grid_config.json");
			
			monkeyDo.SetTestFromConfig (Application.dataPath);
			monkeyDo.TrialEvent += TrialUpdate;
			
			monkeyDo.Start ();
			Debug.Log ("Monkey, type: " + monkeyDo.GetTrialKeys ());
			pinPrompt.GetComponent<PINPrompt> ().UpdatePIN (monkeyDo.GetTrialKeys ());
		}
		
		// Called once for each key pushed
		void  TrialUpdate (ButtonTrial trial, bool correct)
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
		}
		
		void Update (){
//			Debug.Log ("Update");
			if(Input.GetKeyUp(KeyCode.T)){
				Debug.Log ("sliderInt = " + sliderDragger.sliderInt);
				char x = sliderDragger.sliderInt.ToString()[0];
				Debug.Log ("char x = " + x);
				monkeyDo.WhenPushed (true, sliderDragger.sliderInt.ToString()[0]);
			}

		}
		
	}
}
