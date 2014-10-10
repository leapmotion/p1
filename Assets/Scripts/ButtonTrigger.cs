using UnityEngine;
using System.Collections;

namespace P1
{
		public class ButtonTrigger : MonoBehaviour
		{

				Vector3 original_position;
				Vector3 correct_basis;

        private bool isIndex_ = false;
        private bool readyToPress = true;

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

        public void FingerEntered(bool isIndex)
        {
          isIndex_ = isIndex;
        }

				void OnTriggerEnter (Collider other)
				{
						if (other.gameObject.layer != LayerMask.NameToLayer ("Mouse")) {
              if (other.gameObject.name == "Trigger")
              {
                if (readyToPress)
                {
                  isIndex_ = true; // Allow all fingers to activate the buttons
                  if (isIndex_)
                  {
                    transform.parent.parent.GetComponent<TenKeyKey>().OnTenKeyEvent(true, "Leap");
                    //transform.parent.parent.GetComponent<TenKeyKey>().UpdateColor(Color.cyan);
                  }
                  else
                  {
                    transform.parent.parent.GetComponent<TenKeyKey>().OnTenKeyEvent(false, "Leap");
                  }
                  readyToPress = false;
                }
              } 
              else if (other.gameObject.name == "Cushion") 
              {
                //transform.parent.parent.GetComponent<TenKeyKey>().UpdateColor(Color.gray);
              }
						}
				}

        void OnTriggerExit (Collider other)
				{
          if (other.gameObject.layer != LayerMask.NameToLayer("Mouse"))
          {
            if (other.gameObject.name == "Cushion")
            {
              readyToPress = true;
              //transform.parent.parent.GetComponent<TenKeyKey>().ResetColor();
            }
            else if (other.gameObject.name == "Trigger")
            {
              //transform.parent.parent.GetComponent<TenKeyKey>().UpdateColor(Color.gray);
            }
          }
				}
		}
}
