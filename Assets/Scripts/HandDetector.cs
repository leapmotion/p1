using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Leap;

namespace P1
{
  public class HandDetector : MonoBehaviour
  {
    protected Controller leap_controller_;
    private int activeHandId = -1;
    private int activeHandParts = 0;
    private Vector3 palm_position;

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
      Vector3 palm_position = Vector3.zero;
      Frame frame = leap_controller_.Frame();
      HandList hands = frame.Hands;
      bool handExists = false;
      for (int i = 0; i < hands.Count; ++i)
      {
        if (hands[i].Id == activeHandId)
        {
          handExists = true;
          break;
        }
      }
      if (!handExists)
      {
        activeHandParts = 0;
      }

      if (activeHandParts > 0)
      {

      }
    }

    void OnTriggerEnter(Collider other)
    {
      HandModel hand = getHand(other);
      if (hand != null)
      {
        if (activeHandParts == 0)
        {
          activeHandId = hand.GetLeapHand().Id;
        }
        if (hand.GetLeapHand().Id == activeHandId)
        {
          activeHandParts++;
          Debug.Log("Add: " + activeHandParts);
        }
      }
    }

    void OnTriggerExit(Collider other)
    {
      HandModel hand = getHand(other);
      if (hand != null)
      {
        if (hand.GetLeapHand().Id == activeHandId)
        {
          activeHandParts--;
          Debug.Log("Del: " + activeHandParts);
        }
      }
    }

    private HandModel getHand(Collider other)
    {
      if (other.transform.parent && other.transform.parent.parent && other.transform.parent.parent.GetComponent<HandModel>()) {
        return other.transform.parent.parent.GetComponent<HandModel>();
      }
      else
      {
        return null;
      }
    }
  }
}
