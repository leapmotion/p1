using UnityEngine;
using System.Collections;

namespace P1
{
  public class SliderTenKeyKey : TenKeyKey
  {
    public override void TriggerAction()
    {
      GetComponent<TrialTrigger>().Trigger();
    }
  }
}

