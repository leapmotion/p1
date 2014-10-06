using UnityEngine;
using System.Collections;

public class SceneManager : MonoBehaviour {

  Texture2D texture_;
  private float timer_;
  private float start_timer_;
  private float timer_threshold_ = 2.0f;
  private string stage_to_load_ = "Stage1";
  private bool load_stage_ = false;

	// Use this for initialization
	void Start () {
    Rect pixelInset = new Rect(0, 0, Screen.width, Screen.height);
    guiTexture.color = Color.black;
    guiTexture.pixelInset = pixelInset;
    start_timer_ = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
    // Implement Fading for easier transition
    if (Input.GetKeyDown(KeyCode.Alpha1))
    {
      stage_to_load_ = "Stage1";
      start_timer_ = Time.time;
      load_stage_ = true;
    }
    else if (Input.GetKeyDown(KeyCode.Alpha2))
    {
      stage_to_load_ = "Stage2";
      start_timer_ = Time.time;
      load_stage_ = true;
    }
    else if (Input.GetKeyDown(KeyCode.Alpha3))
    {
      stage_to_load_ = "Stage3";
      start_timer_ = Time.time;
      load_stage_ = true;
    }

    timer_ = Time.time;

    if (load_stage_ && (timer_ - start_timer_) / timer_threshold_ > 1.0f)
    {
      Application.LoadLevel(stage_to_load_);
    }
	}

  

  void OnGUI()
  {
    float alpha = 1.0f - Mathf.Clamp((timer_ - start_timer_)/ timer_threshold_, 0.0f, 1.0f);
    if (load_stage_)
    {
      alpha = 1.0f - alpha;
    }
    Color color = guiTexture.color;
    color.a = alpha;
    guiTexture.color = color;
  }
}
