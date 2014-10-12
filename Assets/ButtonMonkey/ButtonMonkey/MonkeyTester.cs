using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using SimpleJSON;
using P1;

namespace ButtonMonkey
{
		public struct Trial
		{
				public MonkeyCounter counter;
				public List<int> keys;

				public override string ToString ()
				{
						string report = "";
						if (keys.Count > 0) {
								report += "Trial Keys:, ";
								foreach (int k in keys) {
										report += k.ToString () + ", ";
								}
								report += "\n" + counter.ToString () + "\n";
						}
						return report;
				}
		}

		public class MonkeyTester
		{
				Stopwatch timer;
				// Single Trial Variables
				Trial trial;
				float current;
				bool correct;
				int step;
				// Multiple Trial Records
				List<Trial> history;
				string recordPath;
				int test;
				int testNum;

				// Initialize to completed state
				public MonkeyTester ()
				{
						timer = new Stopwatch ();
						// Initialize to Empty Trial
						trial = new Trial ();
						current = 0.0f;
						correct = false;
						step = 0;
						history = new List<Trial> ();
						recordPath = "";
						test = 0;
						testNum = 0;
						Initialize ();
						//ASSERT: StageComplete() == true
						//ASSERT: TrialComplete() == true
				}

				// Configured generation of target keys
				protected virtual List<int> GenerateKeys ()
				{
						return new List<int> ();
				}
			
				protected JSONNode config;

				public void ConfigureTest (string testName)
				{
						//config = JSONNode.Parse (File.ReadAllText (dataPath + "/config/" + testName + "_config.json"));
            config = Utils.FileToJSON(testName);
						testNum = config ["test"] ["number"].AsInt;
						Directory.CreateDirectory ("/TestResults/");
						recordPath = "/TestResults/" + string.Format (testName + "-{0:yyyy-MM-dd_hh-mm-ss-tt}.csv", System.DateTime.Now);
						File.WriteAllText (recordPath, "No Data from Trials");
				}
				
				void Initialize ()
				{
						step = 0;
						trial.counter = new MonkeyCounter ();
						if (config != null) {
								trial.keys = GenerateKeys ();
								trial.counter.ChangeTarget (trial.keys [step].ToString () [0]);
						} else {
								trial.keys = new List<int> ();
						}
				}

				public void Start ()
				{
						Initialize ();
					
						// Begin test
						current = 0.0f;
						timer.Reset ();
						timer.Start ();
				}

				//Events are broadcast AFTER trial state has been updated
				public delegate void TrialUpdate (MonkeyTester trial);

				public event TrialUpdate TrialEvent;

				public void WhenPushed (bool complete, char label)
				{
						UnityEngine.Debug.Log ("MonkeyTester.WhenPushed label = " + label);
						if (StageComplete () ||
								TrialComplete ()) {
								//Already complete -> no event
								return;
						}

						current = timer.ElapsedMilliseconds / 1000.0f;
						trial.counter.WhenPushed (complete, label, current);

						if (complete) {
								if (label == trial.keys [step].ToString () [0]) {
										correct = true;
										step += 1;
										if (TrialComplete ()) {
												trial.counter.CommitTrial ();
												history.Add (trial);
												test += 1;
												// Do not immediately start next test
										} else {
												trial.counter.ChangeTarget (trial.keys [step].ToString () [0]);
										}
								} else {
										correct = false;
								}
			
								// Only send events for complete button pushes
								if (TrialEvent != null) {
										TrialEvent (this);
								}
						}

						// Update test results immediately
						if (recordPath.Length > 0) {
								File.WriteAllText (recordPath, this.ToString ());
						}
				}

				public string GetTrialKeys ()
				{
						string trialKeys = "";
						for (int k = 0; k < trial.keys.Count; k += 1) {
								trialKeys += trial.keys [k].ToString () [0];
						}
						return trialKeys;
				}

				public int GetTrialStep ()
				{
						return step;
				}

				public bool WasCorrect ()
				{
						return correct;
				}

				public float WasAtTime ()
				{
						return current;
				}

				public bool TrialComplete ()
				{
						return step >= trial.keys.Count;
				}

				public bool StageComplete ()
				{
						return test >= testNum;
				}
		
				//Print results in CSV format
				public override string ToString ()
				{
						string report = "";
						foreach (Trial h in history) {
								report += h.ToString ();
						}
						if (!TrialComplete ()) {
								report += trial.ToString ();
						}
						return report;
				}
		}
}

