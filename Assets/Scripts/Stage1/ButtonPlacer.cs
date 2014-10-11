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
    int test;
    string testPath = ""; //DEFAULT: Record in TestResults
    ButtonTrial monkeyDo;
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
      monkeyDo = new ButtonTrial();
      //if (grid == null) {	
      //    grid = GetComponent<GFRectGrid> ();
      //}
      SetGridFromConfig("Assets/config/grid_config.json");

      monkeyDo.SetTestFromConfig(Application.dataPath);
      monkeyDo.TrialEvent += TrialUpdate;

      monkeyDo.Start();
      Debug.Log("Monkey, type: " + monkeyDo.GetTrialKeys());
      pinPrompt.GetComponent<PINPrompt>().UpdatePIN(monkeyDo.GetTrialKeys());
    }

    // Called once for each key pushed
    void TrialUpdate(ButtonTrial trial, bool correct)
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

      int row = 2; // TODO(wyu): Replace with config
      int col = 5; // TODO(wyu): Replace with config
      int num_keys = row * col;
      float half_w = (float)(col - 1) / 2.0f;
      float half_h = (float)(row - 1) / 2.0f;
      Debug.Log(num_keys);
      Debug.Log(keys.Length);
      if (num_keys > keys.Length)
      {
        num_keys = keys.Length;
        half_w = 1.0f; // (float)(3 - 1) / 2.0f;
        half_h = 1.5f; // (float)(4 - 1) / 2.0f;
      }

      Vector3 center = Vector3.zero;

      float x_index = - half_w;
      float y_index = half_h;
      for (int i = 0; i < num_keys; ++i)
      {
        KeyDef k = keys[i];
        
        //Vector3 pos = grid.GridToWorld (new Vector3 (k.i, k.j, 0));
        //Vector3 localPos = new Vector3(
        //  k.i * buttonSpacing.x + k.i * buttonScale.x,
        //  k.j * buttonSpacing.y + k.j * buttonScale.y,
        //  0
        //  );
       
        // Construct the matrix of keys based on the rows and cols. The last key will be at the last rol and centered
        Vector3 localPos;
        if (i == keys.Length - 1 && half_w == 1.0f && half_h == 1.5f)
        {
          localPos = new Vector3(
            0,
            y_index * (buttonSpacing.y + buttonScale.y),
            0
          );
        }
        else
        {
          localPos = new Vector3(
          x_index * (buttonSpacing.x + buttonScale.x),
          y_index * (buttonSpacing.y + buttonScale.y),
          0
          );

          x_index++;
          if (x_index > (half_w + 0.25f))
          {
            x_index = -half_w;
            y_index--;
          }
        }

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

      // PromptLandscape
      // True - Left->Right
      // False - Top->Down
      bool isLandscape = data["button"]["promptLandscape"].AsBool;
      float prompt_h = (isLandscape) ? 1.0f : 4.0f;
      float prompt_w = (isLandscape) ? 4.0f : 1.0f;
      float prompt_padding = 0.1f;

      // PromptPos
      // 0 - Top
      // 1 - Bottom
      // 2 - Right
      // 3 - Left
      int promptPos = data["button"]["promptPos"].AsInt;
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

      transform.FindChild("Cube").transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);

      backPad.transform.localPosition = new Vector3(0.0f, 0.0f, buttonScale.z);
      backPad.transform.localScale = new Vector3(Mathf.Max(buttonScale.x * 0.75f, (3.0f * buttonScale.x + 3.5f * buttonSpacing.x) / 5.0f), (5.0f * buttonScale.y + 3.5f * buttonSpacing.y + 0.1f) / 5.5f, buttonScale.z);
    }

    public void SetTestFromConfig(string filePath)
    {
      JSONNode data = Utils.FileToJSON(filePath);
      testPath = data["results_dir"].ToString();
      // NOTE: JSONNode ToString helpfully interprets both path/ (no quotes in file) and "path/" (quotes in file)
      // as "path/" (quotes IN string).
      testPath = testPath.Substring(1, testPath.Length - 2);
    }

    #endregion
  }
}