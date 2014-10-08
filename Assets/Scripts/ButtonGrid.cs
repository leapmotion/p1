using UnityEngine;
using System.Collections;

public class ButtonGrid : MonoBehaviour {
  private float size = 2.0f;
  private float spacing = 0.2f;
  private float depth = 10.0f;
  private Color button_color_ = Color.black;

	// Use this for initialization
	void Start () {
    // TODO: Load size, spacing and depth from JSON

    Vector3 center = new Vector3(0, 0, -depth);

    GameObject back_panel = CreateBackPanel();
    back_panel.transform.localScale = new Vector3(3 * size + 4 * spacing, 4 * size + 5 * spacing, size / 2);
    back_panel.transform.position = center;

    GameObject num_pad;
    for (int i = 0; i <3 ; ++i)
    {
      for(int j = 0; j < 3; ++j)
      {
        num_pad = CreateNumPad();
        num_pad.transform.localScale = Vector3.one * size;
        num_pad.transform.position = new Vector3((center.x - (spacing + size)) + i * (spacing + size), (center.y + 1.5f * (spacing + size)) - j * (spacing + size), center.z);
      }
    }
    num_pad = CreateNumPad();
    num_pad.transform.localScale = Vector3.one * size;
    num_pad.transform.position = new Vector3(center.x, center.y - 1.5f * (spacing + size), center.z);
	}

  private GameObject CreateNumPad()
  {
    return GameObject.CreatePrimitive(PrimitiveType.Cube);
  }

  private GameObject CreateBackPanel()
  {
    return GameObject.CreatePrimitive(PrimitiveType.Cube);
  }
}
