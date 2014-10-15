using UnityEngine;
using System.Collections;
using System;
using Leap;

namespace P1
{
		public class GripManager : MonoBehaviour
		{
				//grip state
				const string STATE_NAME_TLS_GRIP = "twitter status button grip state";
				const string GRIPSTATE_START = "start";
				const string GRIPSTATE_0BASE = "base";
				const string GRIPSTATE_1THERGRIPPED = "otherGripped";
				const string GRIPSTATE_2GRIPPED = "gripped";
				public State gripState;

				// active manager
				static GripManager activeGrip;

				// which finger initally grabbed in, and where it was then. 
				FingerModel grippingFinger = null;
				float lastFingerTime = 0;
				const float FINGER_DELAY = 0.5f;
				Vector3 fingerPos;

				// affordances for state
				public GameObject gripIndicator;
				public GameObject inactiveIndicator;

				//related components
				TwitterStatusButton statusButton;

#region loop

				float FingerDistance (FingerModel fm)
				{
						if (fm == null)
								throw new UnityException ("no fingerModel for calculating distance for " + this);
						return (transform.position - fm.GetJointPosition (2)).magnitude;
				}

				// Use this for initialization
				void Start ()
				{
						InitStatusButton ();
						InitGripState ();
				}
	
				// Update is called once per frame
				void Update ()
				{
						inactiveIndicator.SetActive (
								(gripState.state == GRIPSTATE_0BASE) 
								&& (activeGrip != null)
								&& (activeGrip.id != id)
						);

						if (gripState.state == GRIPSTATE_2GRIPPED) {
								if (FingerNotStale ()) {
										gripIndicator.SetActive (true);
										Crawl ();

								} else {
										gripState.Change (GRIPSTATE_0BASE);
								}
						} else {
								gripIndicator.SetActive (false);

						}
				}

				void InitStatusButton ()
				{
						if (statusButton == null)
								statusButton = GetComponentInParent<TwitterStatusButton> ();
				}

				public void InitGripState ()
				{
						if (!StateList.HasList (STATE_NAME_TLS_GRIP))
								InitGripStateList ();
						gripState = new State (STATE_NAME_TLS_GRIP);
						gripState.StateChangedEvent += OnGripStateChange;
						gripState.Change (GRIPSTATE_0BASE);
				}

		#endregion

				void Crawl ()
				{
						if (grippingFinger == null) {
								return;
						}

						Vector3 currentPos = grippingFinger.GetTipPosition ();
						Vector3 movement = currentPos - fingerPos;

						//Classify Motion
						if (Math.Abs (movement.y) > Math.Abs (movement.x) &&
								Math.Abs (movement.y) > Math.Abs (movement.z)) {
								//Scroll
								movement.x = 0;
								movement.z = 0;	
								statusButton.MoveList (movement);
						}
						if (Math.Abs (movement.x) > Math.Abs (movement.y) &&
								Math.Abs (movement.x) > Math.Abs (movement.z)) {
								//Select
								movement.y = 0;
								movement.z = 0;	
								rigidbody.MovePosition (movement);
						}
						if (Math.Abs (movement.x) > Math.Abs (movement.y) &&
								Math.Abs (movement.x) > Math.Abs (movement.z)) {
								//Ignore
								UnityEngine.Debug.Log ("Why so Zerious?");
						}
						fingerPos = currentPos;
				}
		
#region state
		
		
				public bool FingerNotStale ()
				{
						return !Utils.Elapsed (lastFingerTime, FINGER_DELAY);
				}

				void InitGripStateList ()
				{
						if (!StateList.HasList (STATE_NAME_TLS_GRIP))
								StateList.Create (
					STATE_NAME_TLS_GRIP,
					GRIPSTATE_START,
				                  GRIPSTATE_0BASE,  // the button is not gripped - but can be gripped
				                  GRIPSTATE_2GRIPPED // this button is gripped
								).Constrain (GRIPSTATE_2GRIPPED, GRIPSTATE_0BASE);
				}
		
				void OnTriggerEnter (Collider c)
				{
						if (activeGrip != null)
								return;
						FingerModel enteringFinger = c.gameObject.GetComponentInParent<FingerModel> ();
						if (enteringFinger != null) {
								grippingFinger = enteringFinger;
								gripState.Change (GRIPSTATE_2GRIPPED);
						}
				}
		
				void OnTriggerStay (Collider c)
				{
						if (grippingFinger == null)
								return;
						FingerModel enteringFinger = c.gameObject.GetComponentInParent<FingerModel> ();
						if (enteringFinger == null)
								return;
						if (enteringFinger.GetLeapFinger ().Id == grippingFinger.GetLeapFinger ().Id) {
								lastFingerTime = Time.time;
						}
				}
		
				void OnTriggerExit (Collider c)
				{
						if (gripState.state != GRIPSTATE_2GRIPPED)
								return;
						FingerModel leavingFinger = c.gameObject.GetComponentInParent<FingerModel> ();
						if (leavingFinger == null)
								return;
						int lfid = leavingFinger.GetLeapFinger ().Id;
						if (lfid == grippingFinger.GetLeapFinger ().Id) {
								gripState.Change (GRIPSTATE_0BASE);
						}
				}

				void ReactivateOtherGripManagers ()
				{
						foreach (TwitterStatusButton otherButton in statusButton.list.statusButtons) {
								if (otherButton.index != id) {
										otherButton.gripManager.gripState.Change (GRIPSTATE_0BASE);
								}
						}

				}

				void OnGripStateChange (StateChange change)
				{
						if (change.unchanged || (!change.allowed)) {
								Debug.Log ("ignoring change " + change.ToString ());
								return;
						}

					//	Debug.Log ("GripStateChange for item #" + statusButton.index + ": " + change.toState.name);

						switch (change.toState.name) {
						case GRIPSTATE_2GRIPPED:
								inactiveIndicator.SetActive (false);
								activeGrip = this;
								lastFingerTime = Time.time;
								if (grippingFinger != null) {
										fingerPos = grippingFinger.GetTipPosition ();
								} else {
										Debug.Log ("Cannot find a gripping finger for gripManager " + this);
								}
								break;

						case GRIPSTATE_0BASE: 
								grippingFinger = null;
								if (isActiveGrip) {
										activeGrip = null;
								}
								break;

						}
			
				}

		#endregion

#region introspection

				public bool isActiveGrip{ get {
								return activeGrip && (activeGrip.id == this.id);
						} }
			
				public override string ToString ()
				{
						return "[grip " + id + "]";
				}

				public int id { get { return this.statusButton.index; } }

#endregion

		}
}