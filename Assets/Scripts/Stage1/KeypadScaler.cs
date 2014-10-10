using UnityEngine;
using System.Collections;

/**
 Integrated into TenKeyKey -- deprecated class
*/

public class KeypadScaler : MonoBehaviour {


	[SerializeField]
	private float m_KeypadUniformScale;
	public float KeypadUniformScale
	{
		get { return m_KeypadUniformScale; }
		set {
			this.transform.localScale = new Vector3( value, value, value);
			m_KeypadUniformScale = value;
		}
	}
	[SerializeField]
	private Vector3 m_KeypadScale;
	public Vector3 KeypadScale
	{
		get { return m_KeypadScale; }
		set {
			this.transform.localScale = value;
			m_KeypadScale = value;
		}
	}





	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
