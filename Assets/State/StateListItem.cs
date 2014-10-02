using UnityEngine;
using System.Collections;

namespace P1
{
		public class StateListItem
		{

				public string name;
				StateList list;
		
				public StateListItem (string n, StateList l)
				{
						name = n;
						list = l;
				}
		
// this constructor only exists for testing; in production, items should always be list aware
				public StateListItem (string n)
				{
						name = n;
				}

				public bool Equals (string otherName)
				{
						return otherName.ToLower () == name.ToLower ();
				}

				public string ToString ()
				{
						return name;
				} 

		}
}