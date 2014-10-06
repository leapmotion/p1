using System;
using System.Collections.Generic;

namespace ButtonMonkey
{
		public struct ButtonPushed
		{
				public char symbol;
				public float time;

				public ButtonPushed (char s, float t)
				{
						symbol = s;
						time = t;
				}
		}

		public struct PushedTrials
		{
				public char target;
				public List<ButtonPushed> attempts;

				public PushedTrials (char t, List<ButtonPushed> a)
				{
						target = t;
						attempts = a;
				}
		}

		public class ButtonCounter
		{
				bool ready;
				char target;
				List<ButtonPushed> attempts;
				List<PushedTrials> complete;

				public ButtonCounter ()
				{
						ready = false;
						target = ' ';

						attempts = new List<ButtonPushed> ();
						complete = new List<PushedTrials> ();
				}
		
				public void WhenPushed (char symbol, float time)
				{
						if (ready == false) {
								return;
						}
						attempts.Add (new ButtonPushed (
				symbol,
				time
						));
				}

				public void CommitTrial ()
				{
						complete.Add (new PushedTrials (
				target, 
				attempts
						));
						attempts = new List<ButtonPushed> ();
						ready = false;
				}

				public void ChangeTarget (char next)
				{
						if (ready == true) {
								CommitTrial ();
						}
						ready = true;
						target = next;
				}

				public ButtonPushed GetTrial (int past)
				{
						return attempts [past];
				}

				//Print results to CSV
				public override string ToString ()
				{
						string report = "Target, Symbol, Time\n";
						foreach (PushedTrials trials in complete) {
								char goal = trials.target;
								foreach (ButtonPushed push in trials.attempts) {
										report += goal + ", " + push.symbol + ", " + push.time.ToString () + "\n";
								}
						}
						return report;
				}
		}
}
