using UnityEngine;
using System.Collections;

namespace P1 
{
  public class IndexDetector : MonoBehaviour {

    public ButtonTrigger buttonTrigger;
    private int active_fingers_ = 0;

    private bool isHand(Collider other)
    {
      return (other.transform.parent && other.transform.parent.parent && other.transform.parent.parent.GetComponent<HandModel>());
    }

	  // Use this for initialization
	  void Start () {
	  }
	
	  // Update is called once per frame
	  void FixedUpdate () {
      if (collider.enabled == true)
      {
        if (active_fingers_ > 0)
        {
          buttonTrigger.FingerEntered(true);
        }
        else
        {
          buttonTrigger.FingerEntered(false);
        }
        active_fingers_ = 0;
      }
	  }

    void OnTriggerStay(Collider other)
    {
      if (isHand(other))
      {
        active_fingers_++;
      } 
    }
  }
}
