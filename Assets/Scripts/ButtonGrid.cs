using UnityEngine;
using System.Collections;

public class ButtonGrid : MonoBehaviour {

  //private GameObject button_;

  private float button_size_ = 1.0f;
  private Vector3 button_location_ = new Vector3(0, 0, 5);
  private float button_spacing_ = 1.0f;
  private float visual_depth_ = 5.0f;
  private float activation_depth_ = 1.0f;
  private Color button_color_ = Color.black;

	// Use this for initialization
	void Start () {
    GameObject back_panel = GameObject.CreatePrimitive(PrimitiveType.Cube);
    back_panel.transform.position = Vector3.zero;
	}
	
	// Update is called once per frame
	void Update () {
	}
}
