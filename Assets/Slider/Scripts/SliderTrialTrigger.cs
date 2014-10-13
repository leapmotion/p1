using UnityEngine;
using System.Collections;

using System.IO;
using System.Text;

using SimpleJSON;

using ButtonMonkey;

namespace P1
{
	
	public class SliderTrialTrigger : MonoBehaviour {
	
		SliderMonkey monkeyDo;
		public GameObject pinPrompt;
		private SliderManager sliderManager;
		private SliderDragger sliderDragger;
		private int prevHandCount = 0;
		
		SliderTrialTrigger() {
			monkeyDo = new SliderMonkey();
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
			monkeyDo.ConfigureTest ("slider_config.json");
			monkeyDo.TrialEvent += TrialUpdate;
			
			monkeyDo.Start ();
			Debug.Log ("Monkey, type: " + monkeyDo.GetTrialKeys ());
			pinPrompt.GetComponent<PINPrompt> ().UpdatePIN (monkeyDo.GetTrialKeys ());
		}
		
		// Called once for each key pushed
		void  TrialUpdate (MonkeyTester trial)
		{
			if (monkeyDo.StageComplete ()) {
				// Show final correct result
				pinPrompt.GetComponent<PINPrompt> ().TogglePIN (true);
				Debug.Log ("Autopsy report for monkey:\n" + monkeyDo.ToString ());
				
				if (CameraManager.instance) {
					CameraManager.instance.NextScene();
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
//		bool isHandInTestTrigger = false;
//		int colliderCount = 0;
//		bool isHandNearSlider = false;
//		void OnTriggerEnter (){
////			Debug.Log ("OnTriggerEnter isHandNearSlider = " + isHandNearSlider);
//			
//			isHandNearSlider = true;
//			colliderCount++;
//			// if its a hand
//			//start or advance test
//		}
//		
//		void OnTriggerStay () {
//		}
//		
//		void OnTriggerExit () {
//			colliderCount--;
//			if(colliderCount == 0 && isHandNearSlider == true){
//				Debug.Log ("OnTriggerExit isHandNearSlider = " + isHandNearSlider);
//				isHandNearSlider = false;
//				StepThroughTrial();
//
//			}
//		}
		
		void StepThroughTrial (){
			char x = sliderDragger.sliderInt.ToString()[0];
			Debug.Log ("char x = " + x);
			monkeyDo.WhenPushed (true, sliderDragger.sliderInt.ToString()[0]);
			sliderDragger.sliderInt = 0;
			sliderManager.SliderHandleGRP.transform.localPosition = new Vector3 (0.0f, sliderManager.SliderHandleGRP.transform.localPosition.y, sliderManager.SliderHandleGRP.transform.localPosition.z);
		}
		public HandController theHands;
		
		void Update (){
			//@Frame (Current frame)
			if (prevHandCount > 0 && theHands.GetFrame().Hands.Count == 0) {
				// Trigger the function to check if slider is at right spot
				Debug.Log ("Triggering StepThroughTrial");
				StepThroughTrial();
			}
			
			//@Frame - 1 (Last frame)
			prevHandCount = theHands.GetFrame().Hands.Count;
//			Debug.Log ("prevHandCount = " + prevHandCount);
//			if(Input.GetKeyUp(KeyCode.T)){
//				Debug.Log ("sliderInt = " + sliderDragger.sliderInt);
//				char x = sliderDragger.sliderInt.ToString()[0];
//				Debug.Log ("char x = " + x);
//				monkeyDo.WhenPushed (true, sliderDragger.sliderInt.ToString()[0]);
//			}

		}
		
	}
}
