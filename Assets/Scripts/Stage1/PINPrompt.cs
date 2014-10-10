using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using SimpleJSON;

namespace P1
{
  public class PINPrompt : MonoBehaviour
  {

    public GameObject buttonTemplate;
    private List<GameObject> pins = new List<GameObject>();
    private int pins_index = 0;
    private string new_pin;
    private float creation_timer = float.MaxValue;
    private Color starting_color = new Color(1.0f, 0.5f, 0.15f);

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
      if ((Time.time - creation_timer > 0.5f || pins.Count == 0) && pins_index == -1)
      {
        foreach (Transform child in transform)
        {
          Destroy(child.gameObject);
        }
        pins.Clear();

        float starting_position = - (new_pin.Length - 1) * 0.5f;
        for (int i = 0; i < new_pin.Length; ++i)
        {
          pins.Add(CreatePIN(transform.TransformPoint(new Vector3(starting_position + i, 0.0f, 0.0f)), new_pin.Substring(i, 1)));
        }
          

        pins_index = 0;
      }
    }

    public void UpdatePIN(string pin)
    {
      new_pin = pin;
      creation_timer = Time.time;
      pins_index = -1;
    }

    public void TogglePIN(bool status)
    {
      if (pins_index >= 0 && pins_index < pins.Count && status)
      {
        pins[pins_index].GetComponent<TenKeyKey>().UpdateColor(Color.green);
        pins_index++;
      }
    }

    private GameObject CreatePIN(Vector3 position, string label)
    {
      GameObject go = ((GameObject)Instantiate(buttonTemplate, position, Quaternion.identity));
      go.SetActive(true);
      TenKeyKey g = (TenKeyKey)(go.gameObject.GetComponent<TenKeyKey>());
      g.label = label;
      go.transform.parent = transform;
      go.gameObject.transform.FindChild("button").FindChild("default").GetComponent<SpringJoint>().connectedAnchor = position;
      go.transform.position = position;
      go.transform.rotation = transform.rotation;
      Collider[] colliders = go.GetComponentsInChildren<Collider>();
      foreach (Collider collider in colliders)
      {
        collider.enabled = false;
      }
      SpringJoint[] joints = go.GetComponentsInChildren<SpringJoint>();
      foreach (SpringJoint joint in joints)
      {
        joint.spring = 0;
      }
      ButtonTrigger button_trigger = go.GetComponentInChildren<ButtonTrigger>();
      button_trigger.transform.localPosition = Vector3.zero;
      go.transform.localScale = Vector3.one;
      go.GetComponent<TenKeyKey>().SetDefaultColor(starting_color);
      go.GetComponent<TenKeyKey>().ResetColor();

      return go;
    }
  }
}
