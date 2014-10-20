using UnityEngine;
using System.Collections;

public class KeyboardMover : MonoBehaviour
{
		public OVRCameraController oculus;

		// Use this for initialization
		void Start ()
		{
	
		}
	
		public float speed = 1.0f; //meters per second
		public float spin = 40.0f; //degrees per section
	
		// Update is called once per frame
		void Update ()
		{
				//Translation
				if (Input.GetKey (KeyCode.UpArrow)) {
						//Forward
						transform.Translate (0, 0, speed * Time.deltaTime);
				}
				if (Input.GetKey (KeyCode.DownArrow)) {
						//Backward
						transform.Translate (0, 0, -speed * Time.deltaTime);
				}

				//Rotation
				if (Input.GetKey (KeyCode.RightArrow)) {
						transform.Rotate (0, spin * Time.deltaTime, 0);
						//transform.Translate(speed * Time.deltaTime,0,0);
				}
				if (Input.GetKey (KeyCode.LeftArrow)) {
						transform.Rotate (0, -spin * Time.deltaTime, 0);
						//transform.Translate(-speed * Time.deltaTime,0,0);
				}
				if (oculus) {
						//Recentering
						//TODO: This should trigger a display of movement direction
						if (Input.GetKey (KeyCode.RightShift)) {
								UnityEngine.Debug.Log ("Recentering... right");
								oculus.SetOrientationOffset (new Quaternion (0, spin * Time.deltaTime, 0, 0));
						}
						if (Input.GetKey (KeyCode.LeftShift)) {
								UnityEngine.Debug.Log ("Recentering... left");
								oculus.SetOrientationOffset (new Quaternion (0, -spin * Time.deltaTime, 0, 0));
						}
				}
		}
}
