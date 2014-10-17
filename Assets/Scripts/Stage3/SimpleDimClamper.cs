using UnityEngine;
using System.Collections;
using TMPro;

namespace P1
{
		public class SimpleDimClamper : MonoBehaviour
		{
// the dimension of constraint
				public Utils.Dimension dim = Utils.Dimension.Y;
// the boundries of motion - absolute and unchanging over time
				public float min = 0;
				public float max = 0;

// objects that if set, define min and max values
				public GameObject ceilingObject;
				public GameObject floorObject;

#region dynamic size indicators
		
				// the dyamic bounds -- usually a local component or a collider attached to a child object, as it must move with this object to be useable.
				BoxCollider c; 
		
				// objects that in the absence of a boxCollider, define the dynamic top and bottom; should be children of this object.
				public GameObject topObject;
				public GameObject bottomObject;

// values -- in the absence of the above objects, offset the transform by these values
				public float minOffset = 0;
				public float maxOffset = 0;
#endregion

// debugger affordances
				public GameObject maxIndicator;
				public GameObject minIndicator;
				public TextMeshPro feedback;

#region extents
		
				public float top {
						get {
								if (c == null) {
										return topObject != null ? topObject.transform.position.y : transform.position.y + minOffset;
								} else {
										return c.bounds.max.y;
								}
						}
				}
		
				public float bottom {
						get {
								if (c == null) {
										return bottomObject != null ? bottomObject.transform.position.y : transform.position.y + minOffset;
								} else {
										return c.bounds.min.y;
								}
						}
				}
		
				public float bottomDepth {
						get { return bottom - transform.position.y; }
				}
		
				public float topDepth {
						get { return transform.position.y - top; }
				}
		
#endregion

		
				// Use this for initialization
				void Start ()
				{
						if (ceilingObject != null) {
								switch (dim) {
								case Utils.Dimension.Y: // only one for now
										min = ceilingObject.transform.position.y;
										break;
								} 
						}
						if (floorObject != null) {
								switch (dim) {
								case Utils.Dimension.Y: // only one for now
										max = floorObject.transform.position.y;
										break;
								} 
						}
						Debug.Log (string.Format ("SimpleDimClamper clamping between {0} and {1}", min, max));
				}
		
				// Update is called once per frame
				void Update ()
				{
						MoveIndicators ();
						HaltAtBoundry ();
				}

				void StopMoving (Vector3 p)
				{
						transform.position = p;
						rigidbody.velocity = Vector3.zero;
				}

				void MoveIndicators ()
				{
						Vector3 p;
						switch (dim) {
						case Utils.Dimension.Y: // only one for now
								if (maxIndicator != null) {
										p = maxIndicator.transform.position;
										p.y = top;
										maxIndicator.transform.position = p;
								}
								if (minIndicator != null) {
										p = minIndicator.transform.position;
										p.y = bottom;
										minIndicator.transform.position = p;
								}
								break;
						}
				}

				object Inty (float top)
				{
						return Mathf.RoundToInt (top * 100);
				}
		
				void HaltAtBoundry ()
				{
						Vector3 p = transform.position;
						string msg = string.Format (
				"y: {0}\n top = {1}\n bottom: {2}\n min: {3}\n max: {4}",
				Inty (p.y), Inty (top), Inty (bottom), Inty (min), Inty (max));

						if (feedback != null)
								feedback.text = msg;
						else
								Debug.Log (msg);
						switch (dim) {
						case Utils.Dimension.Y: // only one for now
								if (bottom < min) { // at bottom
										Debug.Log ("At Bottom");
										while (bottom <= min) {
												p.y += 0.01f;
												transform.position = p;
										}
										StopMoving (p);
								} else if (top > max) { // at top
										Debug.Log ("At Top");
										while (top >= max) {
												p.y -= 0.01f;
												transform.position = p;
										}
										StopMoving (p);
								} else { 
// in the middle
								}
								break;

						default:
								Debug.Log ("Bad dimension: cannot handle dim " + dim);
								break;
						}
				}

		}
}