using System;
using System.Collections.Generic;

namespace ButtonMonkey
{
	class ButtonTrial {
		public delegate void KeyStrokeCall(char key);
		public event KeyStrokeCall KeyStrokeEvent;

		public delegate void HitSymbolCall(char key);
		public event HitSymbolCall HitSymbolEvent;

		public void Trial(char[] symbolTrial) {
			if (HitSymbolEvent == null ||
				KeyStrokeEvent == null) {
				Console.WriteLine("No monkeys? No research!");
			}

			// Initial instructions
			string instructions = "Monkey, type: ";
			foreach (char symbol in symbolTrial) {
				instructions += symbol;
			}
			Console.WriteLine (instructions);
			
			// Attempt guidance
			int symbolInd = -1;
			bool correct = true;
			while (true) {
				if (correct) {
					symbolInd += 1;
					if (symbolInd >= symbolTrial.Length) {
						HitSymbolEvent(' '); //Commit results by setting new target
						break;
					}
					correct = false;
					HitSymbolEvent(symbolTrial[symbolInd]);
				}

				ConsoleKeyInfo info = Console.ReadKey ();
				KeyStrokeEvent(info.KeyChar);
				
				//User guidance
				if (info.KeyChar != symbolTrial[symbolInd]) {
					Console.WriteLine ("Bad monkey - try again!");
					continue;
				}
				Console.WriteLine ("Good monkey - keep typing!");
				correct = true;
			}
		}
	}

	class MainClass
	{
		public static void Main (string[] args)
		{
			ButtonTrial monkeySee = new ButtonTrial();
			ButtonCounter monkeyDo = new ButtonCounter();
			monkeySee.KeyStrokeEvent += new ButtonTrial.KeyStrokeCall (monkeyDo.WhenPushed);
			monkeySee.HitSymbolEvent += new ButtonTrial.HitSymbolCall (monkeyDo.ChangeTarget);

			char[] symbolTrial = {'1','2','3','4'};
			monkeySee.Trial (symbolTrial);

			Console.WriteLine ("Autopsy report for monkey:\n\n" + monkeyDo.ToString());
		}
	}
}
