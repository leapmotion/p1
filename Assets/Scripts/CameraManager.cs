using UnityEngine;
using System.Collections;
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

    public Camera GetActiveCamera()
    {
      return activeCamera;
    }

    private void InitializeSplashScreen()
    {
      float distance_from_camera = splashScreen.transform.localPosition.z;
      float fieldOfView = activeCamera.fieldOfView;
      float aspect = (isConnected) ? activeCamera.aspect * 2 : activeCamera.aspect;
      float y_scale = 2 * distance_from_camera * Mathf.Tan((Mathf.PI * fieldOfView / 180.0f) / 2.0f);
      float x_scale = y_scale * aspect;
      splashScreen.transform.localScale = new Vector3(x_scale, y_scale, 1.0f);
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

      JSONNode data = Utils.FileToJSON("all_config.json");
      handController.transform.localScale = Vector3.one * 20 * data["hand"]["scale"].AsInt;
      handController.GetComponent<HandController>().handMovementScale = Vector3.one * data["hand"]["speed"].AsInt;

      float distance_from_camera = activeCamera.nearClipPlane + 0.1f;
      float fieldOfView = 170.0f; // Initialize at near-full field of view because field of view has not been initialized
      float y_scale = 2 * distance_from_camera * Mathf.Tan((Mathf.PI * fieldOfView / 180.0f) / 2.0f);
      float x_scale = y_scale * activeCamera.aspect;
      splashScreen.transform.parent = activeCamera.transform;
      splashScreen.transform.localPosition = new Vector3(0.0f, 0.0f, distance_from_camera);
      splashScreen.transform.rotation = activeCamera.transform.rotation * splashScreen.transform.rotation;
      splashScreen.transform.localScale = new Vector3(x_scale, y_scale, 1.0f);
	  }
	
	  // Update is called once per frame
    void Update()
    {
      if (initialized < 2) // HACK: Field of view for Oculus does not change until frame 2
      {
        if (initialized == 1)
        {
          InitializeSplashScreen();
        }
        initialized++;
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