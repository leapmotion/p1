using UnityEngine;
using System.Collections;

public class IndexFingerSprite : MonoBehaviour {
	float startTime;
	const float lifeSpan = 1f; //Units: seconds

	// Use this for initialization
	void Start () {
		startTime = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
	if (Time.time - lifeSpan > startTime) {
			gameObject.SetActive(false); //Timed destruction
				}
	}
}
