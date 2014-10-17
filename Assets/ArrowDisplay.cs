using UnityEngine;
using System.Collections;

namespace P1
{
		public class ArrowDisplay : MonoBehaviour
		{

				public GameObject a1;
				public GameObject a2;
				public GameObject a3;

				public int level {
						set {
								int i = Mathf.FloorToInt(Mathf.Sqrt (Mathf.Max (0, value)));
								a1.SetActive (i > 0);
								a2.SetActive (i > 1);
								a3.SetActive (i > 2);

						}
				}

				void Start ()
				{
						level = 0;
				}
		}
	
}