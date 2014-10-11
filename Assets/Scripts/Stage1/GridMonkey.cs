using System;
using System.Collections.Generic;
using ButtonMonkey;

namespace P1
{
		public class GridMonkey : MonkeyTester
		{
				protected override List<int> GenerateKeys ()
				{
						System.Random gen = new System.Random ();

						int row = config ["grid"] ["row"].AsInt;
						int col = config ["grid"] ["col"].AsInt;
				
						// List a random pick from each row
						List<int> picks = new List<int> ();
						for (int r = 0; r < row; ++r) {
								if (1 + (r * col) < 10) {
										picks.Add (1 + (r * col) + (gen.Next () % col));
								} else {
										picks.Add (0);
										break;
								}
						}
				
						// Choose a random ordering of rows
						List<int> keys = new List<int> ();
						while (picks.Count > 0) {
								int next = gen.Next () % picks.Count;
								keys.Add (picks [next]);
								picks.RemoveAt (next);
						}
						return keys;
				}
		}
}
