using UnityEngine;
using System.Collections;

namespace P1
{
		public class Sun : MonoBehaviour
		{

				public LeapButton button;
				public GameObject sunLight;
		public bool isOn = true;

				void OnButtonClick (ButtonEvent e)
				{
						if (e.button.buttonValue == "SunButton") {
								sunLight.SetActive (isOn);
						} else {
								Debug.Log ("Ignoring button " + e.button.buttonValue);
						}
				}

				// Use this for initialization
				void Start ()
				{
						button.ButtonEventBroadcaster += OnButtonClick;
				}
	
				// Update is called once per frame
				void Update ()
				{
	
				}
		}
}