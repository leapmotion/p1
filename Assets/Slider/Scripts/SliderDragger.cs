using UnityEngine;
using System.Collections;

public class SliderDragger : MonoBehaviour {

	public float moveIncrement;
	private float moveAmountX;

	private float deltaX;
	public float moveLimit = 1;

	private Vector3 origPos;
	private bool isThisHit = false;

	private int sliderInt;



	void OnMouseDown (){
		isThisHit = true;
		moveAmountX = moveIncrement;
		origPos = Input.mousePosition;
		SliderManager.Instance.SliderBarHandleMesh.renderer.material = SliderManager.Instance.SliderHandleActive;
	}
	
	void Update () {
		if(Input.GetMouseButton(0) && isThisHit == true){
			deltaX = origPos.x - Input.mousePosition.x;
			moveAmountX = moveIncrement * ((Mathf.Abs(deltaX) * SliderManager.Instance.SliderSpeed));

			if(Input.mousePosition.x > origPos.x && transform.localPosition.x < moveLimit){
				Debug.Log("moveAmountX = " + moveAmountX);
				this.transform.Translate(moveAmountX, 0f, 0f);
			}
			if(Input.mousePosition.x < origPos.x && transform.localPosition.x > 0){
				this.transform.Translate(-moveAmountX, 0f, 0f);
			}
			float sliderValue = SliderManager.Instance.MaxLimit * this.transform.localPosition.x ;
			sliderInt = (int)sliderValue;
			SliderManager.Instance.TextSliderValue.text = sliderInt.ToString();
		}
		origPos = Input.mousePosition;
	}

	void OnMouseUp () {
		isThisHit = false;
		SliderManager.Instance.SliderBarHandleMesh.renderer.material = SliderManager.Instance.SliderHandle;

		float snappedXint = (Mathf.Round (sliderInt / SliderManager.Instance.Interval)) * SliderManager.Instance.Interval;
		Debug.Log ("snappedXint = " + snappedXint);
		float snappedXpos = (1f / SliderManager.Instance.MaxLimit) * snappedXint;
		Debug.Log ("snappedXpos = " + snappedXpos);

		this.transform.localPosition = new Vector3 (snappedXpos, this.transform.localPosition.y, this.transform.localPosition.z);
		SliderManager.Instance.TextSliderValue.text = snappedXint.ToString();

	}
}
