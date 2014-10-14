using UnityEngine;
using System.Collections;

namespace P1
{

		/// <summary>
		/// A record of a change from one state to another
		/// </summary>
		public struct StateChange
		{

				public StateListItem fromState;
				public StateListItem toState;
				public StateList list;
				public bool allowed;

				public string state { get { return toState.name; } }

				public bool unchanged { get { return fromState.name == toState.name; } }

				public string state { get { return toState.name; } }

				public  StateChange (StateListItem fromS, StateListItem toS, StateList li, bool a)
				{
						fromState = fromS;
						toState = toS;
						list = li;
						allowed = a;
				}

				public string ToString ()
				{
						return " [change]  " + fromState.name + " ... " + toState.name + ": " + (allowed ? "" : " (not allowed)") + (unchanged ? " (unchanged)" : "");
				}

		}

}