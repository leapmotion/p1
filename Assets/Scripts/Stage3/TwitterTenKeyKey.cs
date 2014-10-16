using UnityEngine;
using System.Collections;

namespace P1
{
  public class TwitterTenKeyKey : TenKeyKey
  {
    public TwitterList twitterList = null;
    public override void TriggerAction()
    {
      if (twitterList != null)
      {
        twitterList.Trigger();
      }
    }
  }
}
