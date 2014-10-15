using UnityEngine;
using System.Collections;
using SimpleJSON;

namespace P1
{
  public class ButtonTrigger : MonoBehaviour
  {
    private Vector3 original_position;
    private Vector3 correct_basis;

    [HideInInspector]
    public bool is_active_ = false;
    [HideInInspector]
    public bool restricted_ = false;

    private bool readyToPress = true;
    private bool allow_colors_ = true;


    public void FingerEntered(bool is_active)
    {
      if (!is_active)
      {
        Reset();
      }
    }

    public void Reset()
    {
      this.transform.localPosition = Vector3.zero;
      readyToPress = true;
      is_active_ = false;

      if (allow_colors_)
        transform.parent.parent.GetComponent<TenKeyKey>().ResetColor();
    }

    #region Unity_Callbacks
    void OnTriggerEnter(Collider other)
    {
      if (restricted_)
        return;

      if (other.gameObject.layer != LayerMask.NameToLayer("Mouse"))
      {
        if (other.gameObject.name == "Trigger")
        {
          is_active_ = true;
          if (readyToPress)
          {
            //transform.parent.parent.GetComponent<TenKeyKey>().OnTenKeyEvent(true, "Leap");
            transform.parent.parent.GetComponent<TrialTrigger>().SliderTrialBoundary.GetComponent<SliderTrialTrigger>().Trigger();
            if (allow_colors_)
              transform.parent.parent.GetComponent<TenKeyKey>().UpdateColor(Color.cyan);
            readyToPress = false;
          }
        }
        else if (other.gameObject.name == "Cushion")
        {
          if (allow_colors_)
            transform.parent.parent.GetComponent<TenKeyKey>().UpdateColor(Color.gray);
        }
      }
    }

    void OnTriggerExit(Collider other)
    {
      if (restricted_)
        return;

      if (other.gameObject.layer != LayerMask.NameToLayer("Mouse"))
      {
        if (other.gameObject.name == "Cushion")
        {
          readyToPress = true;
          
          if (allow_colors_)
            transform.parent.parent.GetComponent<TenKeyKey>().ResetColor();
        }
        else if (other.gameObject.name == "Trigger")
        {
          is_active_ = false;
          if (allow_colors_)
            transform.parent.parent.GetComponent<TenKeyKey>().UpdateColor(Color.gray);
        }
      }
    }

    // Use this for initialization
    void Start()
    {
      Vector3 trigger_position = transform.parent.FindChild("Trigger").transform.position;
      original_position = transform.position;
      correct_basis = trigger_position - original_position;
      JSONNode data = Utils.FileToJSON("grid_config.json");
      allow_colors_ = data["grid"]["allowColor"].AsBool;
    }

    // Update is called once per frame
    void Update()
    {
      if (restricted_)
      {
        Reset();
        return;
      }

      Vector3 curr_basis = transform.position - original_position;
      Vector3 adjusted_basis = Vector3.Project(curr_basis, correct_basis);
      transform.position = adjusted_basis + original_position;
    }
    #endregion
  }
}
