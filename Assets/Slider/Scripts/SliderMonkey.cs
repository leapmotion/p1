using System;
using System.Collections.Generic;
using ButtonMonkey;

namespace P1
{
	public class SliderMonkey : ButtonTrial
	{
		protected override List<int> GenerateKeys ()
		{
			System.Random gen = new System.Random ();

			int min = config ["min"].AsInt;
			int max = config ["max"].AsInt;

			// Choose a single random number in the specified range
			int next = (gen.Next () % (max - min)) + min;

			List<int> keys = new List<int> ();
			keys.Add (next);
			return keys;
		}
	}
}
