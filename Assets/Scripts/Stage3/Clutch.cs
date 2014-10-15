using UnityEngine;
using System;

namespace P1
{
		public class Clutch : MonoBehaviour
		{
				//Enveloping interaction volume state
				const string STATE_NAME_FINGERS = "Finger state";
				const string FINGERS_OUTSIDE = "Fingers outside of all volumes";
				const string FINGERS_TRIGGER = "Fingers inside of interaction volume";
				State fingerState;
				public BoneTracker fingerTracker;
				//Scrollable object state
				const string STATE_NAME_CLUTCH = "Scrolling state";
				const string CLUTCH_RELEASED = "Fingers outside of scroll volume";
				const string CLUTCH_ENGAGED = "Fingers inside of scroll volume";
				State clutchState;
				public BoneTracker clutchTracker;

				void InitClutchStateList ()
				{
						if (!StateList.HasList (STATE_NAME_FINGERS))
								StateList.Create (
					STATE_NAME_FINGERS,
					FINGERS_OUTSIDE,
					FINGERS_TRIGGER
								);
						fingerState = new State (STATE_NAME_FINGERS,
			                         FINGERS_OUTSIDE);

						if (!StateList.HasList (STATE_NAME_CLUTCH))
								StateList.Create (
					STATE_NAME_CLUTCH,
					CLUTCH_RELEASED,
					CLUTCH_ENGAGED
								);
						clutchState = new State (STATE_NAME_CLUTCH,
			                         CLUTCH_RELEASED);
				}

		#region loop

				void Start ()
				{
						DoStart ();
				}

				public void DoStart ()
				{
						InitClutchStateList ();
						fingerTracker.CountChangedEvent += OnFingerCountChanged;
						fingerState.StateChangedEvent += OnFingerChange;
						clutchTracker.CountChangedEvent += OnClutchCountChanged;
						clutchState.StateChangedEvent += OnClutchChange;
				}

		#endregion

		#region bones
			
				void OnFingerCountChanged (CountChangedRecord cce)
				{
						//UnityEngine.Debug.Log ("OnFingerCountChanged count -> " + cce.newCount);
						if (cce.newCount == 0) {
								fingerState.Change (FINGERS_OUTSIDE);
						} else {
								fingerState.Change (FINGERS_TRIGGER);
						}

				}
		
				void OnClutchCountChanged (CountChangedRecord cce)
				{
						//UnityEngine.Debug.Log ("OnClutchCountChanged count -> " + cce.newCount);
						if (cce.newCount == 0) {
								clutchState.Change (CLUTCH_RELEASED);
						} else {
								clutchState.Change (CLUTCH_ENGAGED);
						}
			
				}

		#endregion 

		#region state
		
				public void OnFingerChange (StateChange c)
				{
						switch (c.state) {
						case FINGERS_OUTSIDE:
				UnityEngine.Debug.Log("Be freeeee!");
								BounceLock(false);
								break;
				
						case FINGERS_TRIGGER:
								break;
						default:
								UnityEngine.Debug.Log ("OnFingerChange : Transition to unknown state: " + c.state);
								break;
						}
						UnityEngine.Debug.Log ("OnFingerChange c.state = " + c.state);
						VisualDebug ();
				}

				public void OnClutchChange (StateChange c)
				{
						switch (c.state) {
						case CLUTCH_RELEASED:
								break;

						case CLUTCH_ENGAGED:
								break;
						default:
								UnityEngine.Debug.Log ("OnClutchChange: Transition to unknown state: " + c.state);
								break;
						}
						UnityEngine.Debug.Log ("OnClutchChange c.state = " + c.state);
						VisualDebug ();
				}

				void VisualDebug ()
				{
						Color c;
						if (clutchState.state == CLUTCH_RELEASED) {
								if (fingerState.state == FINGERS_OUTSIDE) {
										c = Color.red;
								} else if (fingerState.state == FINGERS_TRIGGER) {
										c = Color.green;
								} else {
										c = Color.black;
								}
						} else if (clutchState.state == CLUTCH_ENGAGED) {
								if (fingerState.state == FINGERS_OUTSIDE) {
										c = Color.blue;
								} else if (fingerState.state == FINGERS_TRIGGER) {
										c = Color.yellow;
								} else {
										c = Color.white;
								}
						} else {
								c = Color.magenta;
						}
						renderer.material.color = c;
		
				}

		#endregion

		public void BounceLock(bool hold = true) {
			UnityEngine.Debug.Log ("BounceLock");
			rigidbody.isKinematic = hold;
			if (!hold) {
				rigidbody.AddForce(new Vector3(0.0f, 0.0f, -200.0f));
						}
		}

		}
}

