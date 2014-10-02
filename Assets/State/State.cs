using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace P1
{
		public class State
		{

				StateList list;
				public StateListItem currentItem;

				public State (string name)
				{
						list = StateList.GetList (name);
						currentItem = list.First ();
				}

				public State (string name, string itemName)
				{
						list = StateList.GetList (name);
						currentItem = list.Item (itemName);
				}
		
				public bool Change (string name, bool force)
				{
						if (force) {
								if (list.Contains (name)) {
										currentItem = list.Item (name);
										return true;
								} else {
										return false;
								}
						} else {
								return	Change (name);
						}
				}

				public bool Change (string name)
				{
						foreach (StateListItem item in list.items) {
								if (item.Equals (name)) {

// if there are controls over a given StateItem, enforce them
										if (list.controlledStateChanges.ContainsKey (item)) {
						
//												Debug.Log ("--- enforcing change limits for changing to " + item.ToString ());
												foreach (StateListItem allowedItem in list.controlledStateChanges[item]) {
														//	Debug.Log ("... can change from state " + allowedItem.ToString ());
														if (allowedItem.Equals (currentItem)) {
																//Debug.Log (".... and that is what we are trying to do!");
																StateChanged (currentItem, item);
																currentItem = item;
																return true;					
														}
												}
												StateChanged (currentItem, item, false);
												return false;
										}

										StateChanged (currentItem, item);
										currentItem = item;
										return true;
								}

						}
						return false;
				}
		
		#region Events

				public delegate void StateChangedDelegate (StateChange change);
		
				/// <summary>An even that gets fired </summary>
				public event StateChangedDelegate StateChangedEvent;
		
				public void StateChanged (StateListItem fromItem, StateListItem toItem)
				{
						if (StateChangedEvent != null) // fire the event
								StateChangedEvent (new StateChange (fromItem, toItem, list, true));
				}

				public void StateChanged (StateListItem fromItem, StateListItem toItem, bool allowed)
				{
						if (StateChangedEvent != null) // fire the event
								StateChangedEvent (new StateChange (fromItem, toItem, list, allowed));
				}

		#endregion
		
		}
}