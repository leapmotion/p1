using UnityEngine;
using System.Collections;
using SimpleJSON;

namespace P1
{
	public class SliderManager : MonoBehaviour
	{
		public string ConfigFileName = "slider_config.json";


		[SerializeField]
		private int
				m_MinLimit;

		public int MinLimit {
				get { return m_MinLimit; }
				set {
						m_MinLimit = value;
						TextLowerLimit.text = m_MinLimit.ToString ();
				}
		}

		[SerializeField]
		private int
				m_MaxLimit;

		public int MaxLimit {
				get { return m_MaxLimit; }
				set {
						m_MaxLimit = value;
						TextUpperLimit.text = m_MaxLimit.ToString ();
				}
		}

		public int Interval;
		public TextMesh TextLowerLimit;
		public TextMesh TextUpperLimit;
		public TextMesh TextSliderValue;
		[SerializeField]
		private float
				m_WidgetWidth;

		public float WidgetWidth {
				get { return m_WidgetWidth; }
				set {
						this.transform.localScale = new Vector3 (value, this.transform.localScale.y, this.transform.localScale.z);
						m_WidgetWidth = value;
				}
		}

		[SerializeField]
		private float
				m_WidgetHeight;

		public float WidgetHeight {
				get { return m_WidgetHeight; }
				set {
						this.transform.localScale = new Vector3 (this.transform.localScale.x, value, this.transform.localScale.z);
						m_WidgetHeight = value;
				}
		}
		[SerializeField]
		private float
			m_WidgetDepth;
		
		public float WidgetDepth {
			get { return m_WidgetDepth; }
			set {
				this.transform.localScale = new Vector3 (this.transform.localScale.x, this.transform.localScale.y, value);
				m_WidgetDepth = value;
			}
		}
		
		//public float Xpos;
		//public float Ypos;
		[SerializeField]
		private float
			m_SliderHandleWidth;
		
		public float SliderHandleWidth {
			get { return m_SliderHandleWidth; }
			set {
				SliderHandleGRP.transform.localScale = new Vector3 (value, SliderHandleGRP.transform.localScale.y, SliderHandleGRP.transform.localScale.z );
				SliderHandleVisGRP.transform.localScale = new Vector3 (value, SliderHandleVisGRP.transform.localScale.y, SliderHandleVisGRP.transform.localScale.z );
				m_SliderHandleWidth = value;
			}
		}

		[SerializeField]
		private float
				m_SliderHandleHeight;

		public float SliderHandleHeight {
			get { return m_SliderHandleHeight; }
			set {
				SliderHandleGRP.transform.localScale = new Vector3 (SliderHandleGRP.transform.localScale.x, value, SliderHandleGRP.transform.localScale.z);
				SliderHandleVisGRP.transform.localScale = new Vector3 (SliderHandleVisGRP.transform.localScale.x, value, SliderHandleVisGRP.transform.localScale.z);
				m_SliderHandleHeight = value;
			}
		}

		[SerializeField]
		private float
				m_SliderHandleDepth;

		public float SliderHandleDepth {
				get { return m_SliderHandleDepth; }
				set {
				SliderHandleGRP.transform.localScale = new Vector3 (SliderHandleGRP.transform.localScale.x, SliderHandleGRP.transform.localScale.y, value);
				SliderHandleVisGRP.transform.localScale = new Vector3 (SliderHandleVisGRP.transform.localScale.x, SliderHandleVisGRP.transform.localScale.y, value);
				m_SliderHandleDepth = value;
				}
		}

		
		public GameObject SliderHandleGRP;
		public GameObject SliderHandleVisGRP;
		
		public GameObject SliderBarMesh;
		public GameObject SliderBarHandleMesh;
		public Material SliderHandle;
		public Material SliderHandleActive;
		public float SliderSpeed;
		public bool TicksOnOff;
		public GameObject TickGRP;
		
		public TextMesh TextNumberPrompt;
		public Material TextNumberBack;


		// Use this for initialization
		void Start ()
		{
			InitializeSlider ();
		}
		
		public void InitializeSlider () {

			JSONNode n = Utils.FileToJSON(ConfigFileName);
			MinLimit = n ["min"].AsInt;
			Debug.Log ("Min = " + MinLimit);
			
			MaxLimit = n ["max"].AsInt;
			Debug.Log ("MaxLimit = " + MaxLimit);
			Interval = n ["interval"].AsInt;
			
			if (TicksOnOff == true) {
				BuildTicks ();
			}
			
			WidgetWidth = n ["widgetXscale"].AsFloat;
			WidgetHeight = n ["widgetYscale"].AsFloat;
			WidgetDepth = n ["widgetZscale"].AsFloat;
			
			SliderHandleWidth = n ["sliderHandleWidth"].AsFloat;
			SliderHandleHeight = n ["sliderHandleWidth"].AsFloat;
			SliderHandleDepth = n ["sliderHandleWidth"].AsFloat;
		}
		
		public IEnumerator BuildTicks (){
//			yield return new WaitForSeconds(3.0f);
			int tickTotal = MaxLimit / Interval;
			int count = 0;
			float tickOffset = 0;
			while (count < tickTotal + 1) {
				tickOffset = ((1f/MaxLimit * transform.localScale.x) * (count * Interval ));
				Vector3 tickPos = new Vector3(SliderBarHandleMesh.transform.position.x + tickOffset, transform.localPosition.y, transform.localPosition.z);
				GameObject tick = Instantiate(TickGRP, tickPos, transform.localRotation) as GameObject;
				tick.transform.parent = this.transform;
				count++;
			}
			return null;
		}

		// Update is called once per frame
		void Update ()
		{

		}
	}
}
