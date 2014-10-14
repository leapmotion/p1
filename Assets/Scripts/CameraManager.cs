using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using OVR;
using SimpleJSON;

namespace P1
{
  public class CameraManager : MonoBehaviour {
    public static CameraManager instance; // SINGLETON

    public GameObject normalCamera;
    public GameObject riftCamera;
    public GameObject handController;
    public GameObject fadeScreen;
    private GameObject fadeScreen2 = null;
    public GameObject splashScreen;
    private GameObject splashScreen2 = null;
    public GameObject focusPoint;

    private bool isConnected = false;
    private Camera activeCamera;
    private int initialized = 0;

    public List<string> scenes;
    private bool sceneLoaded = true;
    private float timeThreshold = 2.0f; // 2 seconds
    private float relativeTime;
    private string currentScene_;
    public string currentScene
    {
      get { return currentScene_; }
      set
      {
        currentScene_ = value;
        relativeTime = Time.time;
      }
    }
    public int currentIndex
    {
      get
      {
        int i = 0;
        while (i < scenes.Count)
        {
          if (scenes[i] == currentScene)
            return i;
          else
            ++i;
        }
        return -1;
      }
    }

    public Camera GetActiveCamera()
    {
      return activeCamera;
    }

    public void LoadScenes()
    {
      scenes = new List<string>();
      JSONNode scene_data = Utils.FileToJSON("scene_config.json");
      if (scene_data == null)
        throw new UnityException("No data");

      for (int i = 0; i < scene_data["scenes"].Count; ++i)
      {
        scenes.Add(scene_data["scenes"][i].Value);
      }

      if (Application.isPlaying)
        currentScene = Application.loadedLevelName;
      else
        currentScene = scenes[0];
      relativeTime = Time.time;
    }

    private void CheckForHands()
    {
      if (handController.GetComponent<HandController>().GetFrame().Hands.Count == 0)
      {
        // No hands detected
        ToggleSplashScreen(true);
      }
      else
      {
        ToggleSplashScreen(false);
      }
    }

    private void InitializeScreens(float fieldOfView, float aspect) 
    {
      if (isConnected)
      {
        InitializeScreen(fadeScreen, 0.0001f, fieldOfView, aspect);
        InitializeScreen(fadeScreen2, 0.0001f, fieldOfView, aspect, new Vector3(0.064f, 0.0f, 0.0f));
        InitializeScreen(splashScreen, 0.002f, fieldOfView, 1.0f);
        InitializeScreen(splashScreen2, 0.002f, fieldOfView, 1.0f, new Vector3(0.064f, 0.0f, 0.0f));
      }
      else
      {
        InitializeScreen(fadeScreen, 0.001f, fieldOfView, aspect);
        InitializeScreen(splashScreen, 0.002f, fieldOfView, 1.0f);
      }
    }

    private void InitializeScreen(GameObject screen, float distance, float fieldOfView, float aspect, Vector3 offset = default(Vector3))
    {
      float distance_from_camera = activeCamera.nearClipPlane + distance;
      float y_scale = 2 * distance_from_camera * Mathf.Tan((Mathf.PI * fieldOfView / 180.0f) / 2.0f);
      float x_scale = y_scale * aspect;

      screen.transform.parent = activeCamera.transform;
      screen.transform.localPosition = new Vector3(0.0f, 0.0f, distance_from_camera) + offset;
      screen.transform.localRotation = Quaternion.identity;
      screen.transform.localScale = new Vector3(x_scale, y_scale, 1.0f);
    }

    public void ToggleSplashScreen(bool active)
    {
      splashScreen.SetActive(active);
      if (isConnected)
        splashScreen2.SetActive(active);
    }

    private void UpdateFadeScreen() 
    {
      if (Input.GetKeyDown(KeyCode.RightArrow))
      {
        Debug.Log("Going to Next Scene");
        NextScene();
      }
      else if (Input.GetKeyDown(KeyCode.LeftArrow))
      {
        Debug.Log("Going to Prev Scene");
        PrevScene();
      }

      float alpha = 1.0f - Mathf.Clamp01((Time.time - relativeTime) / timeThreshold);
      if (!sceneLoaded)
      {
        alpha = 1.0f - alpha;
      }
      Color color = fadeScreen.renderer.material.color;
      color.a = alpha;
      fadeScreen.renderer.material.color = color;
      if (isConnected)
        fadeScreen2.renderer.material.color = color;

      if (!sceneLoaded && alpha > 0.99f)
        Application.LoadLevel(currentScene);
    }

    public void PrevScene()
    {
      int i = currentIndex;
      if (i > 0)
      {
        currentScene = scenes[i - 1];
        sceneLoaded = false;
      }
    }

    public void NextScene()
    {
      int i = currentIndex;
      if (i > -1 && (i + 1) < scenes.Count)
      {
        currentScene = scenes[i + 1];
        sceneLoaded = false;
      }
    }

	  // Use this for initialization
	  void Start () {
      Hmd hmd = OVR.Hmd.GetHmd();
      ovrTrackingState ss = hmd.GetTrackingState();
      isConnected = (ss.StatusFlags & (uint)ovrStatusBits.ovrStatus_HmdConnected) != 0;
      float x_offset = 0.0f;
      if (isConnected)
      {
        riftCamera.SetActive(true);
        normalCamera.SetActive(false);
        activeCamera = riftCamera.GetComponent<OVRCameraController>().CameraMain;
        x_offset = riftCamera.GetComponent<OVRCameraController>().IPD / 2.0f;
        splashScreen2 = Instantiate(splashScreen) as GameObject;
        fadeScreen2 = Instantiate(fadeScreen) as GameObject;
      }
      else
      {
        riftCamera.SetActive(false);
        normalCamera.SetActive(true);
        activeCamera = normalCamera.camera;
      }
      handController.transform.parent = activeCamera.transform;
      handController.transform.rotation = activeCamera.transform.rotation * handController.transform.rotation;

      JSONNode data = Utils.FileToJSON("all_config.json");
      handController.transform.localScale = Vector3.one * 1.55f * data["hand"]["scale"].AsInt;
      handController.transform.localPosition = new Vector3(x_offset, 0.0f, 0.08f);
      handController.GetComponent<HandController>().handMovementScale = Vector3.one * data["hand"]["speed"].AsInt;

      InitializeScreens(170.0f, 1.0f); // Initialize at 170.0f field of view to compensate for uninitiated field of view

      LoadScenes();
	  }
	
	  // Update is called once per frame
    void Update()
    {
      if (initialized < 2) // HACK: Field of view for Oculus does not change until frame 2
      {
        if (initialized == 1)
        {
          float aspect = (isConnected) ? activeCamera.aspect * 2.0f : activeCamera.aspect;
          InitializeScreens(activeCamera.fieldOfView, aspect);
        }
        initialized++;
      }

      UpdateFadeScreen();
      if (sceneLoaded)
      {
        CheckForHands();
      }

      if (focusPoint)
      {
        if (Input.GetKeyDown(KeyCode.Space))
        {
          activeCamera.transform.rotation = Quaternion.LookRotation(focusPoint.transform.position - activeCamera.transform.position);
          OVRDevice.ResetOrientation();
        }
      }
	  }
  }
}