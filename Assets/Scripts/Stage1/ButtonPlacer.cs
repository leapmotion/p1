using UnityEngine;
using System.Collections;
using System.IO;
using System.Text;

using SimpleJSON;

using ButtonMonkey;

namespace P1
{
  public struct KeyDef
  {
    public string label;
    public int i;
    public int j;

    public KeyDef(string l, int ii, int jj)
    {
      label = l;
      i = ii;
      j = jj;
    }
  }

  public class ButtonPlacer : MonoBehaviour
  {

    private Vector3 buttonScale;
    private Vector3 buttonSpacing;
    public KeyDef[] keys = new KeyDef[]{
					new KeyDef ("1", -1, 1),
					new KeyDef ("2", 0, 1),
					new KeyDef ("3", 1, 1),
					new KeyDef ("4", -1, 0),
					new KeyDef ("5", 0, 0),
					new KeyDef ("6", 1, 0),
					new KeyDef ("7", -1, -1),
					new KeyDef ("8", 0, -1),
					new KeyDef ("9", 1, -1),
					new KeyDef ("0", 0, -2)
				};
    public GameObject buttonTemplate;
    //public GFRectGrid grid;
    GridMonkey monkeyDo;
    public GameObject pinPrompt;
    public GameObject backPad;

    #region loop

    // Use this for initialization
    void Start()
    {
      DoStart();
    }

    public void DoStart()
    {
      monkeyDo = new GridMonkey();
      //if (grid == null) {	
      //    grid = GetComponent<GFRectGrid> ();
      //}
      SetGridFromConfig("Assets/config/grid_config.json");

	  monkeyDo.ConfigureTest(Application.dataPath, "grid");
      monkeyDo.TrialEvent += TrialUpdate;

      monkeyDo.Start();
      Debug.Log("Monkey, type: " + monkeyDo.GetTrialKeys());
      pinPrompt.GetComponent<PINPrompt>().UpdatePIN(monkeyDo.GetTrialKeys());
    }

    // Called once for each key pushed
    void TrialUpdate(MonkeyTester trial)
    {
      if (monkeyDo.StageComplete())
      {
        // Show final correct result
        pinPrompt.GetComponent<PINPrompt>().TogglePIN(true);
        Debug.Log("Autopsy report for monkey:\n" + monkeyDo.ToString());
        if (SceneManager.instance)
        {
          SceneManager.instance.Next();
        }
      }
      else
      {
        if (monkeyDo.TrialComplete())
        {
          // Show final correct result
          pinPrompt.GetComponent<PINPrompt>().TogglePIN(true);

          monkeyDo.Start();
          Debug.Log("Monkey, type: " + monkeyDo.GetTrialKeys());
          pinPrompt.GetComponent<PINPrompt>().UpdatePIN(monkeyDo.GetTrialKeys());
        }
        else
        {
          if (monkeyDo.WasCorrect())
          {
            Debug.Log("Good monkey! Next, type: " + monkeyDo.GetTrialKeys()[monkeyDo.GetTrialStep()]);
            pinPrompt.GetComponent<PINPrompt>().TogglePIN(true);
          }
          else
          {
            Debug.Log("Bad monkey! You were told to type: " + monkeyDo.GetTrialKeys()[monkeyDo.GetTrialStep()]);
            pinPrompt.GetComponent<PINPrompt>().TogglePIN(false);
          }
        }
      }
    }

    // Update is called once per frame
    void Update()
    {
    }
    #endregion

    #region configuration

    public void SetGridFromConfig(string filePath)
    {
      JSONNode data = Utils.FileToJSON(filePath);

      float x, y, z;

      x = data["spacing"]["x"].AsFloat;
      y = data["spacing"]["y"].AsFloat;
      z = data["spacing"]["z"].AsFloat;

      //grid.spacing = new Vector3 (x, y, z);
      buttonSpacing = new Vector3(x, y, z);

      x = data["buttonScale"]["x"].AsFloat;
      y = data["buttonScale"]["y"].AsFloat;
      z = data["buttonScale"]["z"].AsFloat;

      buttonScale = new Vector3(x, y, z);

      x = data["position"]["x"].AsFloat;
      y = data["position"]["y"].AsFloat;
      z = data["position"]["z"].AsFloat;

      transform.position = new Vector3(x, y, z);
      transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);

      float sensitivity = data["button"]["sensitivity"].AsFloat;


      // PromptLandscape
      // True - Left->Right
      // False - Top->Down
      bool isLandscape = data["prompt"]["isLandscape"].AsBool;
      float prompt_h = (isLandscape) ? 1.0f : 4.0f;
      float prompt_w = (isLandscape) ? 4.0f : 1.0f;
      float prompt_padding = data["prompt"]["padding"].AsFloat;

      // PromptPos
      // 0 - Top
      // 1 - Bottom
      // 2 - Right
      // 3 - Left
      int promptPos = data["prompt"]["position"].AsInt;

      int row = data["grid"]["row"].AsInt;
      int col = data["grid"]["col"].AsInt;
      int num_keys = row * col;
      if (num_keys > keys.Length)
      {
        num_keys = keys.Length;
        row = 4;
        col = 3;
      }
      float half_h = (float)(row - 1) / 2.0f;
      float half_w = (float)(col - 1) / 2.0f;

      float prompt_rel_h = 0.0f;
      float prompt_rel_w = 0.0f;
      switch (promptPos)
      {
        case 0: // Top
          prompt_rel_h += prompt_h + prompt_padding;
          break;
        case 1: // Bottom
          prompt_rel_h -= prompt_h + prompt_padding;
          break;
        case 2: // Right
          prompt_rel_w += prompt_w + prompt_padding;
          break;
        case 3: // Left
          prompt_rel_w -= prompt_w + prompt_padding;
          break;
        default: // Default: Top
          prompt_rel_h += prompt_h + prompt_padding;
          break;
      }

      Vector3 center = new Vector3(
        - prompt_rel_w / 2.0f,
        - prompt_rel_h / 2.0f,
        0
        );

      transform.FindChild("Cube").transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);

      //backPad.transform.localPosition = new Vector3(0.0f, 0.0f, buttonScale.z / 3.0f);
      //backPad.transform.localScale = new Vector3((prompt_w + half_w * 2) / 4.0f, (prompt_h + half_h * 2) / 4.0f, buttonScale.z);

      float x_index = - half_w;
      float y_index = half_h;
      for (int i = 0; i < num_keys; ++i)
      {
        KeyDef k = keys[i];
       
        // Construct the matrix of keys based on the rows and cols. The last key will be at the last rol and centered
        Vector3 localPos = center;
        float x_coord = 0.0f;
        float y_coord = 0.0f;
        float z_coord = 0.0f;
        if (i == keys.Length - 1 && half_w == 1.0f && half_h == 1.5f)
        {
          y_coord = y_index * (buttonSpacing.y + buttonScale.y);
        }
        else
        {
          x_coord = x_index * (buttonSpacing.x + buttonScale.x);
          y_coord = y_index * (buttonSpacing.y + buttonScale.y);

          x_index++;
          if (x_index > (half_w + 0.25f))
          {
            x_index = -half_w;
            y_index--;
          }
        }
        localPos += new Vector3(x_coord, y_coord, z_coord);

        GameObject go = ((GameObject)Instantiate(buttonTemplate, transform.TransformPoint(localPos), Quaternion.identity));
        go.SetActive(true);
        TenKeyKey g = (TenKeyKey)(go.gameObject.GetComponent<TenKeyKey>());
        g.SetTriggerSensitivity(sensitivity);
        g.KeypadScale = buttonScale;
        g.label = k.label;
        go.transform.parent = transform;
        go.gameObject.transform.FindChild("button").FindChild("default").GetComponent<SpringJoint>().connectedAnchor = transform.TransformPoint(localPos);
        g.TenKeyEventBroadcaster += new TenKeyKey.TenKeyEventDelegate(monkeyDo.WhenPushed);
        go.transform.localPosition = localPos;
        go.transform.localScale = buttonScale;
        go.transform.rotation = transform.rotation;
      }

      Vector3 promptPosition = center;
      switch (promptPos)
      {
        case 0:
          promptPosition.y += prompt_padding + (half_h + prompt_h / 2 + 0.5f) * buttonScale.y + half_h * buttonSpacing.y;
          break;
        case 1:
          promptPosition.y -= prompt_padding + (half_h + prompt_h / 2 + 0.5f) * buttonScale.y + half_h * buttonSpacing.y;
          break;
        case 2:
          promptPosition.x += prompt_padding + (half_w + prompt_w / 2 + 0.5f) * buttonScale.x + half_w * buttonSpacing.x;
          break;
        case 3:
          promptPosition.x -= prompt_padding + (half_w + prompt_w / 2 + 0.5f) * buttonScale.x + half_w * buttonSpacing.x;
          break;
        default:
          promptPosition.y += prompt_padding + (half_h + prompt_h / 2 + 0.5f) * buttonScale.y + half_h * buttonSpacing.y;
          break;
      }
      pinPrompt.transform.localPosition = promptPosition;
      pinPrompt.transform.localScale = buttonScale;
      pinPrompt.transform.rotation = transform.rotation;
      pinPrompt.GetComponent<PINPrompt>().SetOrientation(isLandscape);
    }

    #endregion
  }
}