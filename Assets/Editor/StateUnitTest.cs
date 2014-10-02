using UnityEngine;
using NUnit.Framework;
using System.Collections.Generic;

namespace P1
{
		class StateChangeWatcher
		{
				public StateChange change;

				public void OnStateChange (StateChange c)
				{
						change = c;
				} 

		}

		public class StateUnitTest
		{
		
		#region scaffolding
		
				[SetUp] 
		
				public void CreateYorkieList ()
				{
						StateList.Clear ();
						StateList list = new StateList ("dogs", "Yorkie", "Terrier", "Poodle");
						list.Constrain ("Terrier", "Yorkie");
				}
		
		#endregion

			#region state

				[Test]

				/// <summary>
/// State should be associated with the list "dogs". 
/// </summary>
		public void StateCreationTest ()
				{
						State state = new State ("dogs");
						Assert.AreEqual ("Yorkie", state.currentItem.ToString (), "The state is a member of the 'dogs' stateList and has been initialized to its first item");
				}
		
				[Test]
		
		
				/// <summary>
/// Change accurately reflects whether a state is reachable
/// </summary>
				public void StateChangeTest ()
				{
						State state = new State ("dogs");
						Assert.IsTrue (state.Change ("Terrier"), "Can change to Terrier");
						Assert.IsFalse (state.Change ("Narwhal"), "Cannot change to Narwhal");
			
				}
		
				[Test]
		
				public void StateForceChangeTest ()
				{
			
						State state = new State ("dogs");
						state.Change ("Poodle", true);
						state.Change ("Yorkie", true);
						Assert.IsTrue (state.currentItem.Equals ("Yorkie"), "can force a change against constraints");
				}
		
				[Test]
		
				/// <summary>
/// changing the state affects currentState
/// </summary>
		public void StateChangeTest2 ()
				{
						State state = new State ("dogs");
						state.Change ("Poodle");
						state.Change ("Terrier");
						Assert.AreEqual (state.currentItem.ToString (), "Poodle", "State cannot be changed to Tarrier from Poodle");
						state.Change ("Terrier", true);
						Assert.AreEqual (state.currentItem.ToString (), "Terrier", "State changed to terrier from poodle, forcibly");
			
				}
		
				[Test]
		
				/// <summary>
		/// validates that the state change can be observed
		/// </summary>
		public void StateChangeObservationTest ()
				{
			
						State state = new State ("dogs");
						StateChangeWatcher watcher = new StateChangeWatcher ();
			
						state.StateChangedEvent += watcher.OnStateChange;
			
						state.Change ("Terrier");
			
						Assert.AreEqual ("Terrier", watcher.change.toState.name, "Watcher knows state changed to Terrier");
				}

				[Test]
		
				/// <summary>
		/// validates that you can block certain changes from occuring
		/// </summary>
		public void ControlledStateChangeTest ()
				{
			
						State state = new State ("dogs");
						StateChangeWatcher watcher = new StateChangeWatcher ();
						StateList list = StateList.GetList ("dogs");
			
						state.StateChangedEvent += watcher.OnStateChange;
			
						Assert.IsTrue (state.Change ("Terrier"), "Can change from Yorkie to Terrier");
						Assert.AreEqual ("Terrier", state.currentItem.ToString (), "changed to Terrier");
			
						Assert.IsTrue (state.Change ("Poodle"), "Can change from Terrier to Poodle");
						Assert.AreEqual ("Poodle", state.currentItem.ToString (), "changed to Poodle");
						Assert.IsTrue (watcher.change.allowed, "Watcher reflects allowed change");

						Assert.IsFalse (state.Change ("Terrier"), "Cannot change from Poodle to Terrier");
						Assert.AreEqual ("Poodle", state.currentItem.ToString (), "still a Poodle");
						Assert.AreEqual ("Terrier", watcher.change.toState.name, "Watcher reflects attempted change to Terrier");
						Assert.IsFalse (watcher.change.allowed, "Watcher reflects prohibition of attempted change");
				}
		
				[Test]
		
				/// <summary>
		/// The same test above -- now ensuring that the current state doesn't change when a blocked change is attempted
		/// </summary>
				public void ControlledStateChangeTest2 ()
				{
			
						State state = new State ("dogs");
						StateChangeWatcher watcher = new StateChangeWatcher ();
						StateList list = StateList.GetList ("dogs");
			
						state.StateChangedEvent += watcher.OnStateChange;
			
						state.Change ("Terrier");
						Assert.AreEqual ("Terrier", state.currentItem.ToString (), "changed to Terrier");
			
						state.Change ("Poodle");
						Assert.AreEqual ("Poodle", state.currentItem.ToString (), "changed to Poodle");
			
						state.Change ("Terrier");
						Assert.AreEqual ("Poodle", state.currentItem.ToString (), "still a Poodle");
			
				}
		
				[Test]
		
				/// <summary>
				/// ensures that the allowed field of the StateChange property of the watcher reflects blockage
				/// </summary>
				public void ControlledStateChangeObservationTest ()
				{
			
						State state = new State ("dogs");
						StateChangeWatcher watcher = new StateChangeWatcher ();
						StateList list = StateList.GetList ("dogs");
			
						state.StateChangedEvent += watcher.OnStateChange;
						state.Change ("Terrier");
						Assert.IsTrue (watcher.change.allowed, "Can change from Yorkie to Terrier");
						state.Change ("Poodle");
						Assert.IsTrue (watcher.change.allowed, "Can change from Terrier to Poodle");
						state.Change ("Terrier");
						Assert.IsFalse (watcher.change.allowed, "Cannot change from Poodle to Terrier");
			
				}
		
		#endregion
		
		}
}
