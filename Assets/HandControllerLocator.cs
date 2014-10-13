using UnityEngine;
using System.Collections;

public class HandControllerLocator : MonoBehaviour
{

		public static HandControllerLocator instance;

		public static HandController handController {
				get { return instance.gameObject.GetComponent<HandController> ();}

		}

		// Use this for initialization
		void Start ()
		{
				instance = this;
		}
}
