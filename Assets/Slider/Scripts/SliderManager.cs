using UnityEngine;
using System.Collections;

public class SliderManager : MonoBehaviour {

	static SliderManager m_Instance = null;
	public static SliderManager Instance
	{
		get
		{
			if( null == m_Instance )
			{
				SliderManager temp = (SliderManager)GameObject.FindObjectOfType( typeof(SliderManager) );
				if( temp == null )
				{
					Debug.LogWarning( "You are missing a Facetopo Manager in the scene." );
				}
				m_Instance = temp;
			}
			return m_Instance;
		}
	} 
	[SerializeField]
	private int m_MinLimit;
	public int MinLimit
	{
		get { return m_MinLimit; }
		set {
			TextLowerLimit.text = m_MinLimit.ToString();
			m_MinLimit = value;
		}
	}
	[SerializeField]
	private int m_MaxLimit;
	public int MaxLimit
	{
		get { return m_MaxLimit; }
		set {
			TextUpperLimit.text = m_MaxLimit.ToString();
			m_MaxLimit = value;
		}
	}

	public int Interval;

	public TextMesh TextLowerLimit;
	public TextMesh TextUpperLimit;
	public TextMesh TextSliderValue;


	[SerializeField]
	private float m_WidgetWidth;
	public float WidgetWidth
	{
		get { return m_WidgetWidth; }
		set {
			this.transform.localScale = new Vector3(value, this.transform.localScale.y, this.transform.localScale.z);
			m_WidgetWidth = value;
		}
	}
	[SerializeField]
	private float m_WidgetHeight;
	public float WidgetHeight
	{
		get { return m_WidgetHeight; }
		set {
			this.transform.localScale = new Vector3(this.transform.localScale.x, value, this.transform.localScale.z);
			m_WidgetHeight = value;
		}
	}

	//public float Xpos;
	//public float Ypos;

	[SerializeField]
	private float m_SliderHandleHeight;
	public float SliderHandleHeight
	{
		get { return m_SliderHandleHeight; }
		set {
			this.transform.localScale = new Vector3(SliderBarHandleMesh.transform.localScale.x, value, SliderBarHandleMesh.transform.localScale.z);
			m_SliderHandleHeight = value;
		}
	}

	[SerializeField]
	private float m_SliderHandleWidth;
	public float SliderHandleWidth
	{
		get { return m_SliderHandleWidth; }
		set {
			this.transform.localScale = new Vector3(SliderBarHandleMesh.transform.localScale.x, SliderBarHandleMesh.transform.localScale.y, value);
			m_SliderHandleWidth = value;
		}
	}

	[SerializeField]
	private float m_SliderHandleDepth;
	public float SliderHandleDepth
	{
		get { return m_SliderHandleDepth; }
		set {
			this.transform.localScale = new Vector3(SliderBarHandleMesh.transform.localScale.x, SliderBarHandleMesh.transform.localScale.y);
			m_SliderHandleDepth = value;
		}
	}

	public GameObject SliderBarMesh;
	public GameObject SliderBarHandleMesh;

	public Material SliderHandle;
	public Material SliderHandleActive;

	public float SliderSpeed;

	public bool TicksOnOff;
	public GameObject TickGRP;



	// Use this for initialization
	void Start () {
		MinLimit = MinLimit;
		MaxLimit = MaxLimit;
		BuildTicks ();
	}

	public IEnumerator BuildTicks (){
		int tickTotal = MaxLimit / Interval;
		int count = 0;
		float tickOffset = 0;
		while (count < tickTotal + 1) {
			tickOffset = (1f/MaxLimit) * (count * Interval);
			Vector3 tickPos = new Vector3(tickOffset, transform.localPosition.y, transform.localPosition.z);
			GameObject tick = Instantiate(TickGRP, tickPos, transform.localRotation) as GameObject;
			tick.transform.parent = this.transform;
			count++;
		}

		return null;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
