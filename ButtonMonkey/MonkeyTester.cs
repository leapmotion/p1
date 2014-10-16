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

		//Print results in Dorin accessibility mode
		public string ToDorin (bool isGrid)
		{
			string report = "";
			if (keys.Count > 0) {
				report += "Trial Keys: ";
				foreach (int k in keys) {
					report += k.ToString () + ".";
				}
				report += "\n" + counter.ToDorin (isGrid) + "\n";
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
		// Dorin accessibility mode
		static bool empowerDorin = true;
		protected bool isDorinGrid = false;

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
		/// Configures the test using ./config/<test>_config.json
		/// Stores test results in ./TestResults/<userName>/<testName>-<year>-<month>-<day>_<hour>-<minute>-<second>-<AM/PM>.csv
		/// Copies all configurations into same directory as test results.
		/// </summary>
		public void ConfigureTest (string testName)
		{
			//(1) Create user record directory
			string userConfigPath = Environment.CurrentDirectory + "/config/user_config.json";
			if (File.Exists (userConfigPath)) {
				JSONNode userConfig = JSONNode.Parse (File.ReadAllText (userConfigPath));
				string recordPath = Environment.CurrentDirectory + "/TestResults/" + userConfig ["userName"].Value + "/";
				Directory.CreateDirectory (recordPath);
				
				//(2) Copy configurations into user record directory
				string configPath = Environment.CurrentDirectory + "/config/";
				string[] files = Directory.GetFiles (configPath);
				foreach (string f in files) {
					string configName = Path.GetFileName (f);
					string recordName = Path.Combine (recordPath, configName);
					// Ignore if the file is .meta
					if (configName.Substring (configName.Length - 5) != ".meta")
						continue;
					System.IO.File.Copy (f, recordName, true);
				}

				//(3) Create test record
				recordFile = recordPath + string.Format (testName + "-{0:yyyy-MM-dd_hh-mm-ss-tt}.csv", System.DateTime.Now);
				File.WriteAllText (recordFile, "No Data from Trials"); //This will be overwritten when trial begins
			}
			
			//(4) Read test configuration
			string testConfigPath = Environment.CurrentDirectory + "/config/" + testName + "_config.json";
			if (File.Exists (testConfigPath)) {
				testConfig = JSONNode.Parse (File.ReadAllText (Environment.CurrentDirectory + "/config/" + testName + "_config.json"));
				testNum = testConfig ["test"] ["number"].AsInt;
			}
		}
				
		void Initialize ()
		{
			step = 0;
			trial.counter = new MonkeyCounter ();
			if (testConfig != null) {
				trial.keys = GenerateKeys ();
				if (trial.keys.Count > 0) {
					trial.counter.ChangeTarget (trial.keys [step]);
				} else {
					test = testNum;
				}
			} else {
				trial.keys = new List<int> ();
				test = testNum;
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

		public void WhenPushed (bool complete, int symbol)
		{
			if (StageComplete () ||
				TrialComplete ()) {
				//Already complete -> no event
				return;
			}

			current = timer.ElapsedMilliseconds / 1000.0f;
			trial.counter.WhenPushed (complete, symbol, current);

			if (complete) {
				if (symbol == trial.keys [step]) {
					correct = true;
					step += 1;
					if (TrialComplete ()) {
						trial.counter.CommitTrial ();
						history.Add (trial);
						test += 1;
						// Do not immediately start next test
					} else {
						trial.counter.ChangeTarget (trial.keys [step]);
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
				if (empowerDorin) {
					File.WriteAllText (recordFile, this.ToDorin (isDorinGrid));
				} else {
					File.WriteAllText (recordFile, this.ToString ());
				}
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
				trialKeys += trial.keys [k].ToString () + " ";
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

		//Print results in Dorin accessibility mode
		public string ToDorin (bool isGrid)
		{
			string report = "";
			foreach (Trial h in history) {
				report += h.ToDorin (isGrid);
			}
			if (!TrialComplete ()) {
				report += trial.ToDorin (isGrid);
			}
			return report;
		}
	}
}

