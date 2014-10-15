using UnityEngine;
using System.Collections;

namespace P1
{
  public class Radical : MonoBehaviour
  {
    public GameObject TwitterList;

    private TwitterStatusButton getTwitterStatusButton(Collider other)
    {
      if (other.transform.parent && other.transform.parent.GetComponent<TwitterStatusButton>())
      {
        return other.transform.parent.GetComponent<TwitterStatusButton>();
      }
      return null;
    }

    // Use this for initialization
    void Start()
    {
      this.transform.position = new Vector3(0.0f, 0.0f, TwitterList.transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter(Collider other)
    {
      TwitterStatusButton button = getTwitterStatusButton(other);
      if (button)
      {
        button.SetColor(Color.cyan);
      }
    }

    void OnTriggerExit(Collider other)
    {
      TwitterStatusButton button = getTwitterStatusButton(other);
      if (button)
      {
        button.ResetColor();
      }
      
    }
  }
}
