using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HandInitializer : MonoBehaviour {
  private List<int> hand_IDs_ = new List<int>();

  private HandManager getHand(Collider other)
  {
    HandManager hand_manager = null;
    if (other.transform.parent && other.transform.parent.parent && other.transform.parent.parent.GetComponent<HandManager>()) 
    {
      hand_manager = other.transform.parent.parent.GetComponent<HandManager>();
    }
    return hand_manager;
  }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

  void OnTriggerStay(Collider other)
  {
    // Detect if the collider inside is part of the list of hands that are initialized properly
    HandManager hand_manager = getHand(other);
    if (hand_manager != null)
    {
      hand_manager.SetActive(true);
    }
  }
}
