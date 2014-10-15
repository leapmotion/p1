using System;
using System.Collections.Generic;
using ButtonMonkey;

namespace P1
{
		public class SliderMonkey : MonkeyTester
		{
				//NOTE: Slider position always begins at 0, so 0
				//should not be the first number picked.
				int last_number = 0;

				protected override List<int> GenerateKeys ()
				{
						System.Random gen = new System.Random ();

						int min = testConfig ["min"].AsInt;
						int max = testConfig ["max"].AsInt;

						//Choose a single random number in the specified range,
						//with last_number removed
						int next = (gen.Next () % (max - min)) + min;
						if (next >= last_number) {
								next += 1;
								if (next > max) {
										next = min;
								}
						}
						last_number = next;
			
						List<int> keys = new List<int> ();
						keys.Add (next);
						return keys;
				}
		}
}
