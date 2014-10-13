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
    public GameObject splashScreen;
    public GameObject focusPoint;

    private bool isConnected = false;
    private Camera activeCamera;
    private int initialized = 0;

    private List<string> scenes;
    private bool sceneLoaded = false;
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
        sceneLoaded = true;
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

    private void LoadScenes()
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

    private void InitializeSplashScreen(float fieldOfView, float aspect)
    {
      float distance_from_camera = activeCamera.nearClipPlane + 0.1f;
      float y_scale = 2 * distance_from_camera * Mathf.Tan((Mathf.PI * fieldOfView / 180.0f) / 2.0f);
      float x_scale = y_scale * aspect;
      splashScreen.transform.parent = activeCamera.transform;
      splashScreen.transform.localPosition = new Vector3(0.0f, 0.0f, distance_from_camera);
      splashScreen.transform.rotation = activeCamera.transform.rotation * splashScreen.transform.rotation;
      splashScreen.transform.localScale = new Vector3(x_scale, y_scale, 1.0f);
    }

    private void UpdateSplashScreen() 
    {
      float alpha = 1.0f - Mathf.Clamp01((Time.time - relativeTime) / timeThreshold);
      if (sceneLoaded)
      {
        alpha = 1.0f - alpha;
      }

    }

    public void PrevScene()
    {
      int i = currentIndex;
      if (i > 0)
        currentScene = scenes[i - 1];
    }

    public void NextScene()
    {
      int i = currentIndex;
      if (i < scenes.Count - 1)
        currentScene = scenes[i - 1];
    }

	  // Use this for initialization
	  void Start () {
      Hmd hmd = OVR.Hmd.GetHmd();
      ovrTrackingState ss = hmd.GetTrackingState();
      isConnected = (ss.StatusFlags & (uint)ovrStatusBits.ovrStatus_HmdConnected) != 0;
      if (isConnected)
      {
        riftCamera.SetActive(true);
        normalCamera.SetActive(false);
        activeCamera = riftCamera.GetComponent<OVRCameraController>().CameraMain;
      }
      else
      {
        riftCamera.SetActive(false);
        normalCamera.SetActive(true);
        activeCamera = normalCamera.camera;
      }
      handController.transform.parent = activeCamera.transform;
      handController.transform.rotation = activeCamera.transform.rotation * handController.transform.rotation;

      InitializeSplashScreen(170.0f, 1.0f); // Initialize at 170.0f field of view to compensate for uninitiated field of view

      JSONNode data = Utils.FileToJSON("all_config.json");
      handController.transform.localScale = Vector3.one * 20 * data["hand"]["scale"].AsInt;
      handController.GetComponent<HandController>().handMovementScale = Vector3.one * data["hand"]["speed"].AsInt;

      

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
          InitializeSplashScreen(activeCamera.fieldOfView, aspect);
        }
        initialized++;
      }

      if (initialized > 1)
      {
        UpdateSplashScreen();
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