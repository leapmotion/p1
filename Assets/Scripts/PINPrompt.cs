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
      JSONNode data = Utils.FileToJSON("Assets/config/grid_config.json");
      float x = data["buttonScale"]["x"].AsFloat;
      float y = data["buttonScale"]["y"].AsFloat;
      float z = data["buttonScale"]["z"].AsFloat;
      transform.localScale = new Vector3(x, y, z);
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

        pins.Add(CreatePIN(transform.TransformPoint(new Vector3(-1.5f, 0.0f, 0.0f)), new_pin.Substring(0, 1)));
        pins.Add(CreatePIN(transform.TransformPoint(new Vector3(-0.5f, 0.0f, 0.0f)), new_pin.Substring(1, 1)));
        pins.Add(CreatePIN(transform.TransformPoint(new Vector3(0.5f, 0.0f, 0.0f)), new_pin.Substring(2, 1)));
        pins.Add(CreatePIN(transform.TransformPoint(new Vector3(1.5f, 0.0f, 0.0f)), new_pin.Substring(3, 1)));

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
      go.transform.localScale = Vector3.one;
      go.GetComponent<TenKeyKey>().SetDefaultColor(starting_color);
      go.GetComponent<TenKeyKey>().ResetColor();

      return go;
    }
  }
}
