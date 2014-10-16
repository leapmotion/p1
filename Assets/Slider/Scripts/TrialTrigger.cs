using UnityEngine;
using System.Collections;

namespace P1
{
  public class TrialTrigger : MonoBehaviour
  {
    public GameObject SliderTrialBoundary;

    public void Trigger()
    {
      SliderTrialBoundary.GetComponent<SliderTrialTrigger>().Trigger();
    }
  }
}

