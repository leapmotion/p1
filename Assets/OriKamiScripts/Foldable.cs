using UnityEngine;
using System.Collections;

//FIXME: Apply this when more than one HingeJoint is present
public class Foldable : MonoBehaviour
{
		static float maxDiff = 270.0f; //Maximum single step in angle. Should be in (180, 360)
		public float filterTime = 0.0f; //Time holding a position for a fold to set
		public float foldAngle; //Filtered angle of fold in degrees

		/// <summary>
		/// Initializes crease parameters
		/// </summary>
		void Start ()
		{
				foldAngle = 0.0f;
		}
	
		/// <summary>
		/// Maintains a rolling avergage of bending angle
		/// </summary>
		void Update ()
		{
				if (hingeJoint == null)
						return;

				//Calculate target angle, with push-through prevented
				//ASSUME: Angle range is (-180, 180] degrees
				//NOTE: Velocity is not necessarily consistent with position differential
				float angleDiff = foldAngle - hingeJoint.angle;

				if (maxDiff < angleDiff) {
						Debug.Log ("name(" + name + "): maxDiff < angleDiff = " + angleDiff + 
								"\n foldAngle = " + foldAngle +
								"\n hingeJoint.angle = " + hingeJoint.angle);
						angleDiff = foldAngle - 180.0f;
				}
				if (- maxDiff > angleDiff) {
						Debug.Log ("name(" + name + "): -maxDiff > angleDiff = " + angleDiff + 
								"\n foldAngle = " + foldAngle +
								"\n hingeJoint.angle = " + hingeJoint.angle);
						angleDiff = foldAngle + 180.0f;
				}

				//Update rolling average
				float filter = 0.0f;
				if (filterTime > 0.0f) {
						filter = Mathf.Exp (-Time.deltaTime / filterTime);
						Debug.Log ("name(" + name + "): hingeJoint.angle = " + hingeJoint.angle + 
								"\nfoldAngle = " + ((filter * angleDiff) + hingeJoint.angle).ToString ());
				}
				foldAngle = (filter * angleDiff) + hingeJoint.angle;

				//Update angle target
				JointSpring spring = hingeJoint.spring;
				spring.targetPosition = foldAngle;
				hingeJoint.spring = spring;
		}
}
