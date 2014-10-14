using UnityEngine;
using System.Collections;

namespace P1
{
		public class SimpleDimClamper : MonoBehaviour
		{
				public float min = 0;
				public float max = 0;
				public Utils.Dimension dim = Utils.Dimension.Y;
				public float HUG_RADIUS = 1f;
				public float HUG_FORCE = 5f;
				public GameObject minGameObject;
				public GameObject maxGameObject;
				public GameObject maxIndicator;
				public GameObject minIndicator;
				public float BOUNCE_SCALE = 0.8f;
		
				// Use this for initialization
				void Start ()
				{
						if (minGameObject != null) {
								switch (dim) {
								case Utils.Dimension.Y: // only one for now
										min = minGameObject.transform.position.y;
										break;
								} 
						}
						if (maxGameObject != null) {
								switch (dim) {
								case Utils.Dimension.Y: // only one for now
										max = maxGameObject.transform.position.y;
										break;
								} 
						}
						Debug.Log (string.Format ("SimpleDimClamper clamping between {0} and {1}", min, max));
				}
		
				// Update is called once per frame
				void Update ()
				{
						MoveIndicators ();
						Bounce ();
				}

				void StopMoving (Vector3 p)
				{
						transform.position = p;
						rigidbody.velocity = Vector3.zero;
				}

				void MoveIndicators ()
				{
						BoxCollider c = GetComponent<BoxCollider> ();
						switch (dim) {
						case Utils.Dimension.Y: // only one for now
								if (maxIndicator != null) {
										Vector3 p = maxIndicator.transform.position;
										p.y = c.bounds.max.y;
										maxIndicator.transform.position = p;
								}
								if (minIndicator != null) {
										Vector3 p = minIndicator.transform.position;
										p.y = c.bounds.min.y;
										minIndicator.transform.position = p;
								}
								break;
						}
				}
		
				void Hug ()
				{
						switch (dim) {
						case Utils.Dimension.Y: // only one for now
								Vector3 p = transform.localPosition;
								if (p.y - HUG_RADIUS < min) {
										rigidbody.AddForce (Vector3.up * HUG_FORCE);
								} else if (p.y + HUG_RADIUS > max) {
										rigidbody.AddForce (Vector3.down * HUG_FORCE);
								}
								break;
						}
				}
		
				void Bounce ()
				{
						switch (dim) {
						case Utils.Dimension.Y: // only one for now
								Vector3 p = transform.position;
								BoxCollider c = GetComponent<BoxCollider> ();
								if (c.bounds.min.y < min) { // at bottom
										//		Debug.Log ("At Bottom");
										p.y = min - (c.bounds.min.y - p.y);
										StopMoving (p);
								} else if (c.bounds.max.y > max) { // at top
										//	Debug.Log ("At Top");
										p.y = max - (c.bounds.max.y - p.y);
										StopMoving (p);
								} else { // in the middle
								}
								break;
						}
				}

				void Reflect ()
				{

						switch (dim) {
						case Utils.Dimension.Y: // only one for now
								Vector3 p = rigidbody.velocity;
								p.y *= -BOUNCE_SCALE;
								rigidbody.velocity = p;
								break;
						}
				}
		}
	
}