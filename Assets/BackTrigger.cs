using UnityEngine;
using System.Collections;

namespace P1
{
		public class BackTrigger : MonoBehaviour
		{

				public Clutch scrollBox;

				// Use this for initialization
				void Start ()
				{
	
				}
	
				// Update is called once per frame
				void Update ()
				{
	
				}

				public void OnTriggerEnter (Collider c)
				{
						UnityEngine.Debug.Log ("OnTriggerEnter");
						scrollBox.BounceLock ();
				}
		}
}
