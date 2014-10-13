using UnityEngine;
using System.Collections;
using OVR;
using SimpleJSON;

namespace P1
{
  public class CameraManager : MonoBehaviour {
    public GameObject normalCamera;
    public GameObject riftCamera;
    public GameObject handController;
    public GameObject focusPoint;

    private Camera activeCamera;

	  // Use this for initialization
	  void Start () {
      Hmd hmd = OVR.Hmd.GetHmd();
      ovrTrackingState ss = hmd.GetTrackingState();
      bool isConnected = (ss.StatusFlags & (uint)ovrStatusBits.ovrStatus_HmdConnected) != 0;
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
	  }
	
	  // Update is called once per frame
	  void Update () {
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