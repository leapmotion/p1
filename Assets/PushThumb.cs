using UnityEngine;
using System.Collections;
using UnityTest;

public class PushThumb : MonoBehaviour
{
		public Vector3 thumbPush = new Vector3 (0, -300, 0);
		public GameObject thumb;
		public float startScoring = 2f;
		bool tellPassed = false;

		// Use this for initialization
		void Start ()
		{
				thumb.rigidbody.AddForce (thumbPush);
		}
	
		// Update is called once per frame
		void Update ()
		{

		}
}
