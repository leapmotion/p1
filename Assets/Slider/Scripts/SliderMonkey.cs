using System;
using System.Collections.Generic;
using ButtonMonkey;

namespace P1
{
		public class SliderMonkey : MonkeyTester
		{
				protected override List<int> GenerateKeys ()
				{
						System.Random gen = new System.Random ();

						int min = testConfig ["min"].AsInt;
						int max = testConfig ["max"].AsInt;

						// Choose a single random number in the specified range
						int next = (gen.Next () % (max - min)) + min;

						List<int> keys = new List<int> ();
						keys.Add (next);
						return keys;
				}
		}
}
