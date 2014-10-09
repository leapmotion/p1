using UnityEngine;
using System.Collections;

namespace P1
{
		public class SliderDragger : MonoBehaviour
		{

				public float moveIncrement;
				private float moveAmountX;
				private float deltaX;
				public float moveLimit = 1;
				private Vector3 origPos;
				private bool isThisHit = false;
				private int sliderInt;

				private	float snappedXint;
				private float snappedXpos;

				public GameObject HandleVisMesh;
				public GameObject HandleVisGRP;
				
				private SliderManager sliderManager;

				void Start() {

				}
				
				void Awake () {
					sliderManager =  (SliderManager)GameObject.FindObjectOfType (typeof(SliderManager));
					if (sliderManager == null) {
						Debug.LogWarning ("You are missing a Facetopo Manager in the scene.");
					}
				}
		
			
				void OnMouseDown ()
				{
					isThisHit = true;
					moveAmountX = moveIncrement;
					origPos = Input.mousePosition;
					sliderManager.SliderBarHandleMesh.renderer.material = sliderManager.SliderHandleActive;
				}
	
				void Update ()
				{
						if (Input.GetMouseButton (0) && isThisHit == true) {
								deltaX = origPos.x - Input.mousePosition.x;
								moveAmountX = moveIncrement * ((Mathf.Abs (deltaX) * sliderManager.SliderSpeed));
	
								if (Input.mousePosition.x > origPos.x && transform.localPosition.x < moveLimit) {
										Debug.Log ("moveAmountX = " + moveAmountX);
										this.transform.Translate (moveAmountX, 0f, 0f);
								}
								if (Input.mousePosition.x < origPos.x && transform.localPosition.x > 0) {
										this.transform.Translate (-moveAmountX, 0f, 0f);
								}
								float sliderValue = sliderManager.MaxLimit * this.transform.localPosition.x;
								sliderInt = (int)sliderValue;
								sliderManager.TextSliderValue.text = sliderInt.ToString ();
						}
						origPos = Input.mousePosition;

						Vector3 pos = rigidbody.position;
						pos.x = Mathf.Clamp(pos.x, 0.0f, 1.0f * sliderManager.transform.localScale.x);
						rigidbody.position = pos;

						if (rigidbody.position.x < 1.0f  * sliderManager.transform.localScale.x) {
							HandleVisGRP.transform.position = rigidbody.position;
					}
				}
				void FixedUpdate(){
					float sliderValue = sliderManager.MaxLimit * this.transform.localPosition.x;
					sliderInt = (int)sliderValue;
					sliderManager.TextSliderValue.text = sliderInt.ToString ();
				}

				void OnMouseUp ()
				{
					isThisHit = false;
					sliderManager.SliderBarHandleMesh.renderer.material = sliderManager.SliderHandle;
//					SnapToInterval ();
				}
				
				void OnTriggerEnter(){
					sliderManager.SliderBarHandleMesh.renderer.material = sliderManager.SliderHandleActive;
				}
				
				void OnTriggerExit(){
					sliderManager.SliderBarHandleMesh.renderer.material = sliderManager.SliderHandle;
//					SnapToInterval ();
				}
				
				void SnapToInterval () {
					snappedXint = (Mathf.Round (sliderInt / sliderManager.Interval)) * sliderManager.Interval;
					Debug.Log ("snappedXint = " + snappedXint);
					snappedXpos = (1f / sliderManager.MaxLimit) * snappedXint;
					Debug.Log ("snappedXpos = " + snappedXpos);
					
					this.transform.localPosition = new Vector3 (snappedXpos, this.transform.localPosition.y, this.transform.localPosition.z);
					sliderManager.TextSliderValue.text = snappedXint.ToString ();
		
				}
		}
}