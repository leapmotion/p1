using UnityEngine;
using System.Collections;

namespace P1
{
  public class ButtonGrid : MonoBehaviour
  {
    public GameObject buttonTemplate;
    public Vector3 buttonScale;

    private float size = 2.0f;
    private float spacing = 0.2f;
    private float depth = -5.0f;

    // Use this for initialization
    void Start()
    {
      // TODO: Load size, spacing and depth from JSON

      Vector3 center = new Vector3(0, 0, -depth);

      GameObject num_pad;
      for (int i = 0; i < 3; ++i)
      {
        for (int j = 0; j < 3; ++j)
        {
          num_pad = CreateNumPad(new Vector3((center.x - (spacing + size)) + i * (spacing + size), (center.y + 1.5f * (spacing + size)) - j * (spacing + size), center.z));
          num_pad.transform.localScale = Vector3.one * size;
        }
      }
      num_pad = CreateNumPad(new Vector3(center.x, center.y - 1.5f * (spacing + size), center.z));
      num_pad.transform.localScale = Vector3.one * size;
    }

    private GameObject CreateNumPad(Vector3 position)
    {
      GameObject go = ((GameObject)Instantiate(buttonTemplate, position, Quaternion.identity));
      TenKeyKey g = (TenKeyKey)(go.gameObject.GetComponent<TenKeyKey>());
      g.KeypadScale = buttonScale;
      //g.label = k.label;
      g.label = "1";
      go.transform.parent = transform;
      go.gameObject.transform.FindChild("button").FindChild("default").GetComponent<SpringJoint>().connectedAnchor = position;
      go.transform.position = position;
      return go;
      //return GameObject.CreatePrimitive(PrimitiveType.Cube);
    }
  }
}
