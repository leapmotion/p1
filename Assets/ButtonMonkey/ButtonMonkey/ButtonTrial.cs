using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using SimpleJSON;

namespace ButtonMonkey
{
		public struct Trial
		{
				public List<int> keys;
				public ButtonCounter counter;

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

		public class ButtonTrial
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
				public ButtonTrial ()
				{
						timer = new Stopwatch ();
						// Initialize to Empty Trial
						trial = new Trial ();
						current = 0.0f;
						correct = false;
						step = 0;
						history = new List<Trial> ();
						recordPath = "";
						test = 1;
						testNum = 0;
						Initialize ();
				}

				void Initialize ()
				{
						step = 0;
						trial.keys = new List<int> ();
						trial.counter = new ButtonCounter ();
						//ASSERT: IsComplete() == true
				}

				//TODO: Move generation to a separate file (will be distinct for other tests).
				// Generate a random sequence of keys
				// subject to the condition that no row is repeated
				public void Start ()
				{
						Initialize ();

						System.Random gen = new System.Random ();
			
						// List a random key from each row
						List<int> rows = new List<int> ();
						rows.Add (1 + (gen.Next () % 3));
						rows.Add (4 + (gen.Next () % 3));
						rows.Add (7 + (gen.Next () % 3));
						rows.Add (0);
			
						// Choose a random ordering of rows
						trial.keys = new List<int> ();
						while (rows.Count > 0) {
								int next = gen.Next () % rows.Count;
								trial.keys.Add (rows [next]);
								rows.RemoveAt (next);
						}
						trial.counter.ChangeTarget (trial.keys [step].ToString () [0]);

						// Begin test
						current = 0.0f;
						timer.Reset ();
						timer.Start ();
				}
		
				public void SetTestFromConfig (string dataPath)
				{
						JSONNode config = JSONNode.Parse (File.ReadAllText (dataPath + "/config/test_config.json"));
						testNum = config ["trial_count"].AsInt;
						SetRecordFile (dataPath + "/TestResults/" + config ["results_dir"].Value, "ButtonTest");
				}

				public void SetRecordFile (string path, string name)
				{
						Directory.CreateDirectory (path);
						recordPath = path + "/" + name + string.Format ("-{0:yyyy-MM-dd_hh-mm-ss-tt}.csv", System.DateTime.Now);
						File.WriteAllText (recordPath, "No Data from Trials");
				}

				public string GetRecordPath ()
				{
						return recordPath;
				}

				//Events are broadcast AFTER trial state has been updated
				public delegate void TrialUpdate (ButtonTrial trial,bool correct);

				public event TrialUpdate TrialEvent;

				public void WhenPushed (bool complete, char label)
				{
						if (IsComplete ()) {
								//Already complete -> no event
								return;
						}

						current = timer.ElapsedMilliseconds / 1000.0f;
						trial.counter.WhenPushed (complete, label, current);

						UnityEngine.Debug.Log ("Monkey pushed " + label + " for a total push count of " + trial.counter.CurrentAttemptsCount);

						if (complete) {
								if (label == trial.keys [step].ToString () [0]) {
										correct = true;
										step += 1;
										if (IsComplete ()) {
												trial.counter.CommitTrial ();
												history.Add (trial);
												Initialize ();
										} else {
												trial.counter.ChangeTarget (trial.keys [step].ToString () [0]);
										}
								} else {
										correct = false;
								}
			
								if (TrialEvent != null) {
										TrialEvent (this, correct);
								}
						}
			
						if (recordPath.Length > 0) {
								// Record partial results
								File.WriteAllText (recordPath, this.ToString ());
						}
				}

				public char GetTargetKey ()
				{
						if (IsComplete ()) {
								return ' ';
						}
						return trial.keys [step].ToString () [0];
				}

				public string GetTrialKeys ()
				{
						string trialKeys = "";
						for (int k = 0; k < trial.keys.Count; k += 1) {
								trialKeys += trial.keys [k].ToString () [0];
						}
						return trialKeys;
				}

				public bool HasAttempt ()
				{
						return trial.counter.CurrentAttemptsCount > 0;
				}

				public bool WasCorrect ()
				{
						return correct;
				}

				public float WasAtTime ()
				{
						return current;
				}

				public bool IsComplete ()
				{
						return step >= trial.keys.Count;
				}
		
				//Print results in CSV format
				public override string ToString ()
				{
						string report = "";
						foreach (Trial h in history) {
								report += h.ToString ();
						}
						report += trial.ToString ();
						return report;
				}
		}
}

