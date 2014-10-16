using UnityEngine;
using System;
using System.Collections;

namespace P1
{
  public class TenKeyKey : MonoBehaviour
  {

    public GameObject[] labels = new GameObject[10];
    public GameObject button;
    public GameObject trigger;
    public GameObject cushion;

    private Color original_color = new Color(0.25f, 0.25f, 0.25f);
    private bool mouse_clicked = false;

    public float springConstant = 1000.0f;

    [SerializeField]
    private float m_KeypadUniformScale;
    public float KeypadUniformScale
    {
      get { return m_KeypadUniformScale; }
      set
      {
        this.transform.localScale = new Vector3(value, value, value);
        m_KeypadUniformScale = value;
      }
    }

    [SerializeField]
    private Vector3
        m_KeypadScale;

    public Vector3 KeypadScale
    {
      get { return m_KeypadScale; }
      set
      {
        this.transform.localScale = value;
        m_KeypadScale = value;
      }
    }

    private string label_ = "";
    public string label
    {
      get
      {
        return label_;
      }

      set
      {
        label_ = value;
        foreach (GameObject g in labels)
        {
          Transform[] child_transforms = g.GetComponentsInChildren<Transform>();
          foreach (Transform child_transform in child_transforms)
          {
            child_transform.gameObject.SetActive(false);
          }
          g.SetActive(false);
        }

        try
        {
          labels[System.Convert.ToInt16(value)].SetActive(true);
          Transform[] child_transforms = labels[System.Convert.ToInt16(value)].GetComponentsInChildren<Transform>(true);
          foreach (Transform child_transform in child_transforms)
          {
            child_transform.gameObject.SetActive(true);
          }
        }
        catch (UnityException e)
        {
          Debug.Log(e + ": Cannot set letter for value " + value);
        }
      }
    }

    public void Init()
    {
      button.GetComponent<SpringJoint>().connectedAnchor = transform.position;
    }

    public void SetActive(bool value)
    {
      if (value)
      {
        Collider[] colliders = GetComponentsInChildren<Collider>();
        foreach (Collider collider in colliders)
        {
          collider.enabled = true;
        }
        SpringJoint[] joints = GetComponentsInChildren<SpringJoint>();
        foreach (SpringJoint joint in joints)
        {
          joint.spring = springConstant;
        }
      }
      else
      {
        Collider[] colliders = GetComponentsInChildren<Collider>();
        foreach (Collider collider in colliders)
        {
          collider.enabled = false;
        }
        SpringJoint[] joints = GetComponentsInChildren<SpringJoint>();
        foreach (SpringJoint joint in joints)
        {
          joint.spring = 0;
        }
      }
    }

    public virtual void TriggerAction()
    {
      OnTenKeyEvent(true, "Leap");
    }

    public void SetTriggerSensitivity(float sensitivity)
    {
      BoxCollider box_collider = GetComponentInChildren<Joint>().GetComponent<BoxCollider>();
      float edge = 3 * box_collider.size.z / 2 - box_collider.center.z - 1.0f;
      Vector3 trigger_position = new Vector3(0.0f, 0.0f, edge + sensitivity);
      trigger.transform.localPosition = trigger_position;

      Vector3 cushion_position = new Vector3(0.0f, 0.0f, (trigger_position.z + edge) / 2);
      Vector3 cushion_size = new Vector3(1.0f, 1.0f, Mathf.Max(0.0f, (trigger_position.z - edge) - 1.0f) * 0.8f);
      cushion.transform.localPosition = cushion_position;
      cushion.transform.localScale = cushion_size;
    }

    public void UpdateColor(Color color)
    {
      float alpha = button.renderer.material.color.a;
      color.a = alpha;
      button.renderer.material.color = color;
    }

    public void SetLabelColor(Color color)
    {
      foreach (GameObject num_label in labels)
      {
        if (num_label.renderer)
        {
          num_label.renderer.material.color = color;
        }
      }
    }

    public void SetDefaultColor(Color color)
    {
      original_color = color;
    }

    public void ResetColor()
    {
      button.renderer.material.color = original_color;
    }
    #region loop

    State state;

    // Use this for initialization
    void Start()
    {
      if (!StateList.HasList("ButtonState"))
      {
        new StateList("ButtonState", "unknown", "default", "over", "down");
      }
      state = new State("ButtonState");
    }

    // Update is called once per frame
    void Update()
    {
      if (!mouse_clicked &&
          Input.GetMouseButtonDown(0))
      {
        mouse_clicked = true;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100))
        {
          if (hit.transform.parent.parent == this.transform &&
              hit.transform.gameObject.layer == LayerMask.NameToLayer("Mouse"))
          {
            OnTenKeyEvent(true, "click"); //Mouse click cannot be incomplete
          }
        }
      }

      //NOTE: ButtonUp is an event, NOT a state,
      //so mouse_clicked must be initialized to false
      //in order to register the first click
      if (Input.GetMouseButtonUp(0))
      {
        mouse_clicked = false;
      }
    }

    #endregion

    #region broadcast

    public delegate void TenKeyEventDelegate(bool complete, int symbol);

    public event TenKeyEventDelegate TenKeyEventBroadcaster;

    public void OnTenKeyEvent(bool complete, string e)
    {
      if (TenKeyEventBroadcaster != null)
      {
        TenKeyEventBroadcaster(complete, Convert.ToInt32(label,10));
      }
    }

    #endregion

    #region mouse2D

    void OnMouseEnter()
    {
      //Debug.Log ("OnMouseEnter");
      state.Change("over");
    }

    void OnMouseDown()
    {
      //Debug.Log ("OnMouseDown");
      state.Change("down");
    }

    void OnMouseUp()
    {
      //Debug.Log ("OnMouseUp");
      OnTenKeyEvent(true, "click");
      state.Change("over");
    }

    void OnMouseExit()
    {
      //Debug.Log ("OnMouseExit");
      state.Change("default");
    }

    #endregion

  }
}