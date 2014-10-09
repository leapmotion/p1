using UnityEngine;
using System.Collections;

public class HandDetector : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

  void OnTriggerEnter(Collider other)
  {
    Debug.Log(other.gameObject.name);
  }

  void OnTriggerExit(Collider other)
  {
    Debug.Log(other.gameObject.name);
  }
}
