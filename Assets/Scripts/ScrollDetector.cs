using UnityEngine;
using System.Collections;

namespace P1
{
  public class ScrollDetector : MonoBehaviour
  {
    public ScrollHandler scrollHandler;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter(Collider other)
    {
      scrollHandler.SetScroll(true);
    }

    void OnTriggerExit(Collider other)
    {
      scrollHandler.SetScroll(false);
    }
  }
}
