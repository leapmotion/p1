using UnityEngine;
using System.Collections;

public class HandManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
    SetActive(false);
	}

  public void SetActive(bool status)
  {
    Collider[] colliders = GetComponentsInChildren<Collider>();
    foreach (Collider collider in colliders)
    {
      collider.isTrigger = !status;
    }
  }
}
