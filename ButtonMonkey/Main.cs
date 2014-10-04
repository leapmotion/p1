using System;
using System.Collections.Generic;

namespace ButtonMonkey
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			ButtonCounter monkey = new ButtonCounter();

			// Print 4 Numbers
			char[] symbols = {'1','2','3','4'};
			Console.WriteLine ("Monkey, type: " + symbols[0] + " " + symbols[1] + " " + symbols[2] + " " + symbols[3]);

			// Retrieve Attempts
			int symbolInd = -1;
			bool correct = true;
			while (true) {
				if (correct) {
					symbolInd += 1;
					if (symbolInd > 3) {
						break;
					}
					monkey.SetNextSymbol(symbols[symbolInd]);
					correct = false;
				}
				ConsoleKeyInfo info = Console.ReadKey ();
				monkey.OnKeyStroke(info.KeyChar);

				//User guidance
				if (info.KeyChar != symbols[symbolInd]) {
					Console.WriteLine ("Bad monkey - try again!");
					continue;
				}
				Console.WriteLine ("Good monkey - keep typing!");
				correct = true;
			}

			Console.WriteLine ("Monkey dismissed...");
		}
	}
}
