using UnityEngine;
using System.Collections;

public class SceneManager : MonoBehaviour {

  Texture2D texture_;
  private float timer_ = 0.0f;
  private float timer_threshold_ = 120.0f;
  private float timer_increment_ = 1.0f;
  private string stage_to_load_ = "Stage1";

	// Use this for initialization
	void Start () {
    Rect pixelInset = new Rect(0, 0, Screen.width, Screen.height);
    guiTexture.color = Color.black;
    guiTexture.pixelInset = pixelInset;
	}
	
	// Update is called once per frame
	void Update () {
    // Implement Fading for easier transition
    if (Input.GetKeyDown(KeyCode.Alpha1))
    {
      stage_to_load_ = "Stage1";
      timer_increment_ = -1.0f;
    }
    else if (Input.GetKeyDown(KeyCode.Alpha2))
    {
      stage_to_load_ = "Stage2";
      timer_increment_ = -1.0f;
    }
    else if (Input.GetKeyDown(KeyCode.Alpha3))
    {
      stage_to_load_ = "Stage3";
      timer_increment_ = -1.0f;
    }

    if (timer_ < 0.0f)
    {
      Application.LoadLevel(stage_to_load_);
    }

    timer_ += timer_increment_;
    if (timer_ > timer_threshold_)
    {
      timer_ = timer_threshold_;
    }
    Debug.Log(timer_);
	}

  

  void OnGUI()
  {
    float alpha = 1.0f - Mathf.Clamp(timer_ / timer_threshold_, 0.0f, 1.0f);
    Color color = guiTexture.color;
    color.a = alpha;
    guiTexture.color = color;
  }
}
