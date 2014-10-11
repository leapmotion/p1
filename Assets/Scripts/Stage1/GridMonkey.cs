using System;
using System.Collections.Generic;
using ButtonMonkey;

namespace P1
{
		public class GridMonkey : ButtonTrial
		{
				protected override List<int> GenerateKeys ()
				{
						System.Random gen = new System.Random ();
				
						// List a random key from each row
						List<int> rows = new List<int> ();
						rows.Add (1 + (gen.Next () % 3));
						rows.Add (4 + (gen.Next () % 3));
						rows.Add (7 + (gen.Next () % 3));
						if (config ["test"]["pickZero"].AsBool) {
								rows.Add (0);
						}
				
						// Choose a random ordering of rows
						List<int> keys = new List<int> ();
						while (rows.Count > 0) {
								int next = gen.Next () % rows.Count;
								keys.Add (rows [next]);
								rows.RemoveAt (next);
						}
						return keys;
				}
		}
}
