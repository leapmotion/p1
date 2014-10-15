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
								report += "Trial Keys: ";
								foreach (int k in keys) {
										report += k.ToString () + ".";
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
				string recordFile;
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
						recordFile = "";
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
			
				protected JSONNode testConfig;

				/// <summary>
				/// Configures the test using ./Assets/config/<test>_config.json
				/// Stores test results in ./TestResults/<userName>/<test>-<year>-<month>-<day>_<hour>-<minute>-<second>-<AM/PM>.csv
				/// Copies all configurations into same directory as test results.
				/// </summary>
				public void ConfigureTest (string testName)
				{
						//(1) Create unique record directory
						//JSONNode userConfig = JSONNode.Parse (File.ReadAllText (Environment.CurrentDirectory + "/config/user_config.json"));
						JSONNode userConfig = Utils.FileToJSON ("user_config.json");
						string recordPath = Environment.CurrentDirectory + "/TestResults/" + userConfig ["userName"].Value + "/";
						Directory.CreateDirectory (recordPath);

						//(2) Copy configurations into directory
						string configPath = Environment.CurrentDirectory + "/config/";
						string[] files = Directory.GetFiles (configPath);
						foreach (string f in files) {
								string configName = Path.GetFileName (f);
								string recordName = Path.Combine (recordPath, configName);
								System.IO.File.Copy (f, recordName, true);
						}
			
						//(3) Read test configuration
						//testConfig = JSONNode.Parse (File.ReadAllText (Environment.CurrentDirectory + "/Assets/config/" + testName + "_config.json"));
						testConfig = Utils.FileToJSON (testName + "_config.json");
						testNum = testConfig ["test"] ["number"].AsInt;

						//(4) Create test file
						recordFile = recordPath + string.Format (testName + "-{0:yyyy-MM-dd_hh-mm-ss-tt}.csv", System.DateTime.Now);
						File.WriteAllText (recordFile, "No Data from Trials");
				}
				
				void Initialize ()
				{
						step = 0;
						trial.counter = new MonkeyCounter ();
						if (testConfig != null) {
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
						if (recordFile.Length > 0) {
								File.WriteAllText (recordFile, this.ToString ());
						}
				}
		
				public List<int> GetTrialKeys ()
				{
						return trial.keys;
				}

				public string GetTrialKeysString ()
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

