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
		
		public void Trial(List<char> symbolTrial) {
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
					if (symbolInd >= symbolTrial.Count) {
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

			int test = 1;
			while (test<=4) {
				// List a random key from each row
				Random gen = new Random ();
				List<int> rows = new List<int> ();
				rows.Add (1 + (gen.Next () % 3));
				rows.Add (4 + (gen.Next () % 3));
				rows.Add (7 + (gen.Next () % 3));
				rows.Add (0);

				// Generate a random sequence of keys
				// subject to the condition that no row is repeated
				List<int> trial = new List<int> ();
				while (rows.Count > 0) {
					int next = gen.Next () % rows.Count;
					trial.Add (rows [next]);
					rows.RemoveAt (next);
				}

				// Generate keypad 
				List<char> symbolTrial = new List<char> ();
				foreach (int key in trial) {
					symbolTrial.Add (key.ToString () [0]);
				}
				monkeySee.Trial (symbolTrial);

				test+=1;
			}

			Console.WriteLine ("Autopsy report for monkey:\n\n" + monkeyDo.ToString());
		}
	}
}
