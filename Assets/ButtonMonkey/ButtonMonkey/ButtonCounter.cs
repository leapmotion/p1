using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ButtonMonkey
{
	public struct ButtonPushed {
		public char symbol;
		public TimeSpan time;
		public ButtonPushed(char s, TimeSpan t) {
			symbol = s;
			time = t;
		}
	}

	public struct PushedTrials {
		public char target;
		public List<ButtonPushed> attempts;
		public PushedTrials(char t, List<ButtonPushed> a) {
			target = t;
			attempts = a;
		}
	}

	public class ButtonCounter
	{
		Stopwatch timer;

		bool ready;
		char target;

		List<ButtonPushed> attempts;
		List<PushedTrials> complete;

		public ButtonCounter()
		{
			timer = new Stopwatch ();

			ready = false;
			target = ' ';

			attempts = new List<ButtonPushed> ();
			complete = new List<PushedTrials> ();
		}
		
		public void WhenPushed (char symbol) {
			if (ready == false) {
				return;
			}
			attempts.Add (new ButtonPushed (
				symbol,
				timer.Elapsed
				));
		}

		public void CommitTrial() {
			complete.Add(new PushedTrials(
				target, 
				attempts
				));
			attempts = new List<ButtonPushed>();
			ready = false;
			target = ' ';
		}

		public void ChangeTarget (char next) {
			if (ready == true) {
				CommitTrial();
			}
			ready = true;
			target = next;
			timer.Reset();
			timer.Start();
		}

		//Print results to CSV
		public override string ToString() {
			string report = "Target, Symbol, Time\n";
			foreach (PushedTrials trials in complete) {
				char goal = trials.target;
				foreach (ButtonPushed push in trials.attempts) {
					report += goal + ", " + push.symbol + ", " + push.time.ToString() + "\n";
				}
			}
			return report;
		}
	}
}

