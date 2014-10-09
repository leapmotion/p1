using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ButtonMonkey;

namespace P1
{
		struct FingerData
		{
				public float time;
				public	FingerModel fingerModel;

				public FingerData (BoneTracker tracker, GameObject g)
				{
						time = tracker.time.now;
						fingerModel = g.GetComponent<FingerModel> ();
			
				}
		}

/**
 * this class exists to track the number of bones in the scene 
 * to determine whether or not a hand is in this.
*/

		public class BoneTracker: MonoBehaviour
		{
				Dictionary<int, FingerData> bonePresence = new Dictionary<int, FingerData> ();
				public GlobalTime time = new GlobalTime ();
				public const float MAX_BONE_STALE_TIME = 0.5f;
		
		#region leap interaction
		
				void OnTriggerEnter (Collider c)
				{
						TallyBone (c);
				}
		
				void OnTriggerLeave (Collider c)
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
						case "bone1": 
								break;
				
						case "bone2": 
								break;
				
						case "bone3":
								break;
				
						default:
								return;
								break;
						}
			
						if (enter)
								bonePresence.Add (g.GetInstanceID (), new FingerData (this, g));
						else
								bonePresence.Remove (g.GetInstanceID ());
				}

				public string Report ()
				{
						string s = "--- state at " + time.now + " seconds: " + boneCount + " bones \n";

						foreach (int index in bonePresence.Keys) {
								s += ReportItem (index, bonePresence [index].time);
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

				public int boneCount { get { return bonePresence.Count; } }

/**
 Because of the rare but possible case of a hand being removed between frames and thus possibly not 
 triggering a leave condiition, all bones not refreshed within MAX_BONE_STALE_TIME 
 are removed.
*/

				public void RetireOldBones ()
				{
						List<int> killMe = new List<int> ();

						foreach (int index in bonePresence.Keys) {
								float lastEncounteredTime = bonePresence [index].time;
								if (IsOld (lastEncounteredTime))
										killMe.Add (index);
						}
						foreach (int i in killMe) {
								bonePresence.Remove (i);
						}
				}
		}
}