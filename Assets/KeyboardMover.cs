using UnityEngine;
using System.Collections;

public class KeyboardMover : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	public float speed = 1.0f; //meters per second
	public float spin = 40.0f; //degrees per section
	
	// Update is called once per frame
	void Update()
	{
		//Translation
		if(Input.GetKey(KeyCode.UpArrow))
		{
			//Forward
			transform.Translate(0,0,speed * Time.deltaTime);
		}
		if(Input.GetKey(KeyCode.DownArrow))
		{
			//Backward
			transform.Translate(0,0,-speed * Time.deltaTime);
		}

		//Rotation
		if(Input.GetKey(KeyCode.RightArrow))
		{
			transform.Rotate(0,spin * Time.deltaTime,0);
			//transform.Translate(speed * Time.deltaTime,0,0);
		}
		if(Input.GetKey(KeyCode.LeftArrow))
		{
			transform.Rotate(0,-spin * Time.deltaTime,0);
			//transform.Translate(-speed * Time.deltaTime,0,0);
		}
	}
}
