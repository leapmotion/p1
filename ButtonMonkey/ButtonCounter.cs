using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ButtonMonkey
{
	public struct ButtonPushed {
		public char symbol;
		public bool correct;
		public TimeSpan time;
		public ButtonPushed(char s, bool c, TimeSpan t) {
			symbol = s;
			correct = c;
			time = t;
		}
	}

	public class ButtonCounter
	{
		Stopwatch timer;

		char symbol;
		List<ButtonPushed> attempts;
		List<List<ButtonPushed>> complete;

		public ButtonCounter()
		{
			attempts = new List<ButtonPushed> ();
			complete = new List<List<ButtonPushed>> ();

			timer = new Stopwatch ();
		}

		public void SetNextSymbol(char next) {
			complete.Add(attempts);
			attempts = new List<ButtonPushed>();
			symbol = next;
			timer.Reset();
		}

		public void OnKeyStroke (char key) {
			attempts.Add (new ButtonPushed (
				key,
				key == symbol,
				timer.Elapsed
			));
		}
	}
}

