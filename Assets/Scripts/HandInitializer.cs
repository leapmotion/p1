using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HandInitializer : MonoBehaviour {
  private List<int> hand_IDs_ = new List<int>();

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

  void OnTriggerStay(Collider other)
  {
    // Detect if the collider inside is part of the list of hands that are initialized properly
    for (int i = 0; i < hand_IDs_.Count; ++i)
    {
    }
  }
}
