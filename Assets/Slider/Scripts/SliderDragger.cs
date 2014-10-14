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
				private int prevSliderInt;
				public int sliderInt;

				private	float snappedXint;
				private float snappedXpos;

				public GameObject HandleVisMesh;
				public GameObject HandleVisGRP;
				
				public AudioSource sliderClickSound;
				
				private SliderManager sliderManager;
		
				void Start() {

				}
				
				void Awake () {
					sliderManager =  (SliderManager)GetComponentInParent (typeof(SliderManager));
					if (sliderManager == null) {
						Debug.LogWarning ("You are missing a Slider Manager in the scene.");
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
						prevSliderInt = sliderInt;
//						if (Input.GetMouseButton (0) && isThisHit == true) {
//								deltaX = origPos.x - Input.mousePosition.x;
//								moveAmountX = moveIncrement * ((Mathf.Abs (deltaX) * sliderManager.SliderSpeed));
//	
//								if (Input.mousePosition.x > origPos.x && transform.localPosition.x < moveLimit) {
//										Debug.Log ("moveAmountX = " + moveAmountX);
//										this.transform.Translate (moveAmountX, 0f, 0f);
//								}
//								if (Input.mousePosition.x < origPos.x && transform.localPosition.x > 0) {
//										this.transform.Translate (-moveAmountX, 0f, 0f);
//								}
//								float sliderValue = sliderManager.MaxLimit * this.transform.localPosition.x;
//								sliderInt = (int)(sliderValue);
//								sliderManager.TextSliderValue.text = sliderInt.ToString ();
//						}
//						origPos = Input.mousePosition;

						Vector3 pos = rigidbody.position;
						pos.x = Mathf.Clamp(pos.x, 0.0f, 1.0f * sliderManager.transform.localScale.x);
						rigidbody.position = pos;

						if (rigidbody.position.x < 1.0f  * sliderManager.transform.localScale.x) {
							HandleVisGRP.transform.position = rigidbody.position;
						}

		}
		void FixedUpdate(){
			float sliderValue = sliderManager.MaxLimit * this.transform.localPosition.x;
					sliderInt = (int)(sliderValue +.5f);
					sliderManager.TextSliderValue.text = sliderInt.ToString ();
					if(prevSliderInt != sliderInt){
//						Debug.Log("Click Sound");
						sliderClickSound.Play();
					}
		}
		
		void OnMouseUp ()
				{
					isThisHit = false;
					sliderManager.SliderBarHandleMesh.renderer.material = sliderManager.SliderHandle;
					SnapToInterval ();
				}
				
				void OnTriggerEnter(){
//					StopCoroutine("hiLightPause");
					StopAllCoroutines();
					sliderManager.SliderBarHandleMesh.renderer.material = sliderManager.SliderHandleActive;
				}
				
				void OnTriggerExit(){
					StartCoroutine(hiLightPause());
				}
				
				private IEnumerator hiLightPause () {
					yield return new WaitForSeconds (.2f);
					sliderManager.SliderBarHandleMesh.renderer.material = sliderManager.SliderHandle;
					SnapToInterval ();
			
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