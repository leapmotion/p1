using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace P1
{
  public class PINPrompt : MonoBehaviour
  {

    public GameObject buttonTemplate;
    private List<GameObject> pins = new List<GameObject>();
    private int pins_index = 0;

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void UpdatePIN(string pin)
    {
      foreach (Transform child in transform)
      {
        Destroy(child.gameObject);
      }
      pins.Clear();

      GameObject prompt;
      prompt = CreatePIN(transform.TransformPoint(new Vector3(-3.0f, 0.0f, 0.0f)), pin.Substring(0, 1));
      prompt.transform.localScale = Vector3.one;
      pins.Add(prompt);
      prompt = CreatePIN(transform.TransformPoint(new Vector3(-2.0f, 0.0f, 0.0f)), pin.Substring(1, 1));
      prompt.transform.localScale = Vector3.one;
      pins.Add(prompt);
      prompt = CreatePIN(transform.TransformPoint(new Vector3(-1.0f, 0.0f, 0.0f)), pin.Substring(2, 1));
      prompt.transform.localScale = Vector3.one;
      pins.Add(prompt);
      prompt = CreatePIN(transform.TransformPoint(new Vector3(-0.0f, 0.0f, 0.0f)), pin.Substring(3, 1));
      prompt.transform.localScale = Vector3.one;
      pins.Add(prompt);

      pins_index = 0;
    }

    public void TogglePIN(bool status)
    {
      if (true)
      {
        pins[pins_index].GetComponent<TenKeyKey>().UpdateColor(Color.green);
        pins_index++;  
      } 
      else
      {
        pins[pins_index].GetComponent<TenKeyKey>().UpdateColor(Color.red);
      }
    }

    private GameObject CreatePIN(Vector3 position, string label)
    {
      GameObject go = ((GameObject)Instantiate(buttonTemplate, position, Quaternion.identity));
      TenKeyKey g = (TenKeyKey)(go.gameObject.GetComponent<TenKeyKey>());
      g.label = label;
      go.transform.parent = transform;
      go.gameObject.transform.FindChild("button").FindChild("default").GetComponent<SpringJoint>().connectedAnchor = position;
      go.transform.position = position;
      go.transform.rotation = transform.rotation;
      Collider[] colliders = GetComponentsInChildren<Collider>();
      foreach (Collider collider in colliders)
      {
        collider.enabled = false;
      }
      return go;
    }
  }
}
