using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ButtonMonkey;
using Leap;

namespace P1
{
		public class FingerData
		{
				public float time;

				public	FingerModel fingerModel {
						set {
								Finger lf = value.GetLeapFinger ();
								fingerId = lf.Id;
								Debug.Log (string.Format ("Creating fingerModel for Finger id {0} (name {1}) ", fingerId.ToString (), value.gameObject.name));
						}
						get {
								foreach (object o in Object.FindObjectsOfType(typeof(FingerModel))) {
										FingerModel f = (FingerModel)o;
										Finger finger = f.GetLeapFinger ();
										if (finger.Id == fingerId) {
												return f;
										}
								}
								Debug.Log (string.Format ("Cannot find finger id {0}", fingerId));
								return null;
						}
				}

				int fingerId;

				public FingerData (BoneTracker tracker, GameObject g)
				{
						time = tracker.time.now;
						bool inEditor = false;
                         
						#if UNITY_EDITOR
                        inEditor = true;
#endif
						if (inEditor)
								return;
						fingerModel = g.GetComponentInParent<FingerModel> ();
						if (fingerModel == null)
								throw new UnityException ("Cannot find finger for FingerData");
				}
		}

		public struct CountChangedRecord
		{
				public int oldCount;
				public int newCount;
				public BoneTracker tracker;

				public CountChangedRecord (int o, int n, BoneTracker t)
				{
						oldCount = o; 
						newCount = n;
						tracker = t;
				}
		}

/**
 * this class exists to track the number of bones in the scene 
 * to determine whether or not a hand is in this.
*/

		public class BoneTracker: MonoBehaviour
		{
				public Dictionary<int, FingerData> fingersInMe = new Dictionary<int, FingerData> ();
				public GlobalTime time = new GlobalTime ();
				public const float MAX_BONE_STALE_TIME = 0.5f;
		
		#region leap interaction
		
				void OnTriggerEnter (Collider c)
				{
						TallyBone (c);
				}
		
				void OnTriggerExit (Collider c)
				{
						TallyBone (c, false);
				}
		
				void OnTriggerStay (Collider c)
				{
						TallyBone (c);
				}
		
				public void TallyBone (Collider c, bool enter = true)
				{
						TallyGameObject (c.gameObject, enter);
				}

				public void TallyGameObject (GameObject g, bool enter = true)
				{
						switch (g.name) {
						/* case "bone1": 
								break; */
				
						/*	case "bone2": 
								break; */
				
						case "bone3":
								break;
				
						default:
								return;
								break;
						}
			
						if (enter)
								AddBone (g);
						else 
								RemoveBone (g);
				}
/**
 * note - even if the bone exists we need to add new fingerData to change the time. 
*/
				void AddBone (GameObject g)
				{
						int oldCount = boneCount;
						if (fingersInMe.ContainsKey (g.GetInstanceID ())) {
								fingersInMe.Remove (g.GetInstanceID ());
								fingersInMe.Add (g.GetInstanceID (), new FingerData (this, g));
// no change in count; don't tally
						} else {
								fingersInMe.Add (g.GetInstanceID (), new FingerData (this, g));
								CountChanged (oldCount, boneCount);
						}
				}
		
				void RemoveBone (GameObject g)
				{
						RemoveBone (g.GetInstanceID ());
				}

				void RemoveBone (int i)
				{
						int oldCount = boneCount;
						if (fingersInMe.ContainsKey (i)) {
								fingersInMe.Remove (i);
								CountChanged (oldCount, boneCount);
						} else {
								// no change in count; don't tally
						}
				}
		
		#endregion
		
		#region Events
		
				public delegate void CountChangedDelegate (CountChangedRecord c);
		
				/// <summary>An event that gets fired </summary>
				public event CountChangedDelegate CountChangedEvent;
		
				public void CountChanged (int oldCount, int newCount)
				{
						if (CountChangedEvent != null) // fire the event
								CountChangedEvent (new CountChangedRecord (oldCount, newCount, this));
				}
		
		#endregion

		#region report

				public string Report ()
				{
						string s = "--- state at " + time.now + " seconds: " + boneCount + " bones \n";

						foreach (int index in fingersInMe.Keys) {
								s += ReportItem (index, fingersInMe [index].time);
						}
						return s;
				}

				string ReportItem (int index, float t)
				{
						return " id " + index + ": recorded at " + t + "(" + (time.now - t) + " seconds ago) " + (IsOld (t) ? " (old)" : " - active") + "\n";
				}

		#endregion

		#region loop
		
				void Start ()
				{
				}
		
				void Update ()
				{
						time.now = Time.time;
						RetireOldBones ();
				}

		#endregion

				bool IsOld (float t)
				{
						return (t + MAX_BONE_STALE_TIME) < time.now;
				}

				public int boneCount { get { return fingersInMe.Count; } }

/**
 Because of the rare but possible case of a hand being removed between frames and thus possibly not 
 triggering a leave condiition, all bones not refreshed within MAX_BONE_STALE_TIME 
 are removed.
*/

				public void RetireOldBones ()
				{
						List<int> killMe = new List<int> ();

						foreach (int index in fingersInMe.Keys) {
								float lastEncounteredTime = fingersInMe [index].time;
								if (IsOld (lastEncounteredTime))
										killMe.Add (index);
						}
						foreach (int i in killMe) {
								RemoveBone (i);
						}
				}
		}
}