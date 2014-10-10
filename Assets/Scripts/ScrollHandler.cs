using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Leap;

namespace P1
{
  public class ScrollHandler : MonoBehaviour
  {
    public GameObject content;
    protected Controller leap_controller_;
    private int activeHandParts = 0;
    private bool allowScroll_ = false;
    private HandModel activeHand_ = null;
    private Vector3 start_palm_position_ = Vector3.zero;
    private Vector3 start_content_position = Vector3.zero;

    public void SetScroll(bool allowScroll)
    {
      allowScroll_ = allowScroll;
      if (activeHand_ != null)
      {
        start_palm_position_ = activeHand_.GetPalmPosition();
      }
      if (content != null)
      {
        start_content_position = content.transform.position;
      }
    }

    private HandModel getHand(Collider other)
    {
      if (other.transform.parent && other.transform.parent.parent && other.transform.parent.parent.GetComponent<HandModel>())
      {
        return other.transform.parent.parent.GetComponent<HandModel>();
      }
      else
      {
        return null;
      }
    }

    void OnTriggerEnter(Collider other)
    {
      if (activeHand_ == null)
      {
        activeHand_ = getHand(other);
        activeHandParts = 0;
      }

      if (activeHand_ == getHand(other))
      {
        activeHandParts++;
      }
    }

    void OnTriggerExit(Collider other)
    {
      if (activeHand_ == getHand(other))
      {
        activeHandParts--;
        if (activeHandParts == 0)
        {
          activeHand_ = null;
        }
      }
    }

    #region loop
    void Awake()
    {
      leap_controller_ = new Controller();
    }

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
      if (activeHand_ != null)
      {
        HandList hands = leap_controller_.Frame().Hands;
        bool handExists = false;
        for (int i = 0; i < hands.Count; ++i)
        {
          if (hands[i].Id == activeHand_.GetLeapHand().Id)
          {
            handExists = true;
            break;
          }
        }
        if (!handExists)
        {
          activeHand_ = null;
          activeHandParts = 0;
        }

        if (allowScroll_ && activeHandParts > 0 && content)
        {
          Vector3 position_change = activeHand_.GetPalmPosition() - start_palm_position_;
          content.transform.position = start_content_position + position_change;
          Vector3 local_position = content.transform.localPosition;
          local_position.z = 0.0f;
          content.transform.localPosition = local_position;
        }
      }
    }
    #endregion
  }
}
