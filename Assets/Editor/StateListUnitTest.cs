using UnityEngine;
using NUnit.Framework;
using System.Collections.Generic;

namespace P1
{
		public class StateListUnitTest
		{
		
#region StateListItem
				[Test]
				/// <summary>
		/// Tests identity/creation of StateListItem
		/// </summary>
				public void StateListItemCreationTest ()
				{
						string expect = "foo";
						StateListItem sli = new StateListItem (expect);
						Assert.AreEqual (expect, sli.name, "new StateListItem has name 'foo'");
				}
		
		#endregion

#region StateList
		
				[Test]
				/// <summary>
		/// Tests basic list membership/creation
		/// </summary>
		public void StateListCreationTest ()
				{
						StateList.Clear ();
						StateList list = new StateList ("dogs", "Yorkie", "Terrier", "Dauschound");
						Assert.IsTrue (list.Contains ("Yorkie"), "can find a dog by name");
						Assert.IsFalse (list.Contains ("kitten"), "cannot find an item not in the list");
				}
		
				[Test]
		
				/// <summary>
/// Testing the ability to equate an element with a string irrespective of case
/// </summary>
		public void StateListCreationTest2 ()
				{
						StateList.Clear ();
						StateList list = new StateList ("dogs", "Yorkie", "Terrier", "Dauschound");
						Assert.IsTrue (list.Contains ("yorkie"), "can find a dog by a lowercase name");
				}

				[Test]
		
				/// <summary>
/// In this test we get the list indirectly from the static StateList.Get method
/// </summary>
		public void StateListCreationTest3 ()
				{
						StateList.Clear ();
						new StateList ("dogs", "Yorkie", "Terrier", "Dauschound");
						StateList list = StateList.GetList ("dogs");
						Assert.IsTrue (list.Contains ("Yorkie"), "can find a dog by a name");
				}
		
		#endregion

		#region state

[Test]

/// <summary>
/// State should be associated with the list "dogs". 
/// </summary>
		public void StateCreationTest(){
			
			StateList.Clear ();
			new StateList ("dogs", "Yorkie", "Terrier", "Dauschound");
			State state = new State("dogs");
			Assert.AreEqual( "Yorkie", state.currentItem.ToString (), "The state is a member of the 'dogs' stateList and has been initialized to its first item");
		}
		
		[Test]

		public void StateChangeTest(){
			StateList.Clear ();
			new StateList ("dogs", "Yorkie", "Terrier", "Dauschound");
			State state = new State("dogs");
			state.Change ("Terrier");
			Assert.AreEqual( "Terrier", state.currentItem.ToString (), "Can change the state");
			
}
		
#endregion

		}
}
