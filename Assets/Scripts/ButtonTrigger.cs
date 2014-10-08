using UnityEngine;
using System.Collections;

namespace P1
{
		public class ButtonTrigger : MonoBehaviour
		{

				Vector3 original_position;
				Vector3 correct_basis;

				// Use this for initialization
				void Start ()
				{
						Vector3 trigger_position = transform.parent.FindChild ("Trigger").transform.position;
						original_position = transform.position;
						correct_basis = trigger_position - original_position;
				}

				// Update is called once per frame
				void Update ()
				{
						Vector3 curr_basis = transform.position - original_position;
						Vector3 adjusted_basis = Vector3.Project (curr_basis, correct_basis);
						transform.position = adjusted_basis + original_position;
				}

				void OnTriggerEnter (Collider other)
				{
						if (other.gameObject.layer != LayerMask.NameToLayer ("Mouse")) {
								transform.parent.parent.GetComponent<TenKeyKey> ().OnTenKeyEvent ("Leap");
						}
				}
		}
}
