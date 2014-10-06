using UnityEngine;
using System.Collections;

public class SceneManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
    // Implement Fading for easier transition
    if (Input.GetKeyDown(KeyCode.Alpha1))
    {
      Application.LoadLevel("Stage1");
    }
    else if (Input.GetKeyDown(KeyCode.Alpha2))
    {
      Application.LoadLevel("Stage2");
    }
    else if (Input.GetKeyDown(KeyCode.Alpha3))
    {
      Application.LoadLevel("Stage3"); 
    }
	}
}
