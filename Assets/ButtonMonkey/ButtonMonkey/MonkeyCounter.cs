using System;
using System.Collections.Generic;

namespace ButtonMonkey
{
		public struct ButtonPushed
		{
				public bool complete;
				public int symbol;
				public float time;

				public ButtonPushed (bool c, int s, float t)
				{
						complete = c;
						symbol = s;
						time = t;
				}

				public string RecordFormat (int goal)
				{
						//return goal + ", " + push.symbol + ", " + push.complete.ToString () + ", " + push.time.ToString () + "\n";
			
						//Dorin accessibility assistance
						string report = "";
						int IsHit = 0;
						if (goal == symbol) {
								IsHit = 1;
						}
						int rectified = -1; //DEFAULT: Not on key pad
						if (0 <= symbol && symbol <= 9) {
								rectified = symbol;
						}
						if (rectified >= 0 && !complete) {
								rectified += 10;
						}
						report += goal.ToString () + ", " + rectified.ToString () + ", " + IsHit.ToString () + ", " + time.ToString () + "\n";
						return report;
				}
		}

		public struct AttemptsMade
		{
				public int target;
				public List<ButtonPushed> attempts;

				public AttemptsMade (int t, List<ButtonPushed> a)
				{
						target = t;
						attempts = a;
				}
		}

		public class MonkeyCounter
		{
				bool ready;
				int target;
				List<ButtonPushed> attempts;
				List<AttemptsMade> complete;

				public MonkeyCounter ()
				{
						ready = false;
						target = ' ';

						attempts = new List<ButtonPushed> ();
						complete = new List<AttemptsMade> ();
				}
		
				public void WhenPushed (bool complete, int symbol, float time)
				{
						if (ready == false) {
								return;
						}
						attempts.Add (new ButtonPushed (
							complete, 
							symbol,
							time
						));
				}

				public void CommitTrial ()
				{
						complete.Add (new AttemptsMade (
				target, 
				attempts
						));
						attempts = new List<ButtonPushed> ();
						ready = false;
				}

				public void ChangeTarget (int next)
				{
						if (ready == true) {
								CommitTrial ();
						}
						ready = true;
						target = next;
				}

				public void Reset ()
				{
						ready = false;
						target = ' ';
						attempts.Clear ();
						complete.Clear ();

				}

				public ButtonPushed GetTrial (int past)
				{
						return attempts [past];
				}

				public int CurrentAttemptsCount {
						get {
								return attempts.Count;
						}
				}
		
				public int CompletedTrialsCount {
						get {
								return complete.Count;
						}
				}

				public int CurrentSuccessCount {
						get { 
								int successes = 0;
								foreach (ButtonPushed push in attempts) {
										if (push.symbol == target) {
												successes += 1;
										}
								}
								return successes; 
						}
				}
		
				public int CompletedSuccessCount {
						get { 
								int successes = 0;
								foreach (AttemptsMade c in complete) {
										int goal = c.target;
										foreach (ButtonPushed a in c.attempts) {
												if (a.symbol == goal) {
														successes += 1;
												}
										}
								}
								return successes; 
						}
				}

				//Print results in CSV format
				public override string ToString ()
				{
						string report = "";
						foreach (AttemptsMade c in complete) {
								int goal = c.target;
								foreach (ButtonPushed a in c.attempts) {
										report += a.RecordFormat (goal);
								}
						}
						foreach (ButtonPushed a in attempts) {
								report += a.RecordFormat (target);
						}
						return report;
				}
		}
}
