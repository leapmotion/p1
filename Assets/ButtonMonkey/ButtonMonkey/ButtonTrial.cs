using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ButtonMonkey
{
	public class ButtonTrial
	{
		int step;
		List<int> keys;
		ButtonCounter counter;
		bool correct;
		string report;
		Stopwatch timer;

		// Initialize to completed state
		public ButtonTrial ()
		{
			step = 4;
			keys = new List<int> ();
			counter = new ButtonCounter ();
			correct = false;
			report = "";

			timer = new Stopwatch ();
		}
		
		// Generate a random sequence of 4 keys
		// subject to the condition that no row is repeated
		public void Start ()
		{
			Random gen = new Random ();
			
			// List a random key from each row
			List<int> rows = new List<int> ();
			rows.Add (1 + (gen.Next () % 3));
			rows.Add (4 + (gen.Next () % 3));
			rows.Add (7 + (gen.Next () % 3));
			rows.Add (0);
			
			// Choose a random ordering of rows
			keys.Clear ();
			while (rows.Count > 0) {
				int next = gen.Next () % rows.Count;
				keys.Add (rows [next]);
				rows.RemoveAt (next);
			}
			
			// Initialize 
			step = 0;
			counter.Reset ();
			counter.ChangeTarget (keys [step].ToString () [0]);
			
			timer.Reset ();
			timer.Start ();
		}
		
		public void WhenPushed (char label)
		{
			if (IsComplete ()) {
				return;
			}
			
			counter.WhenPushed (label, timer.ElapsedMilliseconds / 1000.0f);
			
			if (label == keys [step].ToString () [0]) {
				step += 1;
				correct = true;
				if (IsComplete ()) {
					report += "Trial: " + GetTrialKeys () + "\n";
					report += counter.ToString () + "\n";
				} else {
					counter.ChangeTarget (keys [step].ToString () [0]);
				}
			} else {
				correct = false;
			}
		}

		public char GetTargetKey ()
		{
			if (IsComplete ()) {
				return ' ';
			}
			return keys [step].ToString () [0];
		}

		public string GetTrialKeys ()
		{
			string trialKeys = "";
			for (int k = 0; k < 4; k += 1) {
				trialKeys += keys [k].ToString () [0];
			}
			return trialKeys;
		}

		public bool WasCorrect ()
		{
			return correct;
		}

		public bool IsComplete ()
		{
			return step > 3;
		}
		
		//Print results in CSV format
		public override string ToString ()
		{
			return report;
		}
	}
}

