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
					new KeyDef ("0", 0, -2),
					new KeyDef ("1", -1, 1),
					new KeyDef ("2", 0, 1),
					new KeyDef ("3", 1, 1),
					new KeyDef ("4", -1, 0),
					new KeyDef ("5", 0, 0),
					new KeyDef ("6", 1, 0),
					new KeyDef ("7", -1, -1),
					new KeyDef ("8", 0, -1),
					new KeyDef ("9", 1, -1)
				};
    public GameObject buttonTemplate;
    //public GFRectGrid grid;
    int test;
    string testPath = ""; //DEFAULT: Record in TestResults
    int testNum = 1; //DEFAULT: Run one trial
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

      int Imin = 0;
      int Imax = 0;
      int Jmin = 0;
      int Jmax = 0;

      int row = 4; // TODO(wyu): Replace with config
      int col = 3; // TODO(wyu): Replace with config
      int num_keys = Mathf.Min(10, row * col);

      foreach (KeyDef k in keys)
      {
        //Vector3 pos = grid.GridToWorld (new Vector3 (k.i, k.j, 0));
        Vector3 localPos = new Vector3(
          k.i * buttonSpacing.x + k.i * buttonScale.x,
          k.j * buttonSpacing.y + k.j * buttonScale.y,
          0
          );

        Imin = (int)Mathf.Min(k.i, Imin);
        Imax = (int)Mathf.Max(k.i, Imax);
        Jmin = (int)Mathf.Min(k.j, Jmin);
        Jmax = (int)Mathf.Max(k.j, Jmax);

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

      // PromptPos
      // 0 - Top
      // 1 - Bottom
      // 2 - Right
      // 3 - Left
      int promptPos = data["button"]["promptPos"].AsInt;
      Vector3 promptPosition = Vector3.zero;

      // PromptLandscape
      // True - Left->Right
      // False - Top->Down
      bool isLandscape = data["button"]["promptLandscape"].AsBool;
      float y_size = 0.0f;
      float x_size = 0.0f;
      if (isLandscape)
      {
        x_size = 2.5f;
        y_size = 1.0f;
      }
      else
      {
        x_size = 1.0f;
        y_size = 2.5f;
      }

      switch (promptPos)
      {
        case 0:
          promptPosition.y = 0.1f + (Mathf.Abs(Jmax) + y_size) * buttonScale.y + Mathf.Abs(Jmax) * buttonSpacing.y;
          break;
        case 1:
          promptPosition.y = -(0.1f + (Mathf.Abs(Jmin) + y_size) * buttonScale.y + Mathf.Abs(Jmin) * buttonSpacing.y);
          break;
        case 2:
          promptPosition.x = 0.1f + (Mathf.Abs(Imax) + x_size) * buttonScale.x + Mathf.Abs(Imax) * buttonSpacing.x;
          break;
        case 3:
          promptPosition.x = -(0.1f + (Mathf.Abs(Imin) + x_size) * buttonScale.x + Mathf.Abs(Imin) * buttonSpacing.x);
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
      testNum = data["trial_count"].AsInt;
    }

    #endregion
  }
}