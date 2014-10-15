using System;
using System.Collections.Generic;
using ButtonMonkey;

namespace P1
{
		public class TwitterMonkey : MonkeyTester
		{
				public int statusButtonsCount = 0;
				int last_number = int.MaxValue;

				protected override List<int> GenerateKeys ()
				{
						System.Random gen = new System.Random ();

						const int min = 0;
						int max = testConfig ["max_tweets"].AsInt - 1;
						if (statusButtonsCount > 0 &&
								max > statusButtonsCount) {
								max = statusButtonsCount;
						}

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
