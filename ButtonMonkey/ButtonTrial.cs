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
				float current;
				string report;
				Stopwatch timer;

				// Initialize to completed state
				public ButtonTrial ()
				{
						step = 4;
						keys = new List<int> ();
						counter = new ButtonCounter ();
						correct = false;
						current = 0.0f;
						report = "";

						timer = new Stopwatch ();
				}
		
				// Generate a random sequence of 4 keys
				// subject to the condition that no row is repeated
				public void Start ()
				{
						System.Random gen = new System.Random ();
			
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
			
						current = 0.0f;
						timer.Reset ();
						timer.Start ();
				}
		
				public void WhenPushed (char label)
				{
						if (IsComplete ()) {
								return;
						}

						current = timer.ElapsedMilliseconds / 1000.0f;
						counter.WhenPushed (label, current);
			
						if (label == keys [step].ToString () [0]) {
								step += 1;
								correct = true;
								if (IsComplete ()) {
										counter.CommitTrial ();
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

				public bool HasAttempt ()
				{
						return counter.CurrentAttemptsCount > 0;
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
						return step > 3;
				}
		
				//Print results in CSV format
				public override string ToString ()
				{
						return report;
				}
		}
}

