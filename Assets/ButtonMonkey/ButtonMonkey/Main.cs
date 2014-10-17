using System;
using System.Collections.Generic;

namespace ButtonMonkey
{
	public class ConsoleMonkey : MonkeyTester
	{
		protected override List<int> GenerateKeys ()
		{
			System.Random gen = new System.Random ();
			int trialNum = testConfig ["trialNum"].AsInt;

			List<int> keys = new List<int> ();
			for (int trial = 0; trial < trialNum; ++trial) {
				keys.Add (gen.Next () % 10);
			}
			return keys;
		}
	}

	class MainClass
	{
		public static void Main (string[] args)
		{
			MonkeyTalker monkeySee = MonkeyTalker.instance;
			#if UNITY_EDITOR || UNITY_STANDALONE
				monkeySee.Log ("No monkeys? No research!");
				return;
			#else
			ConsoleMonkey monkeyDo = new ConsoleMonkey ();
			monkeyDo.ConfigureTest ("console");
			monkeyDo.Start ();
			monkeySee.Log ("Monkey, type: " + monkeyDo.GetTrialKeysString ());

			while (true) {
				if (monkeyDo.StageComplete ()) {
					monkeySee.Log  ("Well done monkey! Close you eyes and we'll give you a treat...");
					break; //End testing
				}
				if (monkeyDo.TrialComplete ()) {
					monkeyDo.Start (); //Start next trial
					monkeySee.Log  ("Monkey, type: " + monkeyDo.GetTrialKeysString ());
				}

				ConsoleKeyInfo info = Console.ReadKey ();
				int symbol = -1; //DEFAULT: Key is not a digit
				switch (info.KeyChar) {
				case '1': 
					symbol = 1;
					break;
				case '2': 
					symbol = 2;
					break;
				case '3': 
					symbol = 3;
					break;
				case '4': 
					symbol = 4;
					break;
				case '5': 
					symbol = 5;
					break;
				case '6': 
					symbol = 6;
					break;
				case '7': 
					symbol = 7;
					break;
				case '8': 
					symbol = 8;
					break;
				case '9': 
					symbol = 9;
					break;
				case '0': 
					symbol = 0;
					break;
				default:
					break; //DEFAULT
				}
				monkeyDo.WhenPushed (true, symbol); //No partial key strokes

				if (!monkeyDo.TrialComplete ()) {
					if (monkeyDo.WasCorrect ()) {
						monkeySee.Log ("Good monkey! Next, type: " + monkeyDo.GetTrialKeys () [monkeyDo.GetTrialStep ()]);
					} else {
						monkeySee.Log ("Bad monkey! You were told to type: " + monkeyDo.GetTrialKeys () [monkeyDo.GetTrialStep ()]);
					}
				}
			}
			monkeySee.Log ("Autopsy report for monkey in: " + monkeyDo.recordPath);
			#endif
		}
	}
}
