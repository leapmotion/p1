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

	public class ButtonCounter
	{
		Stopwatch timer;

		bool ready;
		char target;

		List<ButtonPushed> attempts;
		List<Tuple<char, List<ButtonPushed>>> complete;

		public ButtonCounter()
		{
			timer = new Stopwatch ();

			ready = false;
			target = ' ';

			attempts = new List<ButtonPushed> ();
			complete = new List<Tuple<char, List<ButtonPushed>>> ();
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
			complete.Add(new Tuple<char, List<ButtonPushed>>(
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
			foreach (Tuple<char, List<ButtonPushed>> trial in complete) {
				char goal = trial.Item1;
				foreach (ButtonPushed push in trial.Item2) {
					report += goal + ", " + push.symbol + ", " + push.time.ToString() + "\n";
				}
			}
			return report;
		}
	}
}

