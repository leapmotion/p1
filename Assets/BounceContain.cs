using UnityEngine;
using System.Collections;
using SimpleJSON;

namespace P1
{
		public class BounceContain : MonoBehaviour
		{
				const string BOUNCE_STATE = "bounce state";
				const string  BOUNCE_STATE_01_MIDDLE = "middle";
				const  string BOUNCE_STATE_02_BOTTOM = "bottom";
				const  string BOUNCE_STATE_04_BOTTOM_SNAP = "bottom snap";
				const  string BOUNCE_STATE_03_TOP = "top";
				const  string BOUNCE_STATE_05_TOP_SNAP = "top snap";
				public GameObject topIndicator;
				public GameObject bottomIndicator;
				public GameObject middleIndicator;
				public BoxCollider allowableSlideContainer; // the region we are allowed to move in -- not to be confused with our own collider. 
				State bounceState;
				public float upperLimit;
				public float lowerLimit;
// these should be constants -- exposing for tuning
				public float SPRINGINESS = 4f;
				public float SPRINGINESS_NEAR_TARGET = 2f;
				public float DAMPENING = 0.9f;
				public float DAMPENING_NEAR_TARRGET = 0.99f;
				float lastTimeFoundOutsideTaget = 0;
				public float NEARNESS = 0.3f;
				public float TIME_IN_TARGET_TO_STABILIZE = 0.75f;
				public string configFilePath = "";
				public GameObject minIndicator;
				public GameObject maxIndicator;
				public GameObject bMinIndicator;
				public GameObject bMaxIndicator;

#region loop

				// Use this for initialization
				void Start ()
				{

						if (configFilePath != "")
								LoadSpringProps ();

						upperLimit = allowableSlideContainer.bounds.max.y + allowableSlideContainer.gameObject.transform.position.y;
						lowerLimit = allowableSlideContainer.bounds.min.y + allowableSlideContainer.gameObject.transform.position.y;
						Vector3 p;
						if (maxIndicator != null) {
								p = maxIndicator.transform.position;
								maxIndicator.transform.position = new Vector3 (p.x, upperLimit, p.z);
						}
						if (minIndicator != null) {
								p = minIndicator.transform.position;
								minIndicator.transform.position = new Vector3 (p.x, lowerLimit, p.z);
						}
			
						//	Debug.Log (string.Format ("springTarget upperLimit{0}, lowerLimit {1} for obect at {2}", lowerLimit, upperLimit, transform.position.y));
						InitBounceStateList ();
						bounceState = new State (BOUNCE_STATE, BOUNCE_STATE_01_MIDDLE);

						bounceState.StateChangedEvent += OnChange;
				}
	
				// Update is called once per frame
				void Update ()
				{			
			
						if (bounceState.state == BOUNCE_STATE_01_MIDDLE) {
								TestMiddleBounds ();
						} else {
								if (!IsNearTarget ()) {
										lastTimeFoundOutsideTaget = Time.time;
								}

								ShowTopBottomBounds ();
						}
				}

				void FixedUpdate ()
				{
						if (bounceState.state != BOUNCE_STATE_01_MIDDLE) {
								Spring ();
						}
			
				}
		
		#endregion

#region spring

				public void ShowTopBottomBounds ()
				{
						Vector3 p = transform.position;
						if (bMaxIndicator) {
								p.y = top;
								bMaxIndicator.transform.position = p;
						}
						if (bMinIndicator) {
								p.y = bottom;
								bMinIndicator.transform.position = p;
						}

				}
		
				public void LoadSpringProps ()
				{
						LoadSpringProps (configFilePath);
				}

				public void LoadSpringProps (string cfp)
				{
						JSONNode n = Utils.FileToJSON (cfp);
						SPRINGINESS = n ["springiness"].AsFloat;
						SPRINGINESS_NEAR_TARGET = n ["springiness_near_target"].AsFloat;
						DAMPENING = n ["dampening"].AsFloat;
						DAMPENING_NEAR_TARRGET = n ["dampening_near_target"].AsFloat;
				}

				float timeInTarget { get { return Time.time - lastTimeFoundOutsideTaget; } }

				bool IsNearTarget ()
				{
						return Mathf.Abs (transform.position.y - springTarget) < NEARNESS;
				}

				float springTarget {
						get {
								float o;
								switch (bounceState.state) {

								case BOUNCE_STATE_02_BOTTOM:
										o = lowerLimit;
										break;

								case BOUNCE_STATE_04_BOTTOM_SNAP: 
										o = lowerLimit;
										break;

								case  BOUNCE_STATE_03_TOP:
										o = upperLimit;
										break;

								case BOUNCE_STATE_05_TOP_SNAP:
										o = upperLimit;
										break;

								default:
										Debug.Log ("Why do you want to know the spring target if you are in the middle?");
										o = 0f;
										break;
								}
								return o;
						}
				}
		
				void Spring ()
				{
						SpringNear (springTarget);
						if (Utils.Elapsed (lastTimeFoundOutsideTaget, TIME_IN_TARGET_TO_STABILIZE)) {
								switch (bounceState.state) {
								case BOUNCE_STATE_03_TOP:
										bounceState.Change (BOUNCE_STATE_05_TOP_SNAP);
										break;
								case BOUNCE_STATE_02_BOTTOM:
										bounceState.Change (BOUNCE_STATE_04_BOTTOM_SNAP);
										break;

								}
						}
				}

				void SpringNear (float y)
				{
						Vector3 edgePosition = transform.position * 1;

						switch (bounceState.state) {
						case BOUNCE_STATE_02_BOTTOM:
								edgePosition.y = bottom;
								break;
				
						case BOUNCE_STATE_04_BOTTOM_SNAP: 
								edgePosition.y = bottom;
								break;
				
						case  BOUNCE_STATE_03_TOP:
								edgePosition.y = top;
								break;
				
						case BOUNCE_STATE_05_TOP_SNAP:
								edgePosition.y = top;
								break;
						}

						Vector3 springCenter = transform.position * 1;
						springCenter.y = y;
						
						Vector3 force = springCenter - edgePosition;
						rigidbody.AddForce (force * (IsNearTarget () ? SPRINGINESS_NEAR_TARGET : SPRINGINESS));
						rigidbody.velocity *= (IsNearTarget () ? DAMPENING_NEAR_TARRGET : DAMPENING);

				}
		
		#endregion
		
		#region reflection
		
				public float top { get { return collider.bounds.max.y; } }
		
				public float bottom { get { return collider.bounds.min.y; } }

#endregion
	
#region state

				void TestMiddleBounds ()
				{
						if (false)
								Debug.Log (string.Format ("at {0} -- testing top ({1}) against ub {2} and bottom {3}) against bb {4}",
transform.position.y,
							top, upperLimit, bottom, lowerLimit));
						if (top > upperLimit) {
								bounceState.Change (BOUNCE_STATE_03_TOP);
						} else if (bottom < lowerLimit) {
								bounceState.Change (BOUNCE_STATE_02_BOTTOM);
						} else {
								bounceState.Change (BOUNCE_STATE_01_MIDDLE);
						}
				}

				void InitBounceStateList ()
				{
						if (!StateList.HasList (BOUNCE_STATE)) {
								StateList.Create (BOUNCE_STATE, 
									BOUNCE_STATE_01_MIDDLE, 
									BOUNCE_STATE_02_BOTTOM, 
									BOUNCE_STATE_03_TOP,
									BOUNCE_STATE_04_BOTTOM_SNAP, 
									BOUNCE_STATE_05_TOP_SNAP
								);
						}
				}

				void OnChange (StateChange c)
				{
						if (c.unchanged)
								return;
						Debug.Log (">>>>>> State change: " + c.state);
						Debug.Log (string.Format ("Top: {0}; Bottom: {1}; upperLimit: {2}; lowerLimit {3}", top, bottom, upperLimit, lowerLimit));
						if (c.state != BOUNCE_STATE_01_MIDDLE)
								lastTimeFoundOutsideTaget = Time.time;
						
						if (bottomIndicator != null)
								bottomIndicator.SetActive (c.state == BOUNCE_STATE_02_BOTTOM);
						if (topIndicator != null)
								topIndicator.SetActive (c.state == BOUNCE_STATE_03_TOP);
						if (middleIndicator != null)
								middleIndicator.SetActive (c.state == BOUNCE_STATE_01_MIDDLE 
										|| c.state == BOUNCE_STATE_04_BOTTOM_SNAP 
										|| c.state == BOUNCE_STATE_05_TOP_SNAP); 
				}

		#endregion

		}

}
