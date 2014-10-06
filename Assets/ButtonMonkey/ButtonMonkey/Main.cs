using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ButtonMonkey
{
	class ConsoleTrial {
		public delegate void KeyStrokeCall(char key, float time);
		public event KeyStrokeCall KeyStrokeEvent;
		
		public delegate void HitSymbolCall(char key);
		public event HitSymbolCall HitSymbolEvent;
		
		public void Trial(char[] symbolTrial) {
			#if UNITY_EDITOR
			Console.WriteLine ("Have a banana...");
			return;
			#else

			if (HitSymbolEvent == null ||
			    KeyStrokeEvent == null) {
				Console.WriteLine("No monkeys? No research!");
			}
			
			// Initial instructions
			Stopwatch timer = new Stopwatch();
			string instructions = "Monkey, type: ";
			foreach (char symbol in symbolTrial) {
				instructions += symbol;
			}
			Console.WriteLine (instructions);

			// Attempt guidance
			timer.Start ();
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
				KeyStrokeEvent(info.KeyChar, timer.ElapsedMilliseconds/1000.0f);
				
				//User guidance
				if (info.KeyChar != symbolTrial[symbolInd]) {
					Console.WriteLine ("Bad monkey - try again!");
					continue;
				}
				Console.WriteLine ("Good monkey - keep typing!");
				correct = true;
			}		
			#endif
		}
	}

	class MainClass
	{
		public static void Main (string[] args)
		{
			ConsoleTrial monkeySee = new ConsoleTrial();
			ButtonCounter monkeyDo = new ButtonCounter();
			monkeySee.KeyStrokeEvent += new ConsoleTrial.KeyStrokeCall (monkeyDo.WhenPushed);
			monkeySee.HitSymbolEvent += new ConsoleTrial.HitSymbolCall (monkeyDo.ChangeTarget);

			char[] symbolTrial = {'1','2','3','4'};
			monkeySee.Trial (symbolTrial);

			Console.WriteLine ("Autopsy report for monkey:\n\n" + monkeyDo.ToString());
		}
	}
}
