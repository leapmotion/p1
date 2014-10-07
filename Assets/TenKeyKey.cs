using UnityEngine;
using System.Collections;

namespace P1
{
		public class TenKeyKey : MonoBehaviour
		{
		
				[SerializeField]
				private float
						m_KeypadUniformScale;

				public float KeypadUniformScale {
						get { return m_KeypadUniformScale; }
						set {
								this.transform.localScale = new Vector3 (value, value, value);
								m_KeypadUniformScale = value;
						}
				}

				[SerializeField]
				private Vector3
						m_KeypadScale;

				public Vector3 KeypadScale {
						get { return m_KeypadScale; }
						set {
								this.transform.localScale = value;
								m_KeypadScale = value;
						}
				}

				public GameObject[] labels = new GameObject[10];
				string label_ = "";

				public string label {
						get {
								return label_;
						}

						set {
								switch (value) {
								case "1": 
										break;
					
					
								case "2": 
										break;
					
					
								case "3": 
										break;
					
					
								case "4": 
										break;
					
					
								case "5": 
										break;
					
					
								case "6": 
										break;
					
					
								case "7": 
										break;
					
					
								case "8": 
										break;
					
					
								case "9": 
										break;
					
					
								case "0": 
										break;
					
					
								default: 
										return;
										break;
								}
								label_ = value;
								foreach (GameObject g in labels) {
										g.SetActive (false);
								}

								try {
										labels [System.Convert.ToInt16 (value)].SetActive (true);
								} catch (UnityException e) {
										Debug.Log ("Cannot set letter for value " + value);
								}

						}

				}
				// Use this for initialization
				void Start ()
				{
	
				}
	
				// Update is called once per frame
				void Update ()
				{
	
				}
		}
}