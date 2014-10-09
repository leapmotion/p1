using UnityEngine;
using System.Collections;

namespace P1
{
  public class ScrollDetector : MonoBehaviour
  {
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
      Debug.Log("Start Scroll");
    }

    void OnTriggerExit(Collider other)
    {
      Debug.Log("Stop Scroll");
    }
  }
}
