using UnityEngine;
using System.Collections;

namespace P1
{
  public class TwitterTenKeyKey : TenKeyKey
  {
    public TwitterList twitterList = null;

    public override void Start()
    {
 	    base.Start();
      Vector3 direction = transform.position - Camera.main.transform.position;
      float distance = 0.7f;
      transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);
    }

    public override void TriggerAction()
    {
      if (twitterList != null)
      {
        twitterList.Trigger();
      }
    }
  }
}
