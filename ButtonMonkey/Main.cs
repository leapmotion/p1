using System;
using System.Collections.Generic;

namespace ButtonMonkey
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			#if UNITY_EDITOR || UNITY_STANDALONE
				Console.WriteLine ("No monkeys? No research!");
				return;
			#else
			ButtonTrial monkeyDo = new ButtonTrial();

			int test = 1;
			while (test <= 4) {
				// Initial instructions
				monkeyDo.Start ();
				Console.WriteLine ("\nMonkey, type: " + monkeyDo.GetTrialKeys ());

				while (true) {
					ConsoleKeyInfo info = Console.ReadKey ();
					monkeyDo.WhenPushed (true, info.KeyChar); //No partial key strokes
					
					// Monkey guidance
					if (monkeyDo.IsComplete ()) {
						Console.WriteLine ("\nWell done, monkey!\n");
						break;
					} else {
						if (monkeyDo.WasCorrect ()) {
							Console.WriteLine ("\nGood monkey! Next, type: " + monkeyDo.GetTargetKey ());
						} else {
							Console.WriteLine ("\nBad monkey! You were told to type: " + monkeyDo.GetTargetKey ());
						}
					}
				}

				test += 1;
			}
			
			Console.WriteLine ("Autopsy report for monkey:\n\n" + monkeyDo.ToString ());
			#endif
		}
	}
}
