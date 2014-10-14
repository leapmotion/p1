using UnityEngine;
using System.Collections;
using SimpleJSON;

namespace P1
{
  public class ButtonTrigger : MonoBehaviour
  {
    private Vector3 original_position;
    private Vector3 correct_basis;

    private bool isIndex_ = false;
    private bool readyToPress = true;
    private bool allow_colors_ = true;

    public void FingerEntered(bool isIndex)
    {
      isIndex_ = isIndex;
    }

    #region Unity_Callbacks
    void OnTriggerEnter(Collider other)
    {
      if (other.gameObject.layer != LayerMask.NameToLayer("Mouse"))
      {
        if (other.gameObject.name == "Trigger")
        {
          if (readyToPress)
          {
            isIndex_ = true; // Allow all fingers to activate the buttons
            if (isIndex_)
            {
              transform.parent.parent.GetComponent<TenKeyKey>().OnTenKeyEvent(true, "Leap");
              if (allow_colors_)
                transform.parent.parent.GetComponent<TenKeyKey>().UpdateColor(Color.cyan);
            }
            else
            {
              transform.parent.parent.GetComponent<TenKeyKey>().OnTenKeyEvent(false, "Leap");
            }
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
      allow_colors_ = Utils.FileToJSON("grid_config.json")["grid"]["allowColor"].AsBool;
    }

    // Update is called once per frame
    void Update()
    {
      Vector3 curr_basis = transform.position - original_position;
      Vector3 adjusted_basis = Vector3.Project(curr_basis, correct_basis);
      transform.position = adjusted_basis + original_position;
    }
    #endregion
  }
}
