using UnityEngine;
using System.Collections;

namespace P1 
{
  public class IndexDetector : MonoBehaviour {

    public ButtonTrigger buttonTrigger;

	  // Use this for initialization
	  void Start () {
	  }
	
	  // Update is called once per frame
	  void Update () {
	
	  }

    void OnTriggerEnter(Collider other)
    {
      if (other.transform.parent.name == "index")
      {
        buttonTrigger.FingerEntered(true);
      }
      else
      {
        buttonTrigger.FingerEntered(false);
      }
    }
  }
}
