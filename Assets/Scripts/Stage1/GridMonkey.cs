using System;
using System.Collections.Generic;
using ButtonMonkey;

namespace P1
{
		public class GridMonkey : MonkeyTester
		{
				protected override List<int> GenerateKeys ()
				{
						isDorinGrid = true;

						System.Random gen = new System.Random ();

						int row = testConfig ["grid"] ["row"].AsInt;
						int col = testConfig ["grid"] ["col"].AsInt;
						int pins = testConfig ["grid"] ["pins"].AsInt;

						//Generate a selection of random numbers from each row.
						//NOTE: In the case that there are more rows than pins
						//some rows will not be represented in the key sequence.
						//NOTE: In the case that there are fewer rows than pins
						//it will necessary to draw multiple times from a row.
						//ASSERT: The occurance of a row cannot exceed that of any
						//other row by more than 1.
						//ASSERT: The frequency of occurance of each row == pins / rows.
						List<int> keys = new List<int> ();
						while (keys.Count < pins) {
								List<int> picks = new List<int> ();
								for (int r = 0; r < row; ++r) {
										int num = 1 + (r * col) + (gen.Next () % col);
										if (num < 10) {
												picks.Add (num);
										} else {
												picks.Add (0);
										}
								}
				
								// Choose a random ordering of rows
								while (picks.Count > 0 && keys.Count < pins) {
										int next = gen.Next () % picks.Count;
										keys.Add (picks [next]);
										picks.RemoveAt (next);
								}
						}

						return keys;
				}
		}
}
